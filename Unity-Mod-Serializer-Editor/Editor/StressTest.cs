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
        private static TestData toSerialize;

        [MenuItem("Modding/Stress Test", priority = 1000)]
        public static void RunTest()
        {
            
        }

        private const int ITERATIONS = 1000;
        
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
