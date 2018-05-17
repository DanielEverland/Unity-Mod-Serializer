using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class GameObjectModel : ObjectModel<GameObject>
    {
        public override void CreateModel(MetaType type)
        {
            base.CreateModel(type);
        }
    }
}
