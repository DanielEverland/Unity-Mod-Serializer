using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;
using UMS.Reflection;

namespace UMS
{
    [ProtoContract]
    public class SerializableComponent
    {
        public SerializableComponent() { }
        public SerializableComponent(Component comp)
        {
            type = comp.GetType();
            data = ReflectionHelper.Serialize(comp);
        }

        [ProtoMember(1)]
        private System.Type type;
        [ProtoMember(2)]
        private List<MemberValue> data;

        public void Deserialize(GameObject obj)
        {
            Component component = GetComponent(obj);

            ReflectionHelper.Deserialize(component, data);
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
