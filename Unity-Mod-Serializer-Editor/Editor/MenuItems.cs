using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UMS.Editor
{
    public static class MenuItems
    {
        [MenuItem(EditorUtilities.MENU_ITEM_ROOT + "/Create New Session", priority = Utility.MENU_ITEM_PRIORITY)]
        private static void CreateNewSession()
        {
            Mods.CreateNewSession();
        }
        [MenuItem(EditorUtilities.MENU_ITEM_ROOT + "/Deserialize Desktop", priority = Utility.MENU_ITEM_PRIORITY)]
        private static void DeserializeDesktop()
        {
            string[] desktopFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            IEnumerable<string> modFiles = desktopFiles.Where(x => Path.GetExtension(x) == ".mod");

            if (modFiles.Count() == 0)
                Debug.LogWarning("No mod files on desktop");

            foreach (string path in modFiles)
            {
                Mods.Load(path);
            }
        }
#if DEBUG
        [MenuItem(EditorUtilities.MENU_ITEM_ROOT + "/Testing/Singular Dimensional Array", priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestSingularDimensionalArray()
        {
            CreateNewSession();

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
        [MenuItem(EditorUtilities.MENU_ITEM_ROOT + "/Testing/Multi-Dimensional Array", priority = Utility.MENU_ITEM_PRIORITY)]
        private static void TestMultiDimensionalArray()
        {
            CreateNewSession();

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
#endif
    }
}
