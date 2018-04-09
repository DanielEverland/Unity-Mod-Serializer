using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UMS.Editor
{
#if DEBUG
    public static class TestingMenuItems
    {
        private const string ROOT = EditorUtilities.MENU_ITEM_ROOT + "/Testing/";
        
        [MenuItem(ROOT + "Lists", priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestLists()
        {
            MenuItems.CreateNewSession();


            Debug.Log("Serializing primitives");
            List<float> floatList = new List<float>()
            {
                69,
                420,
                123,
                76,
                7,
            };
            Debug.Log(Mods.Serialize(floatList));


            Debug.Log("Serializing objects");
            List<TestObject> objectList = new List<TestObject>()
            {
                new TestObject() { Name = "First",  Number = 69 },
                new TestObject() { Name = "Second", Number = 420 },
                new TestObject() { Name = "Third",  Number = 123 },
                new TestObject() { Name = "Fourth", Number = 76 },
                new TestObject() { Name = "Fifth",  Number = 7 },
            };
            Debug.Log(Mods.Serialize(objectList));


            Debug.Log("Serializing single-dimensional primitive arrays");
            List<float[]> singleDimensionalFloatArrays = new List<float[]>()
            {
                new float[5]
                {
                    69,
                    420,
                    123,
                    76,
                    7,
                },

                new float[2]
                {
                    153,
                    6,
                },

                new float[4]
                {
                    69,
                    43,
                    73,
                    2,
                },
            };
            Debug.Log(Mods.Serialize(singleDimensionalFloatArrays));

            Debug.Log("Serializing multi-dimensional uniform primitive arrays");
            List<float[,,]> multiDimensionalUniformPrimitiveArrays = new List<float[,,]>()
            {
                new float[3, 2, 3]
                {
                    {
                        { 125, 425, 653 }, { 420, 242, 690 },
                    },
                    {
                        { 334, 589, 756 }, { 742, 513, 863 },
                    },
                    {
                        { 541, 152, 234 }, { 434, 542, 615 },
                    },
                },

                new float[1, 2, 3]
                {
                    {
                        { 513, 425, 756 }, { 690, 615, 742 },
                    },
                },

                new float[4, 1, 2]
                {
                    {
                        { 242, 653 },
                    },
                    {
                        { 863, 589 },
                    },
                    {
                        { 513, 615 },
                    },
                    {
                        { 425, 152 },
                    },
                },
            };
            Mods.Serialize(multiDimensionalUniformPrimitiveArrays);

            Debug.Log("Serializing multi-dimensional jagged primtive arrays");
            List<float[][]> multiDimensionalJaggedPrimitiveArrays = new List<float[][]>()
            {
                new float[3][]
                {
                    new float[3] { 125, 425, 653 },
                    new float[1] { 420, },
                    new float[2] { 690, 242 },
                },

                new float[1][]
                {
                    new float[3] { 756, 615, 863 },
                },

                new float[2][]
                {
                    new float[3] { 756, 425, 125 },
                    new float[1] { 615, },
                },
            };
            Debug.Log(Mods.Serialize(multiDimensionalJaggedPrimitiveArrays));

            Debug.Log("Serializing multi-dimensional jagged object arrays");
            List<TestObject[][]> multiDimensionalJaggedObjectArrays = new List<TestObject[][]>()
            {
                new TestObject[3][]
                {
                    new TestObject[3] { new TestObject("0,0", 513), new TestObject("0,1", 615), new TestObject("0,2", 420) },
                    new TestObject[1] { new TestObject("1,0", 742), },
                    new TestObject[2] { new TestObject("2,0", 653), new TestObject("2,1", 242) },
                },

                new TestObject[1][]
                {
                    new TestObject[2] { new TestObject("0,0", 690), new TestObject("0,1", 242) },
                },

                new TestObject[2][]
                {
                    new TestObject[1] { new TestObject("0,0", 242), },
                    new TestObject[2] { new TestObject("1,0", 690), new TestObject("1,1", 589) },
                },
            };
            Debug.Log(Mods.Serialize(multiDimensionalJaggedObjectArrays));

            Debug.Log("Finished testing lists");
        }
        [MenuItem(ROOT + "Singular Dimensional Array", priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestSingularDimensionalArray()
        {
            MenuItems.CreateNewSession();

            Debug.Log("Serializing primitives");

            float[] floatArray = new float[5]
            {
                69,
                420,
                123,
                76,
                7,
            };

            Debug.Log(Mods.Serialize(floatArray));

            Debug.Log("Serializing objects");

            TestObject[] objectArray = new TestObject[5]
            {
                new TestObject() { Name = "First",  Number = 69 },
                new TestObject() { Name = "Second", Number = 420 },
                new TestObject() { Name = "Third",  Number = 123 },
                new TestObject() { Name = "Fourth", Number = 76 },
                new TestObject() { Name = "Fifth",  Number = 7 },
            };

            Debug.Log(Mods.Serialize(objectArray));

            Debug.Log("Finished testing");
        }
        [MenuItem(ROOT + "Multi-Dimensional Array", priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestMultiDimensionalArray()
        {
            MenuItems.CreateNewSession();

            Debug.Log("Serializing primitives");

            float[,,] threeDimensionalFloatArray = new float[3, 2, 3]
            {
                {
                    { 125, 425, 653 }, { 420, 242, 690 },
                },
                {
                    { 334, 589, 756 }, { 742, 513, 863 },
                },
                {
                    { 541, 152, 234 }, { 434, 542, 615 },
                },
            };

            Debug.Log(Mods.Serialize(threeDimensionalFloatArray));

            Debug.Log("Serializing uniform object array");

            TestObject[,,] threeDimensionalObjectArray = new TestObject[3, 2, 1]
            {
                {
                    { new TestObject("0,0", 125) }, { new TestObject("0,1", 425) },
                },
                {
                    { new TestObject("1,0", 653) }, { new TestObject("1,1", 420) },
                },
                {
                    { new TestObject("2,0", 690) }, { new TestObject("2,1", 242) },
                },
            };

            Debug.Log(Mods.Serialize(threeDimensionalObjectArray));

            Debug.Log("Serializing jagged object array");

            TestObject[][] jaggedThreeDimensionalObjectArray = new TestObject[3][]
            {
                new TestObject[3] { new TestObject("0,0", 125), new TestObject("0,1", 425), new TestObject("0,2", 653) },
                new TestObject[1] { new TestObject("1,0", 420), },
                new TestObject[2] { new TestObject("2,0", 690), new TestObject("2,1", 242) },
            };

            Debug.Log(Mods.Serialize(jaggedThreeDimensionalObjectArray));

            Debug.Log("Finished testing multi-dimensional arrays");
        }

        private struct TestObject
        {
            public TestObject(string name, float number)
            {
                Name = name;
                Number = number;
            }

            public string Name;
            public float Number;
        }
    }
#endif
}
