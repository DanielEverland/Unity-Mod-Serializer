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
        public SerializableComponent(Component comp)
        {
            type = comp.GetType();
        }

        [ProtoMember(1)]
        private System.Type type;

        public void Deserialize(GameObject obj)
        {
            Component component = GetComponent(obj);
        }
        private Component GetComponent(GameObject obj)
        {
            if (type == typeof(Transform))
                return obj.GetComponent<Transform>();

            return obj.AddComponent(type);
        }

        public static implicit operator SerializableComponent(Component comp)
        {
            return comp == null ? null : new SerializableComponent(comp);
        }
    }
}
