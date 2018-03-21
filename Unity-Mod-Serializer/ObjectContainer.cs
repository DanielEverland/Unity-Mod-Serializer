using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Dictionary<string, int> _guidIndexes = new Dictionary<string, int>();
            Dictionary<string, int> _keyIndexes = new Dictionary<string, int>();

            List<BufferData> _data = new List<BufferData>();

            public bool ContainsGUID(string guid)
            {
                return _guidIndexes.ContainsKey(guid);
            }
            public bool ContainsKey(string key)
            {
                return _keyIndexes.ContainsKey(key);
            }
            public int GetIndexFromGUID(string guid)
            {
                return _guidIndexes[guid];
            }
            public int GetIndexFromKey(string key)
            {
                return _keyIndexes[key];
            }
            public BufferData GetFromGUID(string guid)
            {
                return _data[_guidIndexes[guid]];
            }
            public BufferData GetFromKey(string key)
            {
                return _data[_keyIndexes[key]];
            }
            public void Add(object obj, string guid, string key)
            {
                BufferData data = new BufferData()
                {
                    obj = obj,
                    guid = guid,
                    key = key,
                };

                if(!IsNull(guid))
                {
                    if(_guidIndexes.ContainsKey(guid))
                    {
                        throw new System.ArgumentException("GUID " + guid + " already exists!");
                    }

                    _guidIndexes.Add(guid, CurrentIndex);
                }
                if (!IsNull(key))
                {
                    if(_keyIndexes.ContainsKey(key))
                    {
                        throw new System.ArgumentException("Key " + key + " already exists!");
                    }

                    _keyIndexes.Add(key, CurrentIndex);
                }

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
                public string guid;
                public string key;
            }
        }
    }
}
