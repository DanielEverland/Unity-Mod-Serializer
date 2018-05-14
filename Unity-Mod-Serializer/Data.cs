using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

namespace UMS
{
    /// <summary>
    /// Helper class used to add an abstraction layer,
    /// so we don't have to fiddle around with Protobuf in converters
    /// </summary>
    [ProtoContract]
    public sealed class Data
    {
        #region Values
        /* This implementation is slightly retarded, but ultimately necessary to properly use Protobuf.
         * Note that even though we have a lot of duplicate fields, Protobuf will completely ignore them
         * unless they have a value assigned. So as long as we make sure only one field has a value at a
         * time, we get the flexibility of using a BinaryFormatter, while harnesing Protobuf's performance
         */

        [ProtoMember(1)]
        private bool? _boolValue;
        [ProtoMember(2)]
        private byte? _byteValue;
        [ProtoMember(3)]
        private sbyte? _sbyteValue;
        [ProtoMember(4)]
        private short? _shortValue;
        [ProtoMember(5)]
        private ushort? _ushortValue;
        [ProtoMember(6)]
        private int? _intValue;
        [ProtoMember(7)]
        private uint? _uintValue;
        [ProtoMember(8)]
        private long? _longValue;
        [ProtoMember(9)]
        private ulong? _ulongValue;
        [ProtoMember(10)]
        private float? _floatValue;
        [ProtoMember(11)]
        private double? _doubleValue;
        [ProtoMember(12)]
        private decimal? _decimalValue;
        [ProtoMember(13)]
        private char? _charValue;
        [ProtoMember(14)]
        private string _stringValue;
        [ProtoMember(15)]
        private byte[] _byteArrayValue;
        [ProtoMember(16)]
        private List<Data> _listValue;
        [ProtoMember(17)]
        private Dictionary<string, Data> _dictionaryValue;
        #endregion

        public Result Add(Data data)
        {
            if (!IsList)
                return Result.Error("Type mismatch. Expected List", this);

            List.Add(data);

            return Result.Success;
        }

        #region Indexers
        public Data this[string index]
        {
            get
            {
                if (!IsDictionary)
                    throw new System.InvalidOperationException("Tried to index a non-dictionary Data object " + this);

                return Dictionary[index];
            }
            set
            {
                if (!IsDictionary)
                    throw new System.InvalidOperationException("Tried to index a non-dictionary Data object " + this);

                Dictionary.Set(index, value);
            }
        }
        public Data this[int index]
        {
            get
            {
                if (!IsList)
                    throw new System.InvalidOperationException("Tried to index a non-list Data object " + this);

                return List[index];
            }
            set
            {
                if (!IsList)
                    throw new System.InvalidOperationException("Tried to index a non-list Data object " + this);

                List[index] = value;
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
        }
        public Data(bool value)
        {
            Bool = value;
        }
        public Data(byte value)
        {
            Byte = value;
        }
        public Data(sbyte value)
        {
            SByte = value;
        }
        public Data(short value)
        {
            Short = value;
        }
        public Data(ushort value)
        {
            UShort = value;
        }
        public Data(int value)
        {
            Int = value;
        }
        public Data(uint value)
        {
            UInt = value;
        }
        public Data(long value)
        {
            Long = value;
        }
        public Data(ulong value)
        {
            ULong = value;
        }
        public Data(float value)
        {
            Float = value;
        }
        public Data(double value)
        {
            Double = value;
        }
        public Data(decimal value)
        {
            Decimal = value;
        }
        public Data(char value)
        {
            Char = value;
        }
        public Data(string value)
        {
            String = value;
        }
        public Data(byte[] value)
        {
            ByteArray = value;
        }
        public Data(List<Data> value)
        {
            List = value;
        }
        public Data(Dictionary<string, Data> value)
        {
            Dictionary = value;
        }
        #endregion

        #region Properties
        public bool     Bool    { get { return _boolValue.Value; }      set { Clear(); _boolValue = value; } }
        public byte     Byte    { get { return _byteValue.Value; }      set { Clear(); _byteValue = value; } }
        public sbyte    SByte   { get { return _sbyteValue.Value; }     set { Clear(); _sbyteValue = value; } }
        public short    Short   { get { return _shortValue.Value; }     set { Clear(); _shortValue = value; } }
        public ushort   UShort  { get { return _ushortValue.Value; }    set { Clear(); _ushortValue = value; } }
        public int      Int     { get { return _intValue.Value; }       set { Clear(); _intValue = value; } }
        public uint     UInt    { get { return _uintValue.Value; }      set { Clear(); _uintValue = value; } }
        public long     Long    { get { return _longValue.Value; }      set { Clear(); _longValue = value; } }
        public ulong    ULong   { get { return _ulongValue.Value; }     set { Clear(); _ulongValue = value; } }
        public float    Float   { get { return _floatValue.Value; }     set { Clear(); _floatValue = value; } }
        public double   Double  { get { return _doubleValue.Value; }    set { Clear(); _doubleValue = value; } }
        public decimal  Decimal { get { return _decimalValue.Value; }   set { Clear(); _decimalValue = value; } }
        public char     Char    { get { return _charValue.Value; }      set { Clear(); _charValue = value; } }
        public string   String  { get { return _stringValue; }          set { Clear(); _stringValue = value; } }
        public byte[]                   ByteArray   { get { return _byteArrayValue; }   set { Clear(); _byteArrayValue = value; } }
        public List<Data>               List        { get { return _listValue; }        set { Clear(); _listValue = value; } }
        public Dictionary<string, Data> Dictionary  { get { return _dictionaryValue; }  set { Clear(); _dictionaryValue = value; } }


        public bool IsNull          { get { return Type == DataType.Null; } }
        public bool IsBool          { get { return _boolValue.HasValue; } }
        public bool IsByte          { get { return _byteValue.HasValue; } }
        public bool IsSByte         { get { return _sbyteValue.HasValue; } }
        public bool IsShort         { get { return _shortValue.HasValue; } }
        public bool IsUShort        { get { return _ushortValue.HasValue; } }
        public bool IsInt           { get { return _intValue.HasValue; } }
        public bool IsUInt          { get { return _uintValue.HasValue; } }
        public bool IsLong          { get { return _longValue.HasValue; } }
        public bool IsULong         { get { return _ulongValue.HasValue; } }
        public bool IsFloat         { get { return _floatValue.HasValue; } }
        public bool IsDouble        { get { return _doubleValue.HasValue; } }
        public bool IsDecimal       { get { return _decimalValue.HasValue; } }
        public bool IsChar          { get { return _charValue.HasValue; } }
        public bool IsString        { get { return _stringValue != null; } }
        public bool IsByteArray     { get { return _byteArrayValue != null; } }
        public bool IsList          { get { return _listValue != null; } }
        public bool IsDictionary    { get { return _dictionaryValue != null; } }

        public object Value
        {
            get
            {
                //We check these first because they're super common
                if (IsByteArray) return ByteArray;
                if (IsList) return List;
                if (IsDictionary) return Dictionary;

                if (IsBool) return Bool;
                if (IsByte) return Byte;
                if (IsSByte) return SByte;
                if (IsShort) return Short;
                if (IsUShort) return UShort;
                if (IsInt) return Int;
                if (IsUInt) return UInt;
                if (IsLong) return Long;
                if (IsULong) return ULong;
                if (IsFloat) return Float;
                if (IsDouble) return Double;
                if (IsDecimal) return Decimal;
                if (IsChar) return Char;
                if (IsString) return String;

                return null;
            }
        }
        #endregion

        #region Type Definitions
        public DataType Type
        {
            get
            {
                //We check these first because they're super common
                if (IsByteArray)    return DataType.ByteArray;
                if (IsList)         return DataType.List;
                if (IsDictionary)   return DataType.Dictionary;

                if (IsBool)     return DataType.Bool;
                if (IsByte)     return DataType.Byte;
                if (IsSByte)    return DataType.SignedByte;
                if (IsShort)    return DataType.Short;
                if (IsUShort)   return DataType.UnsignedShort;
                if (IsInt)      return DataType.Int;
                if (IsUInt)     return DataType.UnsignedInt;
                if (IsLong)     return DataType.Long;
                if (IsULong)    return DataType.UnsignedLong;
                if (IsFloat)    return DataType.Float;
                if (IsDouble)   return DataType.Double;
                if (IsDecimal)  return DataType.Decimal;
                if (IsChar)     return DataType.Char;
                if (IsString)   return DataType.String;

                return DataType.Null;
            }
        }
        public enum DataType
        {
            Null = 0,

            Bool = 1,
            Byte = 2,
            SignedByte = 3,
            Short = 4,
            UnsignedShort = 5,
            Int = 6,
            UnsignedInt = 7,
            Long = 8,
            UnsignedLong = 9,
            Float = 10,
            Double = 11,
            Decimal = 12,
            Char = 13,
            String = 14,
            ByteArray = 15,
            List = 16,
            Dictionary = 17,
        }
        #endregion

        #region Internal Helper Methods
        public void Clear()
        {
            _boolValue = null;
            _byteValue = null;
            _sbyteValue = null;
            _shortValue = null;
            _ushortValue = null;
            _intValue = null;
            _uintValue = null;
            _longValue = null;
            _ulongValue = null;
            _floatValue = null;
            _doubleValue = null;
            _decimalValue = null;
            _charValue = null;
            _stringValue = null;
            _byteArrayValue = null;
            _listValue = null;
            _dictionaryValue = null;
    }
        #endregion

        #region Overrides
        static int indent = 0;
        public override string ToString()
        {
            if (Value == null)
                return "null";

            if (IsByteArray)
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
                return Value.ToString();
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
            Indent(writer);
            writer.Write("{");

            indent++;

            foreach (KeyValuePair<string, Data> item in Dictionary)
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
            writer.WriteLine();
        }
        private void WriteList(StringWriter writer)
        {
            writer.WriteLine();
            Indent(writer);
            writer.Write("{");

            indent++;

            foreach (Data item in List)
            {
                indent++;

                writer.WriteLine();
                Indent(writer);
                writer.Write(item);
                writer.Write(',');

                indent--;
            }

            indent--;

            writer.WriteLine();
            Indent(writer);
            writer.Write("}");
            writer.WriteLine();
        }
        #endregion
    }
}
