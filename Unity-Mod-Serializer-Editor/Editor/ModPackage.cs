using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UMS.Zip;
using Ionic.Zip;

namespace UMS.Editor
{
    [Serializable]
    [CreateAssetMenu(fileName = "ModPackage.asset", menuName = "Modding/Package", order = EditorUtilities.MENU_ITEM_PRIORITY)]
    public class ModPackage : ScriptableObject, IZipFile<ModPackage>
    {
        public IEnumerable<ObjectEntry> ObjectEntries { get { return _objectEntries; } }

        public string FileName { get { return string.Format("{0}.{1}", FileNameWithoutExtension, Extension); } }
        public string FileNameWithoutExtension { get { return name; } }
        public string Extension { get { return Utility.MOD_EXTENSION; } }
        
#pragma warning disable
        [SerializeField]
        private List<ObjectEntry> _objectEntries;
#pragma warning restore

        [Ignore]
        private Dictionary<string, string> _keys = new Dictionary<string, string>();

        /// <summary>
        /// Saves a package 
        /// </summary>
        /// <param name="folderPath">Path of the folder</param>
        public void Save(string folderPath)
        {
            ZipSerializer.Create(this, folderPath);
        }

        public void Serialize(ZipFile file)
        {
            Manifest manifest = new Manifest();

            foreach (ObjectEntry entry in _objectEntries)
            {
                Mods.Serializer.AddToQueue(entry.Object);
                _keys.Add(IDManager.GetID(entry.Object), entry.Key);
            }

            Mods.EmptySerializationQueue();

            foreach (Manifest.Entry manifestEntry in Manifest.Instance.Entries)
            {
                string content = Manifest.Instance.GetContent(manifestEntry.id);

                file.AddEntry(manifestEntry.path, content);
            }
                        
            file.AddEntry(Utility.MANIFEST_NAME, Mods.Serialize(manifest));
        }
        
        [Serializable]
        public class ObjectEntry
        {
            public UnityEngine.Object Object { get { return _object; } set { _object = value; } }
            public string Key { get { return _key; } set { _key = value; } }

            [SerializeField]
            private string _key;
            [SerializeField]
            private UnityEngine.Object _object;
        }
    }
}