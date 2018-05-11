using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UMS.Reflection;

namespace UMS
{
    [System.Serializable]
    public class ModFile
    {
        private ModFile() { }
        public ModFile(string fileName)
        {
            _fileName = fileName;
            _entries = new Dictionary<string, Entry>();
        }

        public Entry this[string id]
        {
            get
            {
                return _entries[id];
            }
        }

        public string FileName { get { return _fileName; } }
        public IEnumerable<string> IDs { get { return _entries.Keys; } }

        private readonly string _fileName;
        private Dictionary<string, Entry> _entries;
        
        public static ModFile Load(string fullPath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Open))
            {
                UnityEngine.Debug.Log("Deserializing " + fullPath);
                return (ModFile)formatter.Deserialize(fileStream);
            }
        }

        /// <summary>
        /// Saves the mod file to a folder
        /// </summary>
        public void Save(string folderDirectory)
        {
            string fullPath = string.Format(@"{0}\{1}{2}", folderDirectory, _fileName, Utility.MOD_EXTENSION);

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                formatter.Serialize(fileStream, this);
            }

            UnityEngine.Debug.Log("Serialized " + fullPath);
        }

        /// <summary>
        /// Adds data to the mod file
        /// </summary>
        public void Add(string id, Data data, string key = null)
        {
            if (id == string.Empty || id == null)
                throw new System.NullReferenceException("Tried to add object with empty ID - " + data);

            if (_entries.ContainsKey(id))
            {
                throw new System.InvalidOperationException("Object with id " + id + " already exists!");
            }

            Entry entry = new Entry()
            {
                Data = data,
                Key = key,
            };

            _entries.Add(id, entry);
        }

        [System.Serializable]
        public class Entry
        {
            public Data Data;
            public string Key;
        }
    }
}
