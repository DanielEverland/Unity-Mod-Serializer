using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UMS.Editor
{
    #region DEBUG
    public static class TestingFunctions
    {
        private const string ROOT = "Modding/Tests/";
        private const string ROOT_VALUE_TYPES = ROOT + "Value Types/";

        private const string KEY_PRIMITIVES = "Primitives";
        private const string KEY_REFLECTION = "Reflection";
        private const string KEY_VECTORS = "Vectors";
        private const string KEY_QUATERNION = "Quaternion";
        
        [MenuItem(ROOT_VALUE_TYPES + KEY_QUATERNION, priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestQuaternion()
        {
            StartTest(KEY_QUATERNION);

            Test(Quaternion.Euler(Utility.GetRandomFloat(), Utility.GetRandomFloat(), Utility.GetRandomFloat()));

            EndTest(KEY_QUATERNION);
        }
        [MenuItem(ROOT_VALUE_TYPES + KEY_VECTORS, priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestVectors()
        {
            StartTest(KEY_VECTORS);

            Test(new Vector2(1.5432f, 3466f));
            Test(new Vector3(1.5432f, 3466f, 1.5432f));
            Test(new Vector4(1.5432f, 3466f, 1.5432f, 3466f));

            Test(new Vector2Int(5, 10));
            Test(new Vector3Int(15, 143, 65));

            EndTest(KEY_VECTORS);
        }
        [MenuItem(ROOT + KEY_REFLECTION, priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestReflection()
        {
            StartTest(KEY_REFLECTION);

            ReflectionTestObject obj = new ReflectionTestObject();
            Test(obj);

            EndTest(KEY_REFLECTION);
        }
        [MenuItem(ROOT_VALUE_TYPES + KEY_PRIMITIVES, priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestPrimitives()
        {
            StartTest(KEY_PRIMITIVES);
            
            Test(true);
            Test((byte)69);
            Test((sbyte)-1);
            Test('%');
            Test(decimal.MaxValue);
            Test(double.MaxValue);
            Test(float.MaxValue);
            Test(int.MaxValue);
            Test(uint.MaxValue);
            Test(long.MaxValue);
            Test(ulong.MaxValue);
            Test(short.MaxValue);
            Test(ushort.MaxValue);
            Test("This is a string");

            EndTest(KEY_PRIMITIVES);
        }
        private static void StartTest(string testName)
        {
            Debug.Log(string.Format("-----STARTING {0}-----", testName.ToUpper()));
        }
        private static void EndTest(string testName)
        {
            Debug.Log(string.Format("-----FINISHED {0}-----", testName.ToUpper()));
        }
        private static void Test(object obj)
        {
            try
            {
                object deserializedObject = null;

                Result result = Result.Success;

                result += Serializer.Serialize(obj, out Data data);

                if (result.Succeeded)
                {
                    Debug.Log(string.Format("SERIALIZED {0} ({1}) to {2}", obj, obj.GetType(), data));
                }
                else
                {
                    Debug.LogError(string.Format("Failed serializing {0} ({1})", obj, obj.GetType()));
                }

                result += Serializer.Deserialize(data.SerializeToBytes(), obj.GetType(), ref deserializedObject);

                if (result.Succeeded)
                {
                    Debug.Log(string.Format("DESERIALIZED {0} ({1}) to {2}", data, obj.GetType(), deserializedObject));
                }
                else
                {
                    Debug.LogError(string.Format("Failed deserializing {0} ({1})", data, obj.GetType()));
                }

                if (result.Succeeded)
                {
                    Debug.Log(string.Format("Testing {0} ({1}) succeeded\n{2}\n{3}", obj, obj.GetType().Name, data, result));
                }
                else
                {
                    result.AssertWithoutWarnings();
                }
            }
            catch (System.Exception)
            {
                Debug.LogError(string.Format("Exception thrown while testing {0} ({1})", obj, obj.GetType().Name));
                throw;
            }
        }
#pragma warning disable
        private class ReflectionTestObject
        {
            public float A = 420;

            [Ignore]
            public string DontSerializeA = "IgnoreMe";

            [SerializeField]
            private ulong B = ulong.MaxValue;

            private ushort DontSerializeB = ushort.MinValue;                
        }
#pragma warning restore
    }
    #endregion
}
