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
    [CreateAssetMenu(fileName = "ModPackage.asset", menuName = "Modding/Package", order = Utility.MENU_ITEM_PRIORITY)]
    public class ModPackage : ScriptableObject, IZipFile<ModPackage>
    {
        public IEnumerable<ObjectEntry> ObjectEntries { get { return _objectEntries; } }
        public bool IncludeInBuilds { get { return _includeInBuilds; } }

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
            try
            {
                DoSerialize(file);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloneManager.Clear();
            }
        }
        private void DoSerialize(ZipFile file)
        {
            Manifest manifest = new Manifest();
            Dictionary<string, string> jsonContent = new Dictionary<string, string>();
            Dictionary<string, byte[]> binaryContent = new Dictionary<string, byte[]>();

            foreach (ObjectEntry entry in _objectEntries)
            {
                if (entry.Key != null && entry.Key != "")
                {
                    string id = IDManager.GetID(entry.Object);

                    Manifest.Instance.AddKey(id, entry.Key);
                }

                if (!Serializer.SerializationQueue.HasBeenEnqueued(entry.Object))
                    Serializer.SerializationQueue.Enqueue(entry.Object);
            }

            while (Serializer.SerializationQueue.Count > 0)
            {
                object toSerialize = Serializer.SerializationQueue.Dequeue();

                if(toSerialize is GameObject)
                {
                    toSerialize = CloneManager.GetClone(toSerialize as GameObject);
                }

                Type objectType = toSerialize.GetType();

                string json = Mods.Serialize(toSerialize);

                //We check this after serializing so the converters have a chance to execute,
                //in case they need to add any other objects to the serialization queue
                if (EntryWriter.IsWritable(objectType))
                {
                    EntryWriter writer = EntryWriter.GetWriter(objectType);
                    Manifest.Entry entry = writer.Write(toSerialize);

                    jsonContent.Add(entry.id, json);
                    manifest.AddEntry(entry);
                }
                else
                {
                    Debug.LogWarning("Omitting serializing " + toSerialize + " to json because it doesn't have a writer");
                }
            }

            foreach (BinarySerializer.Entry binaryEntry in Serializer.BinarySerializer.Entries)
            {
                Type type = binaryEntry.instance.GetType();

                if (EntryWriter.IsWritable(type))
                {
                    EntryWriter writer = EntryWriter.GetWriter(type);
                    Manifest.Entry entry = writer.Write(binaryEntry.instance);
                    entry.type = type;

                    binaryContent.Add(entry.id, binaryEntry.data);
                    manifest.AddEntry(entry);
                }
                else
                {
                    Debug.LogWarning("Omitting serializing " + type + " to binary because it doesn't have a writer");
                }
            }

            foreach (Manifest.Entry manifestEntry in manifest.Entries)
            {
                if (file.ContainsEntry(manifestEntry.path))
                {
                    throw new System.ArgumentException("Zip-file already contains " + manifestEntry.path);
                }

                if (jsonContent.ContainsKey(manifestEntry.id))
                {
                    file.AddEntry(manifestEntry.path, jsonContent[manifestEntry.id]);
                }
                else if (binaryContent.ContainsKey(manifestEntry.id))
                {
                    file.AddEntry(manifestEntry.path, binaryContent[manifestEntry.id]);
                }
                else
                {
                    throw new NullReferenceException("No content found for ID " + manifestEntry.id);
                }
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