using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class UnsignedShortWrapper
    {
        [ProtoMember(1)]
        private ushort _value;

        public static UnsignedShortWrapper Create(ushort value)
        {
            return value;
        }
        public static implicit operator UnsignedShortWrapper(ushort value)
        {
            return new UnsignedShortWrapper() { _value = value };
        }
        public static implicit operator ushort(UnsignedShortWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
