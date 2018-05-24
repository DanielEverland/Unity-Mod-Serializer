using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;
using ProtoBuf.Meta;

namespace UMS.Models
{
    public class GameObjectModel : ModelBase<GameObject>
    {
        public override void CreateModel(MetaType type)
        {
            type.AsReferenceDefault = true;
            type.SetSurrogate(typeof(GameObjectSurrogate));
        }

        [ProtoContract]
        private class GameObjectSurrogate
        {
            public GameObjectSurrogate() { }
            public GameObjectSurrogate(GameObject obj)
            {
                name = obj.name;

                components = new List<SerializableComponent>();
                foreach (Component comp in obj.GetComponents<Component>())
                {
                    components.Add(new SerializableComponent(comp));
                }
            }

            [ProtoMember(1)]
            private string name;
            [ProtoMember(2)]
            private List<SerializableComponent> components;

            public GameObject Deserialize()
            {
                GameObject obj = new GameObject();

                obj.name = name;

                //Protobuf doesn't serialize empty collections, so we have to do a null check here
                if(components != null)
                {
                    foreach (SerializableComponent comp in components)
                    {
                        comp.Deserialize(obj);
                    }
                }

                return obj;
            }

            public static implicit operator GameObjectSurrogate (GameObject obj)
            {
                return obj == null ? null : new GameObjectSurrogate(obj);
            }
            public static implicit operator GameObject (GameObjectSurrogate surrogate)
            {
                return surrogate == null ? null : surrogate.Deserialize();
            }
        }
    }
}
