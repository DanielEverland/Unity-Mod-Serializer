using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class GameObjectModel : ObjectModel<GameObject>
    {
        /// <summary>
        /// This is used for components so they know which object owns them
        /// </summary>
        public static GameObject CurrentInstance { get; set; }

        public override void CreateModel(MetaType type)
        {
            type.AsReferenceDefault = true;
            type.SetSurrogate(typeof(GameObjectSurrogate));
        }

        [ProtoContract]
        private class GameObjectSurrogate
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public List<Component> Components = new List<Component>();
            
            [ProtoBeforeDeserialization()]
            private void BeforeDeserialize()
            {
                //We create a static instance that all the data on our
                //surrogate will deserialize into
                CurrentInstance = new GameObject();
            }

            public static implicit operator GameObjectSurrogate (GameObject obj)
            {
                if (obj == null)
                    return null;

                GameObjectSurrogate surrogate = new GameObjectSurrogate();
                surrogate.Name = obj.name;

                foreach (Component component in obj.GetComponents<Component>())
                {
                    surrogate.Components.Add(component);
                }

                return surrogate;
            }
            public static implicit operator GameObject (GameObjectSurrogate surrogate)
            {
                if (surrogate == null)
                    return null;

                //We consume the static instance
                GameObject obj = CurrentInstance;
                CurrentInstance = null;

                obj.name = surrogate.Name;

                return obj;
            }
        }
    }
}
