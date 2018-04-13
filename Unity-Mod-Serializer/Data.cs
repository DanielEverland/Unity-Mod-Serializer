﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS
{
    /// <summary>
    /// Helper class used to add an abstraction layer,
    /// so we don't have to fiddle around with Protobuf in converters
    /// </summary>
    [System.Serializable]
    public sealed class Data
    {
        /// <summary>
        /// Represents the actual value we serialize
        /// </summary>
        private object _value;

        #region Indexers
        public Data this[string index]
        {
            get
            {
                if (!IsDictioanry)
                    throw new System.InvalidOperationException("Tried to index a non-dictionary Data object " + this);

                return AsDictionary[index];
            }
            set
            {
                if (!IsDictioanry)
                    throw new System.InvalidOperationException("Tried to index a non-dictionary Data object " + this);

                AsDictionary.Set(index, value);
            }
        }
        public Data this[int index]
        {
            get
            {
                if (!IsList)
                    throw new System.InvalidOperationException("Tried to index a non-list Data object " + this);

                return AsList[index];
            }
            set
            {
                AsList[index] = value;
            }
        }
        #endregion

        #region Static Definitions
        public static readonly Data True = new Data(true);
        public static readonly Data False = new Data(false);
        public static readonly Data Null = new Data();
        #endregion

        #region Constructors
        public Data()
        {
            _value = null;
        }
        public Data(bool value)
        {
            _value = value;
        }
        public Data(double value)
        {
            _value = value;
        }
        public Data(decimal value)
        {
            _value = value;
        }
        public Data(long value)
        {
            _value = value;
        }
        public Data(ulong value)
        {
            _value = value;
        }
        public Data(string value)
        {
            _value = value;
        }
        public Data(byte[] value)
        {
            _value = value;
        }
        public Data(Dictionary<string, Data> dictionary)
        {
            _value = dictionary;
        }
        public Data(List<Data> list)
        {
            _value = list;
        }
        #endregion

        #region Definition Properties
        public bool IsNull { get { return _value == null; } }
        public bool IsBool { get { return _value is bool; } }
        public bool IsDouble { get { return _value is double; } }
        public bool IsDecimal { get { return _value is decimal; } }
        public bool IsLong { get { return _value is long; } }
        public bool IsULong { get { return _value is ulong; } }
        public bool IsString { get { return _value is string; } }
        public bool IsBytes { get { return _value is byte[]; } }
        public bool IsDictioanry { get { return _value is Dictionary<string, Data>; } }
        public bool IsList { get { return _value is List<Data>; } }
        #endregion

        #region Cast Properties
        public bool AsBool { get { return Cast<bool>(); } }
        public double AsDouble { get { return Cast<double>(); } }
        public decimal AsDecimal { get { return Cast<decimal>(); } }
        public long AsLong { get { return Cast<long>(); } }
        public ulong AsULong { get { return Cast<ulong>(); } }
        public string AsString { get { return Cast<string>(); } }
        public byte[] AsBytes { get { return Cast<byte[]>(); } }
        public Dictionary<string, Data> AsDictionary { get { return Cast<Dictionary<string, Data>>(); } }
        public List<Data> AsList { get { return Cast<List<Data>>(); } }
        #endregion

        #region Internal Helper Methods
        private T Cast<T>()
        {
            try
            {
                return (T)_value;
            }
            catch (System.InvalidCastException)
            {
                throw new System.InvalidCastException("Issue casting data value " + this + " to type " + typeof(T));
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            if (_value == null)
                return "null";

            return _value.ToString();
        }
        #endregion
    }
}
