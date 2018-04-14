using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace UMS
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ModPackage.asset", menuName = "Modding/Package", order = Utility.MENU_ITEM_PRIORITY)]
    public class ModPackage : ScriptableObject
    {
        public ModPackage()
        {
            _objectEntries = new List<ObjectEntry>();
        }

        public string FileName { get { return name + Utility.MOD_EXTENSION; } }
        public IEnumerable<ObjectEntry> ObjectEntries { get { return _objectEntries; } }
        public bool IncludeInBuilds { get { return _includeInBuilds; } }
                
#pragma warning disable
        [SerializeField]
        private List<ObjectEntry> _objectEntries;
        [SerializeField]
        private bool _includeInBuilds = true;
#pragma warning restore

        public static void Load(string fullPath)
        {
            ModFile file = ModFile.Load(fullPath);

            foreach (string id in file.IDs)
            {
                ModFile.Entry entry = file[id];

                object deserialized = null;
                MetaData.GetType(entry.Data, out System.Type type);

                Serializer.Deserialize(entry.Data, type, ref deserialized).AssertWithoutWarnings();
            }
        }
        public void SaveToDesktop()
        {
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            Save(desktopPath);
        }
        public void Save(string folderPath)
        {
            ModFile file = CreateFile();

            file.Save(folderPath);
        }
        private ModFile CreateFile()
        {
            ModFile file = new ModFile(name);

            foreach (ObjectEntry entry in _objectEntries)
            {
                Result result = Serializer.Serialize(entry.Object, out Data data);

                if (result.Succeeded)
                {
                    file.Add(IDManager.GetID(entry.Object), entry.Object.name, data, entry.Key);
                }
            }

            return file;
        }
        
        [System.Serializable]
        public class ObjectEntry
        {
            public Object Object { get { return _object; } set { _object = value; } }
            public string Key { get { return _key; } set { _key = value; } }

            [SerializeField]
            private string _key;
            [SerializeField]
            private Object _object;
        }
    }
}
