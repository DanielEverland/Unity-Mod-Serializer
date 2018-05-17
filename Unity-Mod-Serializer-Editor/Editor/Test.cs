using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ProtoBuf;
using ProtoBuf.Meta;
using System.IO;
using UMS;

namespace UMS.Editor
{
    public static class Test
    {
        private static TestData toSerialize = new TestData()
        {
            Name = "I am serialized",
            HideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable,
            ExtraData = new ExtraData()
            {
                Value = 435415,
            },
        };

        private const int ITERATIONS = 10000;
        private static RuntimeTypeModel _model;

        #region Surrogate Test
        [MenuItem("Modding/Test Surrogate")]
        private static void TestSurrogate()
        {
            RuntimeTypeModel model = TypeModel.Create();

            MetaType metaType = model.Add(typeof(BaseClass), false);
            metaType.SetSurrogate(typeof(Surrogate));

            BaseClass obj = new BaseClass("Test");
            
            using (MemoryStream stream = new MemoryStream())
            {
                model.Serialize(stream, obj);
                model.Deserialize(stream, null, typeof(BaseClass));
            }
        }
        private class BaseClass
        {
            public BaseClass()
            {
            }
            public BaseClass(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
        }
        [ProtoContract]
        private class Surrogate
        {
            public Surrogate()
            {
            }
            public Surrogate(string name)
            {
                this.name = name;
            }

            [ProtoMember(1)]
            public string name;

            public static implicit operator Surrogate (BaseClass obj)
            {
                return obj == null ? null : new Surrogate(obj.Name);
            }
            public static implicit operator BaseClass (Surrogate surrogate)
            {
                return surrogate == null ? null : new BaseClass(surrogate.name);
            }

            public override string ToString()
            {
                return name;
            }
        }
        #endregion
        [MenuItem("Modding/Run Protobuf Test")]
        private static void RunTest()
        {
            _model = TypeModel.Create();
            _model.AutoAddProtoContractTypesOnly = false;

            MetaType testDataType = _model.Add(typeof(TestData), false);
            testDataType.Add("Name");
            testDataType.Add("HideFlags");
            testDataType.Add("ExtraData");

            MetaType extraDataType = _model.Add(typeof(ExtraData), false);
            extraDataType.Add("Value");

            Benchmarker.Profile("Runtime Model", ITERATIONS, TestRuntimeModel);
            Benchmarker.Profile("Contract Model", ITERATIONS, TestContractModel);
        }
        private static void TestContractModel()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, toSerialize);
                data = stream.ToArray();
            }

            using (MemoryStream stream2 = new MemoryStream(data))
            {
                ProtoBuf.Serializer.Deserialize(typeof(TestData), stream2);
            }
        }
        private static void TestRuntimeModel()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                _model.Serialize(stream, toSerialize);
                data = stream.ToArray();
            }

            using (MemoryStream stream2 = new MemoryStream(data))
            {
                _model.Deserialize(stream2, null, typeof(TestData));
            }
        }

        [ProtoContract()]
        private class TestData
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public HideFlags HideFlags;
            [ProtoMember(3)]
            public ExtraData ExtraData;

            public override string ToString()
            {
                return $"{Name}, {HideFlags}, {ExtraData}";
            }
        }
        [ProtoContract()]
        private class ExtraData
        {
            [ProtoMember(1)]
            public int Value;

            public override string ToString()
            {
                return Value.ToString();
            }
        }
    }
}
