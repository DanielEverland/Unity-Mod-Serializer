using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS
{
    public class SerializationQueue<T> : Queue<T>
    {
        public SerializationQueue()
        {
            _enqueuedObjects = new HashSet<string>();
        }

        /// <summary>
        /// The active object is whichever object we're currently writing data to.
        /// The distinction is important when determining whether we should write
        /// out a Data object, or pass it's ID as a reference.
        /// </summary>
        public T ActiveObject { get; set; }

        /// <summary>
        /// Contains the ID's of all objects that have been enqueued
        /// </summary>
        private HashSet<string> _enqueuedObjects;

        public bool HasBeenEnqueued(string id)
        {
            return _enqueuedObjects.Contains(id);
        }
        public new void Enqueue(T obj)
        {
            if (!IDManager.CanGetID(obj.GetType()))
                throw new System.ArgumentException(string.Format("Cannot enqueue object {0} ({1}) since IDManager doesn't permit it", obj, obj.GetType()));

            string id = IDManager.GetID(obj);

            if (HasBeenEnqueued(id))
                throw new System.InvalidOperationException(string.Format("Object {0} ({1}) - ID: {2} has been enqueued before", obj, obj.GetType(), id));
            
            _enqueuedObjects.Add(id);

            base.Enqueue(obj);
        }
        public new T Dequeue()
        {
            T obj = base.Dequeue();

            ActiveObject = obj;

            return obj;
        }
    }
}
