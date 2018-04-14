using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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

        public Result Add(Data data)
        {
            if (!IsList)
                return Result.Error("Type mismatch. Expected List", this);

            AsList.Add(data);

            return Result.Success;
        }

        #region Indexers
        public Data this[string index]
        {
            get
            {
                if (!IsDictionary)
                    throw new System.InvalidOperationException("Tried to index a non-dictionary Data object " + this);

                return AsDictionary[index];
            }
            set
            {
                if (!IsDictionary)
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
        public bool IsDictionary { get { return _value is Dictionary<string, Data>; } }
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
        public T Cast<T>()
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
        static int indent = 0;
        public override string ToString()
        {
            if (_value == null)
                return "null";

            if (IsBytes)
            {
                return "byte[]";
            }
            else if(IsDictionary || IsList)
            {
                using (StringWriter writer = new StringWriter())
                {
                    if (IsDictionary)
                    {
                        WriteDictionary(writer);
                    }
                    else if(IsList)
                    {
                        WriteList(writer);
                    }

                    return writer.ToString();
                }
            }
            else
            {
                return _value.ToString();
            }
        }
        private void Indent(StringWriter writer)
        {
            for (int i = 0; i < indent; i++)
            {
                writer.Write("    ");
            }
        }
        private void WriteDictionary(StringWriter writer)
        {
            writer.WriteLine();
            Indent(writer);
            writer.Write("{");

            indent++;

            foreach (KeyValuePair<string, Data> item in AsDictionary)
            {
                writer.WriteLine();
                Indent(writer);
                writer.Write("{");

                indent++;

                writer.WriteLine();
                Indent(writer);
                writer.Write("Key: " + item.Key);

                writer.WriteLine();
                Indent(writer);
                writer.Write("Value: " + item.Value);

                indent--;

                writer.WriteLine();
                Indent(writer);
                writer.Write("}");
            }

            indent--;

            writer.WriteLine();
            Indent(writer);
            writer.Write("}");
        }
        private void WriteList(StringWriter writer)
        {
            writer.WriteLine();
            Indent(writer);
            writer.Write("{");

            indent++;

            foreach (Data item in AsList)
            {
                writer.WriteLine();
                Indent(writer);
                writer.Write("{");

                indent++;

                writer.WriteLine();
                Indent(writer);
                writer.Write(item);

                indent--;

                writer.WriteLine();
                Indent(writer);
                writer.Write("}");
            }

            indent--;

            writer.WriteLine();
            Indent(writer);
            writer.Write("}");
        }
        #endregion
    }
}
