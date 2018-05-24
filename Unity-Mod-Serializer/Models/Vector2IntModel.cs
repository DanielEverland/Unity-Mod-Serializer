using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class Vector2IntModel : ModelBase<Vector2Int>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("x");
            type.Add("y");
        }
    }
}
