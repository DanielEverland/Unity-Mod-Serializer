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

            [ProtoAfterDeserialization]
            private void Deserialize()
            {
                GameObject obj = new GameObject();
                obj.name = name;

                foreach (SerializableComponent comp in components)
                {
                    comp.Deserialize(obj);
                }
            }
        }
    }
}
