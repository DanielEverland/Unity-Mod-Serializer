using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class ByteWrapper
    {
        [ProtoMember(1)]
        private byte _value;

        public static ByteWrapper Create(byte value)
        {
            return value;
        }
        public static implicit operator ByteWrapper(byte value)
        {
            return new ByteWrapper() { _value = value };
        }
        public static implicit operator byte(ByteWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
