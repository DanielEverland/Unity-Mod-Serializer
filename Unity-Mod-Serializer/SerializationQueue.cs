using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    public class SerializationQueue<T> : Queue<T>
    {
        public SerializationQueue()
        {
            _usedObjects = new HashSet<T>();
        }

        private HashSet<T> _usedObjects;

        public new void Enqueue(T item)
        {
            if(_usedObjects.Contains(item))
                throw new ArgumentException("Object " + item + " has already been added to the queue");

            _usedObjects.Add(item);

            base.Enqueue(item);

        }
        public bool HasBeenEnqueued(T item)
        {
            return _usedObjects.Contains(item);
        }
    }
}
