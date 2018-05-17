using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public override object CreateInstance(Type type)
        {
            return new PhysicMaterial();
        }
        public override Result DoSerialize(PhysicMaterial obj, out Data data)
        {
            Result result = Result.Success;
            data = Data.CreateDictionary();

            result += SerializeMembers(data, obj, _members);
            result += UnityEngineObjectHelper.Serialize(data, obj);

            return result;
        }
        public override Result DoDeserialize(Data data, ref PhysicMaterial obj)
        {
            Result result = Result.Success;

            result += SerializeMembers(data, obj, _members);
            result += UnityEngineObjectHelper.Deserialize(data, obj);

            return result;
        }
    }
}
