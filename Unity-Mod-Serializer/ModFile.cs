using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UMS.Reflection;
using ProtoBuf;
using UnityEngine;

namespace UMS
{
    /// <summary>
    /// Contains the data we serialize from mod packages
    /// </summary>
    [ProtoContract]
    public class ModFile : IEnumerable<ModFile.Entry>, IEquatable<ModFile>
    {
        public ModFile()
        {
            _entries = new List<Entry>();
        }
        public ModFile(ModPackage package)
        {
            _entries = new List<Entry>();

            if (package.GUID == Guid.Empty)
                throw new System.NullReferenceException("GUID is empty");

            _name = package.name;
            _guid = package.GUID;

            foreach (ModPackage.ObjectEntry entry in package.ObjectEntries)
            {
                Add(entry.Object, entry.Key);
            }
        }

        public Entry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }

        public string Name { get { return _name; } }
        public System.Guid GUID { get { return _guid; } }
        public IEnumerable<Entry> Entries { get { return _entries; } }

        [ProtoMember(1)]
        private readonly string _name;
        [ProtoMember(2)]
        private List<Entry> _entries;
        [ProtoMember(3)]
        private System.Guid _guid;
        
        public static ModFile Load(string fullPath)
        {
            Debugging.Info(DebuggingFlags.Serializer, $"Deserializing {fullPath}");
            
#if DEBUG
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif

            byte[] data = File.ReadAllBytes(fullPath);
            ModFile file = Serializer.Deserialize<ModFile>(data);

            Debugging.Info(DebuggingFlags.Serializer, $"Deserialized {file}");

            file.CreateObjects();

            if(Application.isEditor)
                ObjectHandler.InstantiateAllObjects();
            
#if DEBUG
            Debugging.Info(DebuggingFlags.Serializer, $"Deserialization Elapsed: {stopWatch.Elapsed.Milliseconds}ms");
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
            Debugging.Info(DebuggingFlags.Serializer, $"Serializing {this} to {folderDirectory}");

#if DEBUG
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif

            string fullPath = $@"{folderDirectory}\{_name}{Utility.MOD_EXTENSION}";
            byte[] data = Serializer.Serialize(this);
            
            // Serialzation failed. No need to output any log, the serializer will do that
            if(data == null)
                return;

            File.WriteAllBytes(fullPath, data);
            
#if DEBUG
            Debugging.Info(DebuggingFlags.Serializer, $"Serialization Elapsed: {stopWatch.Elapsed.Milliseconds}ms");
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if(obj is ModFile file)
            {
                return Equals(file);
            }

            return false;
        }
        public bool Equals(ModFile other)
        {
            if (other == null)
                return false;

            return other.GUID == this.GUID;
        }
        public override int GetHashCode()
        {
            return GUID.GetHashCode();
        }
        public override string ToString()
        {
            return $"{Name} ({GUID})";
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
