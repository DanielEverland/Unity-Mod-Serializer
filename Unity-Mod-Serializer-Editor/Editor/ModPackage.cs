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

        private Serializer Serializer { get { return Mods.Serializer; } }
        
#pragma warning disable
        [SerializeField]
        private List<ObjectEntry> _objectEntries;
#pragma warning restore
        
        /// <summary>
        /// Saves a package 
        /// </summary>
        /// <param name="folderPath">Path of the folder</param>
        public void Save(string folderPath)
        {
            Mods.CreateNewSession();

            ZipSerializer.Create(this, folderPath);
        }

        public void Serialize(ZipFile file)
        {
            Manifest manifest = new Manifest();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            Dictionary<string, string> content = new Dictionary<string, string>();

            foreach (ObjectEntry entry in _objectEntries)
            {
                if (entry.Key != null && entry.Key != "")
                {
                    string id = IDManager.GetID(entry.Object);

                    keys.Add(id, entry.Key);
                }
                
                Serializer.SerializationQueue.Enqueue(entry.Object);
            }

            while (Serializer.SerializationQueue.Count > 0)
            {
                object toSerialize = Serializer.SerializationQueue.Dequeue();

                string json = Mods.Serialize(toSerialize);
                string id = IDManager.GetID(toSerialize);
                string key = keys.ContainsKey(id) ? keys[id] : null;

                content.Set(IDManager.GetID(toSerialize), json);

                manifest.AddEntry(new Manifest.Entry(toSerialize, key));
            }

            foreach (Manifest.Entry manifestEntry in manifest.Entries)
            {
                file.AddEntry(manifestEntry.path, content[manifestEntry.id]);
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