using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UMS
{
    /// <summary>
    /// The actual type that a JsonData instance can store.
    /// </summary>
    public enum DataType
    {
        List,
        Array,
        Object,
        Double,
        Int64,
        Boolean,
        String,
        Null
    }

    /// <summary>
    /// A union type that stores a serialized value. The stored type can be one
    /// of six different
    /// types: null, boolean, double, Int64, string, Dictionary, or List.
    /// </summary>
    public sealed class Data
    {
        /// <summary>
        /// The raw value that this serialized data stores. It can be one of six
        /// different types; a boolean, a double, Int64, a string, a Dictionary,
        /// or a List.
        /// </summary>
        private object _value;

        public Data this[string index]
        {
            get
            {
                if (_dictionary == null)
                    CreateInternalDictionary();

                if(!_dictionary.ContainsKey(index))
                {
                    UnityEngine.Debug.LogError(@"Data doesn't contain key """ + index + @"""");
                    return null;
                }

                return _dictionary[index];
            }
        }

        [Ignore]
        private Dictionary<string, Data> _dictionary;
        private void CreateInternalDictionary()
        {
            try
            {
                _dictionary = Cast<Dictionary<string, Data>>();
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError("Issue creating internal dictionary for " + _value);
                throw;
            }
        }

        #region Public Functions
        /// <summary>
        /// Helper method for assigning values from a
        /// multidimensional array.
        /// </summary>
        public void SetValue(Data data, int[] indexes)
        {
            if (!IsArray)
                throw new ArgumentException("Data isn't array!");

            AsArray.SetValue(data, indexes);
        }
        #endregion 

        #region Constructors
        /// <summary>
        /// Creates a Data instance that holds null.
        /// </summary>
        public Data()
        {
            _value = null;
        }

        /// <summary>
        /// Creates a Data instance that holds a boolean.
        /// </summary>
        public Data(bool boolean)
        {
            _value = boolean;
        }

        /// <summary>
        /// Creates a Data instance that holds a double.
        /// </summary>
        public Data(double f)
        {
            _value = f;
        }

        /// <summary>
        /// Creates a new Data instance that holds an integer.
        /// </summary>
        public Data(Int64 i)
        {
            _value = i;
        }

        /// <summary>
        /// Creates a Data instance that holds a string.
        /// </summary>
        public Data(string str)
        {
            _value = str;
        }

        /// <summary>
        /// Creates a Data instance that holds a dictionary of values.
        /// </summary>
        public Data(Dictionary<string, Data> dict)
        {
            _value = dict;
        }

        /// <summary>
        /// Creates a Data instance that holds a list of values.
        /// </summary>
        public Data(List<Data> list)
        {
            _value = list;
        }

        /// <summary>
        /// Creates a Data instance that holds an array of values.
        /// </summary>
        public Data(Array array)
        {
            if (array.GetType().GetElementType() != typeof(Data))
                throw new ArgumentException("Cannot create a data instance using an array that doesn't contain data types. Element type is " + array.GetType().GetElementType());

            _value = array;
        }

        /// <summary>
        /// Helper method to create a Data instance that holds a dictionary.
        /// </summary>
        public static Data CreateDictionary()
        {
            return new Data(new Dictionary<string, Data>(
                GlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Helper method to create a Data instance that holds a list.
        /// </summary>
        public static Data CreateList()
        {
            return new Data(new List<Data>());
        }

        /// <summary>
        /// Helper method to create a Data instance that holds a list with the
        /// initial capacity.
        /// </summary>
        public static Data CreateList(int capacity)
        {
            return new Data(new List<Data>(capacity));
        }

        /// <summary>
        /// Helper method to create a Data instance that hold an array
        /// </summary>
        public static Data CreateArray(int[] lengths)
        {
            return new Data(Array.CreateInstance(typeof(Data), lengths));
        }
        
        public readonly static Data True = new Data(true);
        public readonly static Data False = new Data(false);
        public readonly static Data Null = new Data();
        #endregion Constructors

        #region Internal Helper Methods
        /// <summary>
        /// Transforms the internal Data instance into a dictionary.
        /// </summary>
        internal void BecomeDictionary()
        {
            _value = new Dictionary<string, Data>();
        }

        /// <summary>
        /// Returns a shallow clone of this data instance.
        /// </summary>
        internal Data Clone()
        {
            var clone = new Data();
            clone._value = _value;
            return clone;
        }
        #endregion Internal Helper Methods

        #region Casting Predicates
        public DataType Type
        {
            get
            {
                if (_value == null) return DataType.Null;
                if (_value is double) return DataType.Double;
                if (_value is Int64) return DataType.Int64;
                if (_value is bool) return DataType.Boolean;
                if (_value is string) return DataType.String;
                if (_value is Dictionary<string, Data>) return DataType.Object;
                if (_value is Array) return DataType.Array;
                if (_value is List<Data>) return DataType.List;

                throw new InvalidOperationException("unknown JSON data type");
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to null.
        /// </summary>
        public bool IsNull
        {
            get
            {
                return _value == null;
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to a double.
        /// </summary>
        public bool IsDouble
        {
            get
            {
                return _value is double;
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to an Int64.
        /// </summary>
        public bool IsInt64
        {
            get
            {
                return _value is Int64;
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to a boolean.
        /// </summary>
        public bool IsBool
        {
            get
            {
                return _value is bool;
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to a string.
        /// </summary>
        public bool IsString
        {
            get
            {
                return _value is string;
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to a Dictionary.
        /// </summary>
        public bool IsDictionary
        {
            get
            {
                return _value is Dictionary<string, Data>;
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to a List.
        /// </summary>
        public bool IsList
        {
            get
            {
                return _value is List<Data>;
            }
        }

        /// <summary>
        /// Returns true if this Data instance maps back to an array.
        /// </summary>
        public bool IsArray
        {
            get
            {
                return _value is Array;
            }
        }
        #endregion Casting Predicates

        #region Casts
        /// <summary>
        /// Casts this Data to a double. Throws an exception if it is not a
        /// double.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double AsDouble
        {
            get
            {
                return Cast<double>();
            }
        }

        /// <summary>
        /// Casts this Data to an Int64. Throws an exception if it is not an
        /// Int64.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Int64 AsInt64
        {
            get
            {
                return Cast<Int64>();
            }
        }

        /// <summary>
        /// Casts this Data to a boolean. Throws an exception if it is not a
        /// boolean.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AsBool
        {
            get
            {
                return Cast<bool>();
            }
        }

        /// <summary>
        /// Casts this Data to a string. Throws an exception if it is not a
        /// string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string AsString
        {
            get
            {
                return Cast<string>();
            }
        }

        /// <summary>
        /// Casts this Data to a Dictionary. Throws an exception if it is not a
        /// Dictionary.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Dictionary<string, Data> AsDictionary
        {
            get
            {
                return Cast<Dictionary<string, Data>>();
            }
        }

        /// <summary>
        /// Casts this Data to a List. Throws an exception if it is not a List.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<Data> AsList
        {
            get
            {
                if (IsArray)
                {
                    if (AsArray.Rank != 1)
                        throw new InvalidCastException("Array is multi-dimensional, so we can't cast to List");
                }

                return Cast<List<Data>>();
            }
        }

        /// <summary>
        /// Casts this Data to an Array. Throws an exception if it is not an Array.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Array AsArray
        {
            get
            {
                return Cast<Array>();
            }
        }

        /// <summary>
        /// Internal helper method to cast the underlying storage to the given
        /// type or throw a pretty printed exception on failure.
        /// </summary>
        private T Cast<T>()
        {
            if (_value is T)
            {
                return (T)_value;
            }

            throw new InvalidCastException("Unable to cast <" + this + "> (with type = " +
                _value.GetType() + ") to type " + typeof(T));
        }
        #endregion Casts

        #region ToString Implementation
        public override string ToString()
        {
            return JsonPrinter.CompressedJson(this);
        }
        #endregion ToString Implementation

        #region Equality Comparisons
        /// <summary>
        /// Determines whether the specified object is equal to the current
        /// object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Data);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current
        /// object.
        /// </summary>
        public bool Equals(Data other)
        {
            if (other == null || Type != other.Type)
            {
                return false;
            }

            switch (Type)
            {
                case DataType.Null:
                    return true;

                case DataType.Double:
                    return AsDouble == other.AsDouble || Math.Abs(AsDouble - other.AsDouble) < double.Epsilon;

                case DataType.Int64:
                    return AsInt64 == other.AsInt64;

                case DataType.Boolean:
                    return AsBool == other.AsBool;

                case DataType.String:
                    return AsString == other.AsString;

                case DataType.List:
                    {
                        var thisList = AsList;
                        var otherList = other.AsList;

                        if (thisList.Count != otherList.Count) return false;

                        for (int i = 0; i < thisList.Count; ++i)
                        {
                            if (thisList[i].Equals(otherList[i]) == false)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                case DataType.Array:
                    {
                        Array thisArray = AsArray;
                        Array otherArray = other.AsArray;

                        if (thisArray.Length != otherArray.Length) return false;

                        for (int i = 0; i < thisArray.Length; ++i)
                        {
                            if (thisArray.GetValue(i).Equals(otherArray.GetValue(i)) == false)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                case DataType.Object:
                    {
                        var thisDict = AsDictionary;
                        var otherDict = other.AsDictionary;

                        if (thisDict.Count != otherDict.Count) return false;

                        foreach (string key in thisDict.Keys)
                        {
                            if (otherDict.ContainsKey(key) == false)
                            {
                                return false;
                            }

                            if (thisDict[key].Equals(otherDict[key]) == false)
                            {
                                return false;
                            }
                        }

                        return true;
                    }                    
            }

            throw new Exception("Unknown data type");
        }

        /// <summary>
        /// Returns true iff a == b.
        /// </summary>
        public static bool operator ==(Data a, Data b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            if (a.IsDouble && b.IsDouble)
            {
                return Math.Abs(a.AsDouble - b.AsDouble) < double.Epsilon;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Returns true iff a != b.
        /// </summary>
        public static bool operator !=(Data a, Data b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms
        /// and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
        #endregion Equality Comparisons
    }
}