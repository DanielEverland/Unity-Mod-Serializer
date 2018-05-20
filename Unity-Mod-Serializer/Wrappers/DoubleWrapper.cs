using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class DoubleWrapper
    {
        [ProtoMember(1)]
        private double _value;

        public static DoubleWrapper Create(double value)
        {
            return value;
        }
        public static implicit operator DoubleWrapper(double value)
        {
            return new DoubleWrapper() { _value = value };
        }
        public static implicit operator double(DoubleWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
