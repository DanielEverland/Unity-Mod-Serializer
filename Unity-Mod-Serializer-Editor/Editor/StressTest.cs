using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace UMS.Editor
{
    /// <summary>
    /// Used to test the serialization speed and size
    /// </summary>
    public static class StressTest
    {
        [MenuItem("Modding/Stress Test", priority = 1000)]
        public static void RunTest()
        {
            TestData data = GetNewData();

            long byteLength = 0;
            Benchmarker.Profile("Serialization", ITERATIONS, () =>
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, data);
                    byteLength += stream.ToArray().Length;
                }                
            });

            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, data);

                byte[] dataToDeserialize = stream.ToArray();

                Benchmarker.Profile("Deserialization", ITERATIONS, () =>
                {
                    ProtoBuf.Serializer.Deserialize<TestData>(new MemoryStream(dataToDeserialize));
                });
            }

            UnityEngine.Debug.Log($"Serialized size is in total: {byteLength.ToString("#,##")} bytes, average: {((long)(double)byteLength / ITERATIONS).ToString("#,##")} bytes");
        }

        private const int ITERATIONS = 10000;
        
        [ProtoContract]
        public class TestData
        {
            [ProtoMember(1)]
            public bool? _boolValue;
        }

        private static TestData GetNewData()
        {
            return new TestData()
            {
                _boolValue = true,
            };
        }
    }
}
