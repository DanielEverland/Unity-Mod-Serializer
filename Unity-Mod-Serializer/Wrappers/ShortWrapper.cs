using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class ShortWrapper
    {
        [ProtoMember(1)]
        private short _value;

        public static ShortWrapper Create(short value)
        {
            return value;
        }
        public static implicit operator ShortWrapper(short value)
        {
            return new ShortWrapper() { _value = value };
        }
        public static implicit operator short(ShortWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
