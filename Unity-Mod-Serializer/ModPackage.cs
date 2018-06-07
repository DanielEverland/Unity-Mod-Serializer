using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ModPackage.asset", menuName = "Modding/Package", order = Utility.MENU_ITEM_PRIORITY)]
    public partial class ModPackage : ScriptableObject
    {
        public ModPackage()
        {
            _objectEntries = new List<ObjectEntry>();
        }

        public string FileName { get { return name + Utility.MOD_EXTENSION; } }
        public IEnumerable<ObjectEntry> ObjectEntries { get { return _objectEntries; } }
        public bool IncludeInBuilds { get { return _includeInBuilds; } }
        public System.Guid GUID { get { return guid; } }
                        
#pragma warning disable
        [SerializeField]
        private List<ObjectEntry> _objectEntries;
        [SerializeField]
        private bool _includeInBuilds = true;
        [SerializeField]
        private SerializableGUID guid;
#pragma warning restore
        
        public static void Load(string fullPath)
        {
            ModFile file = ModFile.LoadFromFile(fullPath);
            ObjectHandler.InstantiateAllObjects();
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
        public ModFile CreateFile()
        {
            return new ModFile(this);
        }
        [ContextMenu("Assign New GUID")]
        private void AssignNewGUID()
        {
            guid = System.Guid.NewGuid();
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
