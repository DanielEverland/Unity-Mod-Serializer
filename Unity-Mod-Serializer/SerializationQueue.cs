﻿using System;
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
            _usedObjects = new HashSet<string>();
        }

        private HashSet<string> _usedObjects;

        public new void Enqueue(T item)
        {
            string id = IDManager.GetID(item);

            if(_usedObjects.Contains(id))
                throw new ArgumentException("Object " + item + " has already been added to the queue");

            _usedObjects.Add(id);

            base.Enqueue(item);

        }
        public bool HasBeenEnqueued(T item)
        {
            return _usedObjects.Contains(IDManager.GetID(item));
        }
    }
}
