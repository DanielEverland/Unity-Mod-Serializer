using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class Vector2Model : ModelBase<Vector2>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("x", "y");
        }
    }
}
