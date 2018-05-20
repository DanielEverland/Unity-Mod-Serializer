using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class StringWrapper
    {
        [ProtoMember(1)]
        private string _value;

        public static StringWrapper Create(string value)
        {
            return value;
        }
        public static implicit operator StringWrapper(string value)
        {
            return new StringWrapper() { _value = value };
        }
        public static implicit operator string(StringWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
