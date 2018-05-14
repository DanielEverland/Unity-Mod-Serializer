using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UMS.Reflection;
using ProtoBuf;

namespace UMS
{
    /// <summary>
    /// Contains the data we serialize from mod packages
    /// </summary>
    [ProtoContract]
    public class ModFile
    {
        public ModFile(string fileName)
        {
            _fileName = fileName;
            _entries = new Dictionary<string, Entry>();

            _guid = System.Guid.NewGuid();
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
        public System.Guid GUID { get { return _guid; } }

        [ProtoMember(1)]
        private readonly string _fileName;
        [ProtoMember(2)]
        private Dictionary<string, Entry> _entries;
        [ProtoMember(3)]
        private System.Guid _guid;
        
        public static ModFile Load(string fullPath)
        {
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Open))
            {
                UnityEngine.Debug.Log("Deserializing " + fullPath);

                return ProtoBuf.Serializer.Deserialize<ModFile>(fileStream);
            }
        }

        /// <summary>
        /// Saves the mod file to a folder
        /// </summary>
        public void Save(string folderDirectory)
        {
            string fullPath = string.Format(@"{0}\{1}{2}", folderDirectory, _fileName, Utility.MOD_EXTENSION);
            
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                ProtoBuf.Serializer.Serialize(fileStream, this);
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
                ID = id,
            };

            _entries.Add(id, entry);
        }

        [ProtoContract]
        public class Entry
        {
            [ProtoMember(1)]
            public Data Data;
            [ProtoMember(2)]
            public string Key;
            [ProtoMember(3)]
            public string ID;
        }
    }
}
