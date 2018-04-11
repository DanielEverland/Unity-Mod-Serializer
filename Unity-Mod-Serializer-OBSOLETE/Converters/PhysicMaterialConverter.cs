using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UMS.ConverterHelpers;

namespace UMS.Converters
{
    public class PhysicMaterialConverter : DirectConverter<PhysicMaterial>
    {
        private static readonly List<string> _members = new List<string>()
        {
            "dynamicFriction",
            "staticFriction",
            "bounciness",
            "frictionCombine",
            "bounceCombine",
        };

        public override object CreateInstance(Data data, Type storageType)
        {
            return new PhysicMaterial();
        }
        protected override Result DoSerialize(PhysicMaterial model, Dictionary<string, Data> serialized)
        {
            Result result = Result.Success;

            result += SerializeMembers(serialized, model, _members);
            result += UnityEngineObjectHelper.TrySerialize(serialized, model);

            return result;
        }
        protected override Result DoDeserialize(Dictionary<string, Data> data, ref PhysicMaterial model)
        {
            Result result = Result.Success;

            result += SerializeMembers(data, model, _members);
            result += UnityEngineObjectHelper.TryDeserialize(data, model);

            return result;
        }
    }
}
