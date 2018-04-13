namespace UMS.Converters
{
    /// <summary>
    /// Converters converts object from memory into a serializable data format.
    /// </summary>
    public abstract class BaseConverter<T> : IBaseConverter
    {
        /// <summary>
        /// The type this converter supports
        /// </summary>
        public System.Type ModelType { get { return typeof(T); } }
        
        public abstract Result DoSerialize(T obj, out Data data);
        public abstract Result DoDeserialize(Data data, ref T obj);

        public Result Serialize(object obj, out Data data)
        {
            return DoSerialize((T)obj, out data);
        }
        public Result Deserialize(Data data, ref object obj)
        {
            T outObject = default(T);
            Result result = DoDeserialize(data, ref outObject);

            obj = outObject;
            return result;
        }
    }
}
