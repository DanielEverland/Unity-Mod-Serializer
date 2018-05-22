using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;

namespace UMS.Operators
{
    public class GameObjectOperator : BaseOperator<GameObject>
    {
        public override object Serialize(GameObject obj)
        {
            return new GameObjectModel(obj);
        }

        [ProtoContract]
        private class GameObjectModel
        {
            public GameObjectModel() { }
            public GameObjectModel(GameObject obj)
            {
                name = obj.name;
            }

            [ProtoMember(1)]
            private string name;

            [ProtoAfterDeserialization]
            private void BeforeDeserialize()
            {
                GameObject obj = new GameObject();
                obj.name = name;
            }
        }
    }
}
