using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;
using UMS.Reflection;

namespace UMS
{
    [ProtoContract()]
    public class ComponentData
    {
        public ComponentData() { }
        public ComponentData(Component component)
        {
            Serialize(component);
        }

        [ProtoMember(1)]
        public System.Type Type;
        [ProtoMember(2, DynamicType = true)]
        public Dictionary<int, object> Data;
        
        public void Serialize(Component component)
        {
            Type = component.GetType();
            Data = ReflectionHelper.Serialize(component);
        }
        public void Deserialize(GameObject obj)
        {
            ReflectionHelper.Deserialize(GetComponent(obj, Type), Data);
        }
        private static Component GetComponent(GameObject obj, System.Type type)
        {
            if (typeof(Transform).IsAssignableFrom(type))
                return obj.GetComponent(type);

            return obj.AddComponent(type);
        }
    }
}
