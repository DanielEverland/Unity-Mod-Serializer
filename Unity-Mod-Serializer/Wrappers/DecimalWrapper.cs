using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class DecimalWrapper
    {
        [ProtoMember(1)]
        private decimal _value;

        public static DecimalWrapper Create(decimal value)
        {
            return value;
        }
        public static implicit operator DecimalWrapper(decimal value)
        {
            return new DecimalWrapper() { _value = value };
        }
        public static implicit operator decimal(DecimalWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
