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
            }

            [ProtoMember(1)]
            private string name;

            public GameObject Deserialize()
            {
                GameObject obj = new GameObject();

                obj.name = name;

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
