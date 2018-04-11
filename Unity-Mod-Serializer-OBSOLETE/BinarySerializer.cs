using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Converters;

namespace UMS
{
    /// <summary>
    /// Handles serialization of binary objects into a mod package
    /// </summary>
    public class BinarySerializer
    {
        public BinarySerializer()
        {
            _entries = new List<Entry>();
            _convertes = new List<IBinaryConverter>();
            _cachedConverters = new Dictionary<Type, IBinaryConverter>();
        }

        public IEnumerable<Entry> Entries { get { return _entries; } }

        private List<Entry> _entries;
        private List<IBinaryConverter> _convertes;

        /// <summary>
        /// When a new type asks for a binary converter, we serialize
        /// it so we don't have to poll the inheritance tree again
        /// </summary>
        private Dictionary<Type, IBinaryConverter> _cachedConverters;

        public bool CanConvert(Type type)
        {
            return GetConverter(type) != null;
        }
        public IBinaryConverter GetConverter(Type type)
        {
            if (_cachedConverters.ContainsKey(type))
                return _cachedConverters[type];

            IBinaryConverter converter = TypeInheritanceTree.GetClosestType(_convertes, type, x => x.ModelType);

            if(converter != null)
                _cachedConverters.Add(type, converter);

            return converter;
        }
        public void AddConverter(IBinaryConverter converter)
        {
            converter.Serializer = this;

            _convertes.Add(converter);
        }
        public void AddObject(object instance, byte[] data)
        {
            _entries.Add(new Entry(instance, data));
        }

        public struct Entry
        {
            public Entry(object instance, byte[] data)
            {
                this.data = data;
                this.instance = instance;
            }
            
            public object instance;
            public byte[] data;
        }
    }
}
