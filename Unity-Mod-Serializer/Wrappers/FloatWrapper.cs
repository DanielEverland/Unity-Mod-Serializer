using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class FloatWrapper
    {
        [ProtoMember(1)]
        private float _value;

        public static FloatWrapper Create(float value)
        {
            return value;
        }
        public static implicit operator FloatWrapper(float value)
        {
            return new FloatWrapper() { _value = value };
        }
        public static implicit operator float(FloatWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
