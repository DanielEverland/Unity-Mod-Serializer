﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class QuaternionModel : ModelBase<Quaternion>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("x");
            type.Add("y");
            type.Add("z");
            type.Add("w");
        }
    }
}
