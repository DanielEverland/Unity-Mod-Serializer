using System.Collections.Generic;
using System.Collections;
using UMS.Reflection;
using System.Linq;
using UnityEngine;
using ProtoBuf.Meta;
using ProtoBuf;

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

            public static implicit operator ComponentSurrogate(Component obj)
            {
                if (obj == null)
                    return null;

                ComponentSurrogate surrogate = new ComponentSurrogate();
                surrogate.Type = obj.GetType();

                return surrogate;
            }
            public static implicit operator Component(ComponentSurrogate surrogate)
            {
                if (surrogate == null)
                    return null;
                
                Component component = GetComponent(GameObjectModel.CurrentInstance, surrogate.Type);
                
                return null;
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
