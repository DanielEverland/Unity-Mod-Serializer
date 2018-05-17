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
        public override void CreateModel(MetaType type)
        {
            type.SetSurrogate(typeof(GameObjectSurrogate));
        }

        [ProtoContract]
        private class GameObjectSurrogate
        {
            [ProtoMember(1)]
            public string Name;

            public static implicit operator GameObjectSurrogate (GameObject obj)
            {
                if (obj == null)
                    return null;

                GameObjectSurrogate surrogate = new GameObjectSurrogate();
                surrogate.Name = obj.name;

                foreach (Component component in obj.GetComponents<Component>())
                {
                    Debug.Log("Serialized " + component);
                }

                return surrogate;
            }
            public static implicit operator GameObject (GameObjectSurrogate surrogate)
            {
                if (surrogate == null)
                    return null;

                GameObject obj = new GameObject();

                obj.name = surrogate.Name;

                return obj;
            }
        }
    }
}
