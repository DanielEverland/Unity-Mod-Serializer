using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMS.Reflection;
using ProtoBuf.Meta;
using UnityEngine;

namespace UMS.Models
{
    public class ComponentModel : ObjectModel<Component>
    {
        public ComponentModel(System.Type type)
        {
            ModelType = type;
        }

        public override void CreateModel(MetaType type)
        {
            base.CreateModel(type);

            ReflectionHelper.CreateMetaType(type, ModelType);
        }
    }
}
