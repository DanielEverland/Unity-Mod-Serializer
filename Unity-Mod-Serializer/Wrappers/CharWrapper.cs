using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class CharWrapper
    {
        [ProtoMember(1)]
        private char _value;

        public static CharWrapper Create(char value)
        {
            return value;
        }
        public static implicit operator CharWrapper(char value)
        {
            return new CharWrapper() { _value = value };
        }
        public static implicit operator char(CharWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
