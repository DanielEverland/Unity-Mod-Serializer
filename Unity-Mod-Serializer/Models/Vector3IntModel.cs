using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class Vector3IntModel : ModelBase<Vector3Int>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("x");
            type.Add("y");
            type.Add("z");
        }
    }
}
