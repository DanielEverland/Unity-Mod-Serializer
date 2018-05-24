using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class Color32Model : ModelBase<Color32>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("r", "g", "b", "a");
        }
    }
}
