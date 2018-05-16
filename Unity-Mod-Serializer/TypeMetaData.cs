using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS
{
    [ProtoContract]
    public sealed class TypeMetaData : IMetaData
    {
        public TypeMetaData()
        {
        }
        public TypeMetaData(Type type)
        {
            Type = type;
        }

        [ProtoMember(1)]
        public Type Type { get; set; }

        public override string ToString()
        {
            return $"$type({Type.Name})";
        }
    }
}
