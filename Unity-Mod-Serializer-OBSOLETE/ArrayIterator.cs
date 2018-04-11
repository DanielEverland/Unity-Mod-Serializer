using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    public class ArrayIterator<T> : IEnumerable<T>
    {
        public ArrayIterator(Array array)
        {
            if (array == null)
                throw new NullReferenceException("Array is null!");

            if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
                throw new ArgumentException("Type mismatch " + typeof(T) + " - " + array.GetType().GetElementType());

            _array = array;
        }

        private readonly Array _array;

        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayEnumerator<T>(_array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
