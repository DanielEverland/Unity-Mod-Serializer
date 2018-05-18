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
    public class ModFile : IEnumerable<ModFile.Entry>
    {
        public ModFile()
        {
            _entries = new List<Entry>();
        }
        public ModFile(string fileName)
        {
            _fileName = fileName;
            _entries = new List<Entry>();

            _guid = System.Guid.NewGuid();
        }

        public Entry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }

        public string FileName { get { return _fileName; } }
        public System.Guid GUID { get { return _guid; } }

        [ProtoMember(1)]
        private readonly string _fileName;
        [ProtoMember(2)]
        private List<Entry> _entries;
        [ProtoMember(4)]
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
        
        public void Add(Entry entry)
        {
            _entries.Add(entry);
        }
        public void Add<T>(T obj, string key = null) where T : UnityEngine.Object
        {
            Entry entry = new Entry()
            {
                Data = Serializer.Serialize(obj),
                Key = key,
                Type = obj.GetType(),
            };

            _entries.Add(entry);
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [ProtoContract]
        public class Entry
        {
            [ProtoMember(1)]
            public byte[] Data;
            [ProtoMember(2)]
            public string Key;
            [ProtoMember(3)]
            public System.Type Type;
        }
    }
}
