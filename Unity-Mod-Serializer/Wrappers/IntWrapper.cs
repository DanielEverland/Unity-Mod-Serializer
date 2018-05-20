using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class IntWrapper
    {
        [ProtoMember(1)]
        private int _value;

        public static IntWrapper Create(int value)
        {
            return value;
        }
        public static implicit operator IntWrapper(int value)
        {
            return new IntWrapper() { _value = value };
        }
        public static implicit operator int(IntWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
