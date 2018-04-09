using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    /// <summary>
    /// Implements an iterator that correctly traverses multi-dimensional arrays
    /// </summary>
    /// <typeparam name="T">Element type of the array to iterate across</typeparam>
    public class ArrayEnumerator<T> : IEnumerator<T>
    {
        public ArrayEnumerator(Array array)
        {
            if (array == null)
                throw new NullReferenceException("Array is null!");

            if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
                throw new ArgumentException("Type mismatch " + typeof(T) + " - " + array.GetType().GetElementType());

            _array = array;
            _lengths = array.GetLengths();
            
            Reset();
        }
        
        object IEnumerator.Current { get { return Current; } }
        public T Current
        {
            get
            {
                try
                {
                    return (T)_array.GetValue(_indexes);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException(_indexes.CollectionToString());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private readonly Array _array;

        /// <summary>
        /// The length of each dimension in the array
        /// </summary>
        private readonly int[] _lengths;

        /// <summary>
        /// The index of every dimension in the array
        /// </summary>
        private int[] _indexes;

        /// <summary>
        /// Defines which dimension we're currently iterating over
        /// </summary>
        private int _dimension;

        public bool MoveNext()
        {
            _dimension = _array.Rank - 1;
            while (_indexes[_dimension] + 1 >= _lengths[_dimension])
            {
                _indexes[_dimension] = 0;
                _dimension--;

                //Escape, we've iterated over all elements
                if (_dimension < 0)
                    return false;
            }

            _indexes[_dimension]++;
            
            //Continue iteration
            return true;
        }
        public void Reset()
        {
            _indexes = new int[_array.Rank];

            //As per the MSDN IEnumerator<T> implementation page, we have to assign the initial value to -1.
            //In our case that means the last index must be -1
            _indexes[_array.Rank - 1] = -1;
        }
        public void Dispose()
        {

        }
    }
}
