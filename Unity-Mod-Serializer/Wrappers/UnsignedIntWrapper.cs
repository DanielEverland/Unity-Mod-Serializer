using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class UnsignedIntWrapper
    {
        [ProtoMember(1)]
        private uint _value;

        public static UnsignedIntWrapper Create(uint value)
        {
            return value;
        }
        public static implicit operator UnsignedIntWrapper(uint value)
        {
            return new UnsignedIntWrapper() { _value = value };
        }
        public static implicit operator uint(UnsignedIntWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
