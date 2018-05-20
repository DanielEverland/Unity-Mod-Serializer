using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class UnsignedLongWrapper
    {
        [ProtoMember(1)]
        private ulong _value;

        public static UnsignedLongWrapper Create(ulong value)
        {
            return value;
        }
        public static implicit operator UnsignedLongWrapper(ulong value)
        {
            return new UnsignedLongWrapper() { _value = value };
        }
        public static implicit operator ulong(UnsignedLongWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
