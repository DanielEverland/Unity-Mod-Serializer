using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UMS.Zip;
using UMS.EntryWriters;
using Ionic.Zip;

namespace UMS.Editor
{
    [Serializable]
    [CreateAssetMenu(fileName = "ModPackage.asset", menuName = "Modding/Package", order = EditorUtilities.MENU_ITEM_PRIORITY)]
    public class ModPackage : ScriptableObject, IZipFile<ModPackage>
    {
        public IEnumerable<ObjectEntry> ObjectEntries { get { return _objectEntries; } }
        public bool IncludeInBuilds { get { return _includeInBuilds; }  set { _includeInBuilds = value; } }

        public string FileName { get { return string.Format("{0}.{1}", FileNameWithoutExtension, Extension); } }
        public string FileNameWithoutExtension { get { return name; } }
        public string Extension { get { return Utility.MOD_EXTENSION; } }

        private Serializer Serializer { get { return Mods.Serializer; } }
        
#pragma warning disable
        [SerializeField]
        private List<ObjectEntry> _objectEntries;
        [SerializeField]
        private bool _includeInBuilds = true;
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
            Dictionary<string, string> content = new Dictionary<string, string>();

            foreach (ObjectEntry entry in _objectEntries)
            {
                if (entry.Key != null && entry.Key != "")
                {
                    string id = IDManager.GetID(entry.Object);

                    Manifest.Instance.AddKey(id, entry.Key);
                }
                
                if(!Serializer.SerializationQueue.HasBeenEnqueued(entry.Object))
                    Serializer.SerializationQueue.Enqueue(entry.Object);
            }

            while (Serializer.SerializationQueue.Count > 0)
            {
                object toSerialize = Serializer.SerializationQueue.Dequeue();
                Type objectType = toSerialize.GetType();

                string json = Mods.Serialize(toSerialize);

                //We check this after serializing so the converters have a chance to execute,
                //in case they need to add any other objects to the serialization queue
                if (EntryWriter.IsWritable(objectType))
                {
                    EntryWriter writer = EntryWriter.GetWriter(objectType);
                    Manifest.Entry entry = writer.Write(toSerialize);

                    content.Add(entry.id, json);
                    manifest.AddEntry(entry);
                }
                else
                {
                    Debug.LogWarning("Omitting serializing " + toSerialize + " because it doesn't have a writer");
                }
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