using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Operators
{
    public abstract class BaseOperator
    {
        public abstract Type ModelType { get; }

        public abstract object Serialize(object obj);
    }
    public abstract class BaseOperator<T> : BaseOperator
    {
        public override Type ModelType => typeof(T);

        public override object Serialize(object obj)
        {
            return Serialize((T)obj);
        }

        public abstract object Serialize(T obj);
    }
}
