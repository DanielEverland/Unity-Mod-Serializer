using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class Matrix4x4Model : ModelBase<Matrix4x4>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("m00", "m33", "m23", "m13", "m03", "m32", "m22", "m02", "m12", "m21", "m11", "m01", "m30", "m20", "m10", "m31");
        }
    }
}
