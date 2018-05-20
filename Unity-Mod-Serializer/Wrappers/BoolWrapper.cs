using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS.Wrappers
{
    [ProtoContract]
    public class BoolWrapper
    {
        [ProtoMember(1)]
        private bool _value;
        
        public static BoolWrapper Create(bool value)
        {
            return value;
        }
        public static implicit operator BoolWrapper (bool value)
        {
            return new BoolWrapper() { _value = value };
        }
        public static implicit operator bool (BoolWrapper wrapper)
        {
            return wrapper._value;
        }
    }
}
