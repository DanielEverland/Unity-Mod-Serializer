using System;

namespace UMS
{
    /// <summary>
    /// The serialization converter allows for customization of the serialization
    /// process.
    /// </summary>
    /// <typeparam name="T">The type this converter can convert. Supports inheritance</typeparam>
    public abstract class Converter : BaseConverter
    {
        public abstract Type ModelType { get; }
    }
    public abstract class Converter<T> : Converter
    {
        public sealed override Type ModelType { get { return typeof(T); } }
    }
}