using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class BoneWeightModel : ModelBase<BoneWeight>
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("weight0", "weight1", "weight2", "weight3");
            type.Add("boneIndex0", "boneIndex1", "boneIndex2", "boneIndex3");
        }
    }
}
