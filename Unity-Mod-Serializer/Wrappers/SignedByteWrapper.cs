using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class SignedByteWrapper
    {
        [ProtoMember(1)]
        private sbyte _value;

        public static SignedByteWrapper Create(sbyte value)
        {
            return value;
        }
        public static implicit operator SignedByteWrapper(sbyte value)
        {
            return new SignedByteWrapper() { _value = value };
        }
        public static implicit operator sbyte(SignedByteWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
