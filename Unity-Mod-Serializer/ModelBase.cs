using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;

namespace UMS
{
    public abstract class ModelBase<T> : IModel
    {
        public Type ModelType => typeof(T);

        public abstract void CreateModel(MetaType type);
    }
}
