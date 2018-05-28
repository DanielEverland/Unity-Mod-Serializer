using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS
{
    [System.Serializable]
    public struct SerializableGUID
    {
        [SerializeField]
        private string _value;

        public static implicit operator SerializableGUID(System.Guid guid)
        {
            return new SerializableGUID()
            {
                _value = guid.ToString(),
            };
        }
        public static implicit operator System.Guid(SerializableGUID serializable)
        {
            return System.Guid.Parse(serializable._value);
        }
    }
}
