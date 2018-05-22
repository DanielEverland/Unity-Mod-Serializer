using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ProtoBuf;
using ProtoBuf.Meta;
using System.IO;
using UMS;
using FastMember;

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
        private static string _value;
        private static TypeAccessor _accessor;
        private static ReflectionData _data;
        private static string _fieldName;
        
        [MenuItem("Modding/Test GameObject")]
        private static void TestGameObject()
        {
            Session.Initialize();

            GameObject obj = new GameObject();
            obj.name = "This is a test";
            obj.transform.position = new Vector3(155, 45, 65);

            object toSerialize = Operator.Serialize(obj);

            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, toSerialize);
                Debug.Log($"Serializing ({stream.Length})");
                
                data = stream.ToArray();
            }

            using (MemoryStream stream2 = new MemoryStream(data))
            {
                Debug.Log("Deserilizing...");
                Debug.Log(ProtoBuf.Serializer.Deserialize(toSerialize.GetType(), stream2));
            }
        }
        [MenuItem("Modding/Test Field")]
        private static void TestField()
        {
            _data = new ReflectionData();
            _data.Field = "Field Value";
            _data.Property = "Property Value";

            _fieldName = "Property";
            _value = "test";

            PropertyInfo property = _data.GetType().GetProperty(_fieldName);
            MethodInfo propertyGetMethod = property.GetGetMethod();

            _accessor = TypeAccessor.Create(_data.GetType());

            Benchmarker.Profile("Creation", ITERATIONS, () => { TypeAccessor.Create(_data.GetType()); });
            
            Benchmarker.Profile("FastMember", ITERATIONS, TestFastMember);
            Benchmarker.Profile("Regular", ITERATIONS, TestRegularAccessor);
        }
        private static void TestFastMember()
        {
            _accessor[_data, _fieldName] = _value;
        }
        private static void TestRegularAccessor()
        {
            _data.Property = _value;
        }
        private class ReflectionData
        {
            public string Field;
            public string Property { get; set; }
        }

        #region Surrogate Test
        [MenuItem("Modding/Test Surrogate")]
        private static void TestSurrogate()
        {
            RuntimeTypeModel model = TypeModel.Create();

            MetaType metaType = model.Add(typeof(GameObject), false);
            metaType.SetSurrogate(typeof(GameObjectSurrogate));

            GameObject toSerialize = new GameObject();
            toSerialize.name = "This is a test name";
            toSerialize.transform.position = new Vector3(64, 1, 54);

            ExecuteSurrogateTest(toSerialize, model);
        }
        private static void ExecuteSurrogateTest(object obj, RuntimeTypeModel model)
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                model.Serialize(stream, obj);
                Debug.Log($"Serializing ({stream.Length})");

                Debug.Log("Deserilizing First Pass...");
                Debug.Log(model.Deserialize(stream, null, obj.GetType()));

                data = stream.ToArray();
            }

            using (MemoryStream stream2 = new MemoryStream(data))
            {
                Debug.Log("Deserilizing Second Pass...");
                Debug.Log(model.Deserialize(stream2, null, obj.GetType()));
            }
        }
        [ProtoContract]
        private class GameObjectSurrogate
        {
            public GameObjectSurrogate()
            {
            }
            public GameObjectSurrogate(GameObject obj)
            {
                name = obj.name;
            }

            [ProtoMember(1)]
            public string name;

            public static implicit operator GameObjectSurrogate(GameObject obj)
            {
                return obj == null ? null : new GameObjectSurrogate(obj);
            }
            public static implicit operator GameObject(GameObjectSurrogate surrogate)
            {
                return surrogate == null ? null : new GameObject(surrogate.name);
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
