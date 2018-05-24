using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class ColorModel : ModelBase<Color>
    {
        public override void CreateModel(MetaType type)
        {
            type.SetSurrogate(typeof(Color32));
        }
    }
}
