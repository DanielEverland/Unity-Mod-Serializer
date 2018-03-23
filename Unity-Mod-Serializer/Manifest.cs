using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    [Serializable]
    public class Manifest
    {
        public Manifest()
        {
            _entries = new List<Entry>();
            _keys = new Dictionary<string, string>();
            _objects = new Dictionary<string, object>();
            _content = new Dictionary<string, string>();
            _serilizationQueueManifest = new HashSet<object>();

            SerializationQueue = new Queue<object>();

            Instance = this;
        }

        public static Manifest Instance { get; private set; }
        
        public IEnumerable<Entry> Entries { get { return _entries; } }

        [Ignore]
        public object CurrentlySerializingObject;
        [Ignore]
        public Queue<object> SerializationQueue;


        [Property]
        private List<Entry> _entries;

        [Ignore]
        private Dictionary<string, object> _objects;
        [Ignore]
        private Dictionary<string, string> _content;        
        [Ignore]
        private HashSet<object> _serilizationQueueManifest;
        [Ignore]
        private Dictionary<string, string> _keys;

        public void AddKey(string id, string key)
        {
            _keys.Add(id, key);
        }
        public bool ContainsKey(string id)
        {
            return _keys.ContainsKey(id);
        }
        public string GetKey(string id)
        {
            if (!_keys.ContainsKey(id))
                throw new NullReferenceException("No key for " + id);

            return _keys[id];
        }
        public void AddToQueue(object obj)
        {
            if (_serilizationQueueManifest.Contains(obj))
                throw new ArgumentException(obj + " has already been added to the serialization queue before!");

            _serilizationQueueManifest.Add(obj);
            SerializationQueue.Enqueue(obj);
            Instance.Add(obj);
        }
        public string GetContent(string id)
        {
            if (!_content.ContainsKey(id))
                throw new ArgumentException("ID " + id + " has not been added to manifest!");

            return _content[id];
        }
        public void AddContent(string id, string content)
        {
            if (_content.ContainsKey(id))
                throw new ArgumentException("ID " + id + " has already been serialized!");

            _content.Add(id, content);
        }
        public void UpdateObject(string id, object obj)
        {
            _objects[id] = obj;
        }
        public object GetObject(string id)
        {
            return _objects[id];
        }
        public bool Contains(string id)
        {
            return _entries.Any(x => x.id == id);
        }
        public void Add(object obj)
        {
            _objects.Add(IDManager.GetID(obj), obj);

            Entry entry = new Entry(obj, null);
            _entries.Add(entry);
        }
        
        public Entry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }
        
        [Serializable]
        public struct Entry
        {
            public Entry(object obj, string key)
            {
                this.id = IDManager.GetID(obj);
                this.path = obj.ToString();
                this.type = obj.GetType();
                this.key = key;
            }

            [Property]
            public string id;
            [Property]
            public string path;
            [Property]
            public Type type;
            [Property]
            public string key;
        }
    }
}
