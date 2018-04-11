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
        public IEnumerable<ObjectEntry> ObjectEntries { get { return _objectEntries; } }
        public bool IncludeInBuilds { get { return _includeInBuilds; } }

        public string FileName { get { return string.Format("{0}{1}", FileNameWithoutExtension, Extension); } }
        public string FileNameWithoutExtension { get { return name; } }
        public string Extension { get { return Utility.MOD_EXTENSION; } }
        
#pragma warning disable
        [SerializeField]
        private List<ObjectEntry> _objectEntries;
        [SerializeField]
        private bool _includeInBuilds = true;
#pragma warning restore
        
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
