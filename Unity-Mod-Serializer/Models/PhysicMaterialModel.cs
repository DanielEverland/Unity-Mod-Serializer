using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class PhysicMaterialModel : ModelBase<PhysicMaterial>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("name", "dynamicFriction", "staticFriction", "bounciness", "frictionCombine");
        }
    }
}
