using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UMS.Converters
{
    public class CyclicReferenceManager
    {
        // We use the default ReferenceEquals when comparing two objects because
        // custom objects may override equals methods. These overriden equals may
        // treat equals differently; we want to serialize/deserialize the object
        // graph *identically* to how it currently exists.
        class ObjectReferenceEqualityComparator : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }

            public static readonly IEqualityComparer<object> Instance = new ObjectReferenceEqualityComparator();
        }

        private Dictionary<object, string> _objectIds = new Dictionary<object, string>(ObjectReferenceEqualityComparator.Instance);
        private int _depth;

        public void Enter()
        {
            _depth++;
        }

        public void Reset()
        {
            _depth = 0;
        }
        public bool Exit()
        {
            _depth--;

            if (_depth == 0)
            {
                _objectIds = new Dictionary<object, string>(ObjectReferenceEqualityComparator.Instance);
            }

            if (_depth < 0)
            {
                _depth = 0;
                throw new InvalidOperationException("Internal Error - Mismatched Enter/Exit. Please report a bug at https://github.com/jacobdufault/fullserializer/issues with the serialization data.");
            }

            return _depth == 0;
        }

        public object GetReferenceObject(string id)
        {
            if (!ObjectContainer.ContainsObject(id, ObjectContainer.IndexType.ID))
            {
                throw new InvalidOperationException("Internal Deserialization Error - Object " +
                    "definition has not been encountered for object with id=" + id +
                    "; have you reordered or modified the serialized data? If this is an issue " +
                    "with an unmodified Full Serializer implementation and unmodified serialization " +
                    "data, please report an issue with an included test case.");
            }

            return ObjectContainer.GetObjectFromID(id);
        }

        public string GetReferenceId(object item)
        {
            string id;
            if (_objectIds.TryGetValue(item, out id) == false)
            {
                id = IDManager.GetID(item);
                _objectIds[item] = id;
            }
            return id;
        }
    }
}