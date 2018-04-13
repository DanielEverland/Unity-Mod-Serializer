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

        private const string KEY_PRIMITIVES = "Primitives";
        private const string KEY_REFLECTION = "Reflection";

        [MenuItem(ROOT + KEY_REFLECTION, priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestReflection()
        {
            StartTest(KEY_REFLECTION);

            ReflectionTestObject obj = new ReflectionTestObject();
            Test(obj);

            EndTest(KEY_REFLECTION);
        }
        [MenuItem(ROOT + KEY_PRIMITIVES, priority = Utility.MENU_ITEM_PRIORITY)]
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
                result += Serializer.Deserialize(data.SerializeToBytes(), obj.GetType(), ref deserializedObject);

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
