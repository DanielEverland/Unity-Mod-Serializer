﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class Vector4Model : ModelBase<Vector4>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("x", "y", "z", "w");
        }
    }
}
