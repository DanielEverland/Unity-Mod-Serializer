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

        public virtual object CreateInstance(System.Type type)
        {
            try
            {
                return System.Activator.CreateInstance(type);
            }
            catch (System.Exception)
            {
                throw new System.NotImplementedException("Please implement CreateInstance on " + GetType().Name);
            };
        }
        public Result Serialize(object obj, out Data data)
        {
            return DoSerialize((T)obj, out data);
        }
        public Result Deserialize(Data data, ref object obj)
        {
            System.Type objType = obj.GetType();

            if (!typeof(T).IsAssignableFrom(objType))
                throw new System.ArgumentException(string.Format("Type mismatch. Cannot deserialize ({0}) using Converter type ({1})", objType, typeof(T)));

            T outObject = (T)obj;
            Result result = DoDeserialize(data, ref outObject);

            obj = outObject;
            return result;
        }
    }
}
