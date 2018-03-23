using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS
{
    internal static class ObjectContainer
    {
        public static Buffer Instance
        {
            get
            {
                if (_instance == null)
                    Initialize();

                return _instance;
            }
        }
        private static Buffer _instance;

        public static void Initialize()
        {
            _instance = new Buffer();
        }

        public class Buffer
        {
            int CurrentIndex { get { return _data.Count; } }

            Dictionary<string, int> _idIndexes = new Dictionary<string, int>();
            Dictionary<string, int> _keyIndexes = new Dictionary<string, int>();

            List<BufferData> _data = new List<BufferData>();

            public bool ContainsID(string id)
            {
                return _idIndexes.ContainsKey(id);
            }
            public bool ContainsKey(string key)
            {
                return _keyIndexes.ContainsKey(key);
            }
            public int GetIndexFromID(string id)
            {
                return _idIndexes[id];
            }
            public int GetIndexFromKey(string key)
            {
                return _keyIndexes[key];
            }
            public BufferData GetFromID(string id)
            {
                return _data[_idIndexes[id]];
            }
            public BufferData GetFromKey(string key)
            {
                return _data[_keyIndexes[key]];
            }
            public void Add(string content, System.Type type, string id, string key)
            {
                if(!IsNull(id))
                {
                    if(_idIndexes.ContainsKey(id))
                    {
                        Debug.LogError("ID " + id + " already exists!");
                        return;
                    }

                    _idIndexes.Add(id, CurrentIndex);
                }
                if (!IsNull(key))
                {
                    if(_keyIndexes.ContainsKey(key))
                    {
                        Debug.LogError("Key " + key + " already exists!");
                        return;
                    }

                    _keyIndexes.Add(key, CurrentIndex);
                }
                
                BufferData data = new BufferData()
                {
                    obj = Mods.DeserializeString(content, type),
                    id = id,
                    key = key,
                };

                _data.Add(data);
            }
            private bool IsNull(string value)
            {
                if (value == null)
                    return true;

                return value == "" || value == string.Empty;
            }
            public struct BufferData
            {
                public object obj;
                public string id;
                public string key;
            }
        }
    }
}
