using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class LongWrapper
    {
        [ProtoMember(1)]
        private long _value;

        public static LongWrapper Create(long value)
        {
            return value;
        }
        public static implicit operator LongWrapper(long value)
        {
            return new LongWrapper() { _value = value };
        }
        public static implicit operator long(LongWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
