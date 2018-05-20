using System.Collections.Generic;
using System.Collections;
using UMS.Reflection;
using System.Linq;
using UnityEngine;
using ProtoBuf.Meta;
using ProtoBuf;
using FastMember;

namespace UMS.Models
{
    public class ComponentModel : ObjectModel<Component>
    {
        public override void CreateModel(MetaType type)
        {
            type.AsReferenceDefault = true;
            type.SetSurrogate(typeof(ComponentSurrogate));
        }

        [ProtoContract]
        private class ComponentSurrogate
        {
            [ProtoMember(1)]
            public System.Type Type;
            [ProtoMember(2, DynamicType = true)]
            public Dictionary<int, object> Data;

            public static implicit operator ComponentSurrogate(Component obj)
            {
                if (obj == null)
                    return null;

                ComponentSurrogate surrogate = new ComponentSurrogate();
                surrogate.Type = obj.GetType();
                surrogate.Data = ReflectionHelper.Serialize(obj);
                
                return surrogate;
            }
            public static implicit operator Component(ComponentSurrogate surrogate)
            {
                if (surrogate == null)
                    return null;
                
                Component component = GetComponent(GameObjectModel.CurrentInstance, surrogate.Type);

                //Data won't be serialized if it's empty, so we have to do a null check here
                if(surrogate.Data != null)
                {
                    ReflectionHelper.Deserialize(component, surrogate.Data);
                }                

                return component;
            }
            private static Component GetComponent(GameObject obj, System.Type type)
            {
                if (typeof(Transform).IsAssignableFrom(type))
                    return obj.GetComponent(type);

                return obj.AddComponent(type);
            }
        }
    }
}
