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

            Instance = this;
        }

        public static Manifest Instance { get; private set; }
        
        public IEnumerable<Entry> Entries { get { return _entries; } }
        
        [Property]
        private List<Entry> _entries;
                
        public Entry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }

        public void AddEntry(Entry entry)
        {
            _entries.Add(entry);
        }
        
        [Serializable]
        public struct Entry
        {
            public Entry(object obj, string key)
            {
                this.id = IDManager.GetID(obj);
                this.path = obj.ToString();
                this.key = key;
            }

            [Property]
            public string id;
            [Property]
            public string path;
            [Property]
            public string key;
        }
    }
}
