using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;

namespace UMS
{
    [ProtoContract]
    public class SerializableComponent
    {
        public SerializableComponent() { }
        public SerializableComponent(Component component)
        {
            type = component.GetType();
        }

        [ProtoMember(1)]
        private System.Type type;
        
        public void Deserialize(GameObject obj)
        {
            Component component = GetComponent(obj);
        }
        private Component GetComponent(GameObject obj)
        {
            if (typeof(Transform).IsAssignableFrom(type))
                return obj.GetComponent(type);

            return obj.AddComponent(type);
        }
    }
}
