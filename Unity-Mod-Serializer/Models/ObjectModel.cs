using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public abstract class ObjectModel<T> : ModelBase<T> where T : Object
    {
        public override void CreateModel(MetaType type)
        {
            type.Add("name");
            type.Add("hideFlags");
        }
    }
}
