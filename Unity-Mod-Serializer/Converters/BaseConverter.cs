﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        
        public abstract Result Serialize(T value, out Data data);
        public abstract Result Deserialize(Data data, out T obj);

        public Result Serialize(object obj, out Data data)
        {
            return Serialize((T)obj, out data);
        }
        public Result Deserialize(Data data, out object obj)
        {
            T outObject = default(T);
            Result result = Deserialize(data, out outObject);

            obj = outObject;
            return result;
        }
    }
}