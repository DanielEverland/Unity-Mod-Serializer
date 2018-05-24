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
            Debugging.Verbose(DebuggingFlags.Serializer, $"Deserializing {fullPath}");
            
#if DEBUG
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif

            byte[] data = File.ReadAllBytes(fullPath);
            ModFile file = Serializer.Deserialize<ModFile>(data);

#if DEBUG
            Debugging.Info(DebuggingFlags.Serializer, $"Deserialization Elapsed: {stopWatch.Elapsed}");
#endif

            return file;
        }

        private void CreateObjects()
        {
            foreach (Entry entry in _entries)
            {
                ObjectHandler.AddObject(entry.Object, entry.Key);
            }
        }

        /// <summary>
        /// Saves the mod file to a folder
        /// </summary>
        public void Save(string folderDirectory)
        {
#if DEBUG
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif

            string fullPath = $@"{folderDirectory}\{_fileName}{Utility.MOD_EXTENSION}";
            byte[] data = Serializer.Serialize(this);
            
            // Serialzation failed. No need to output any log, the serializer will do that
            if(data == null)
                return;

            File.WriteAllBytes(fullPath, data);
            
            Debugging.Verbose(DebuggingFlags.Serializer, $"Serialized {fullPath}");

#if DEBUG
            Debugging.Info(DebuggingFlags.Serializer, $"Serialization Elapsed: {stopWatch.Elapsed}");
#endif
        }

        public void Add(Entry entry)
        {
            _entries.Add(entry);
        }
        public void Add<T>(T obj, string key = null) where T : UnityEngine.Object
        {
            Entry entry = new Entry()
            {
                Object = obj,
                Key = key,
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
            [ProtoMember(1, DynamicType = true)]
            public UnityEngine.Object Object;
            [ProtoMember(2)]
            public string Key;
        }
    }
}
