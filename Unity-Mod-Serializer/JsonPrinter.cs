using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;

namespace UMS
{
    public static class JsonPrinter
    {
        /// <summary>
        /// Inserts the given number of indents into the builder.
        /// </summary>
        private static void InsertSpacing(TextWriter stream, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                stream.Write("    ");
            }
        }

        /// <summary>
        /// Escapes a string.
        /// </summary>
        private static string EscapeString(string str)
        {
            // Escaping a string is pretty allocation heavy, so we try hard to
            // not do it.

            bool needsEscape = false;
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];

                // unicode code point
                int intChar = Convert.ToInt32(c);
                if (intChar < 0 || intChar > 127)
                {
                    needsEscape = true;
                    break;
                }

                // standard escape character
                switch (c)
                {
                    case '"':
                    case '\\':
                    case '\a':
                    case '\b':
                    case '\f':
                    case '\n':
                    case '\r':
                    case '\t':
                    case '\0':
                        needsEscape = true;
                        break;
                }

                if (needsEscape)
                {
                    break;
                }
            }

            if (needsEscape == false)
            {
                return str;
            }

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];

                // unicode code point
                int intChar = Convert.ToInt32(c);
                if (intChar < 0 || intChar > 127)
                {
                    result.Append(string.Format("\\u{0:x4} ", intChar).Trim());
                    continue;
                }

                // standard escape character
                switch (c)
                {
                    case '"': result.Append("\\\""); continue;
                    case '\\': result.Append(@"\\"); continue;
                    case '\a': result.Append(@"\a"); continue;
                    case '\b': result.Append(@"\b"); continue;
                    case '\f': result.Append(@"\f"); continue;
                    case '\n': result.Append(@"\n"); continue;
                    case '\r': result.Append(@"\r"); continue;
                    case '\t': result.Append(@"\t"); continue;
                    case '\0': result.Append(@"\0"); continue;
                }

                // no escaping needed
                result.Append(c);
            }
            return result.ToString();
        }

        private static void BuildCompressedString(Data data, TextWriter stream)
        {
            switch (data.Type)
            {
                case DataType.Null:
                    stream.Write("null");
                    break;

                case DataType.Boolean:
                    if (data.AsBool) stream.Write("true");
                    else stream.Write("false");
                    break;

                case DataType.Double:
                    // doubles must *always* include a decimal
                    stream.Write(ConvertDoubleToString(data.AsDouble));
                    break;

                case DataType.Int64:
                    stream.Write(data.AsInt64);
                    break;

                case DataType.String:
                    stream.Write('"');
                    stream.Write(EscapeString(data.AsString));
                    stream.Write('"');
                    break;

                case DataType.Object:
                    {
                        stream.Write('{');
                        bool comma = false;
                        foreach (var entry in data.AsDictionary)
                        {
                            if (comma) stream.Write(',');
                            comma = true;
                            stream.Write('"');
                            stream.Write(entry.Key);
                            stream.Write('"');
                            stream.Write(":");
                            BuildCompressedString(entry.Value, stream);
                        }
                        stream.Write('}');
                        break;
                    }

                case DataType.List:
                    {
                        stream.Write('[');
                        bool comma = false;
                        foreach (var entry in data.AsList)
                        {
                            if (comma) stream.Write(',');
                            comma = true;
                            BuildCompressedString(entry, stream);
                        }
                        stream.Write(']');
                        break;
                    }

                case DataType.Array:
                    {
                        Array array = data.AsArray;
                        int dimensions = array.Rank;

                        stream.Write('[');

                        if (dimensions > 1)
                            stream.Write('[');

                        for (int i = 0; i < array.GetLength(0); i++)
                        {
                            int[] index = new int[dimensions];
                            index[0] = i;

                            for (int d = 1; d < dimensions; d++)
                            {
                                index[d] = d;
                            }

                            Data obj = array.GetValue(index) as Data;
                            BuildCompressedString(obj, stream);
                        }

                        if (dimensions > 1)
                            stream.Write(']');

                        break;
                    }
                default:
                    throw new NotImplementedException("Data type " + data.Type + " is not recognized");
            }
        }

        /// <summary>
        /// Formats this data into the given builder.
        /// </summary>
        private static void BuildPrettyString(Data data, TextWriter stream, int depth)
        {
            switch (data.Type)
            {
                case DataType.Null:
                    stream.Write("null");
                    break;

                case DataType.Boolean:
                    if (data.AsBool) stream.Write("true");
                    else stream.Write("false");
                    break;

                case DataType.Double:
                    stream.Write(ConvertDoubleToString(data.AsDouble));
                    break;

                case DataType.Int64:
                    stream.Write(data.AsInt64);
                    break;

                case DataType.String:
                    stream.Write('"');
                    stream.Write(EscapeString(data.AsString));
                    stream.Write('"');
                    break;

                case DataType.Object:
                    {
                        stream.Write('{');
                        stream.WriteLine();
                        bool comma = false;
                        foreach (var entry in data.AsDictionary)
                        {
                            if (comma)
                            {
                                stream.Write(',');
                                stream.WriteLine();
                            }
                            comma = true;
                            InsertSpacing(stream, depth + 1);
                            stream.Write('"');
                            stream.Write(entry.Key);
                            stream.Write('"');
                            stream.Write(": ");
                            BuildPrettyString(entry.Value, stream, depth + 1);
                        }
                        stream.WriteLine();
                        InsertSpacing(stream, depth);
                        stream.Write('}');
                        break;
                    }

                case DataType.List:
                    // special case for empty lists; we don't put an empty line
                    // between the brackets
                    if (data.AsList.Count == 0)
                    {
                        stream.Write("[]");
                    }
                    else
                    {
                        bool comma = false;

                        stream.Write('[');
                        stream.WriteLine();
                        foreach (var entry in data.AsList)
                        {
                            if (comma)
                            {
                                stream.Write(',');
                                stream.WriteLine();
                            }
                            comma = true;
                            InsertSpacing(stream, depth + 1);
                            BuildPrettyString(entry, stream, depth + 1);
                        }
                        stream.WriteLine();
                        InsertSpacing(stream, depth);
                        stream.Write(']');
                    }
                    break;

                case DataType.Array:
                    {
                        Array array = data.AsArray;
                        int dimensions = array.Rank;

                        if(array.Length == 0)
                        {
                            stream.Write("[]");
                            break;
                        }

                        if(dimensions == 1)
                        {
                            BuildPrettySingleDimensionalArray(data, stream, depth);
                        }
                        else
                        {
                            BuildPrettyMultiDimensionalArray(data, stream, depth);
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException("Data type " + data.Type + " is not recognized");
            }
        }

        private static void BuildPrettySingleDimensionalArray(Data data, TextWriter stream, int depth)
        {
            bool comma = false;

            stream.Write('[');
            stream.WriteLine();
            foreach (var entry in data.AsArray)
            {
                if (comma)
                {
                    stream.Write(',');
                    stream.WriteLine();
                }
                comma = true;
                InsertSpacing(stream, depth + 1);

                Data dataEntry = entry as Data;
                if (dataEntry == null)
                    throw new ArgumentException("Found a non-data entry in array");

                BuildPrettyString(dataEntry, stream, depth + 1);
            }
            stream.WriteLine();
            InsertSpacing(stream, depth);
            stream.Write(']');
        }
        private static void BuildPrettyMultiDimensionalArray(Data data, TextWriter stream, int depth)
        {
            Array array = data.AsArray;
            int dimensions = array.Rank;

            stream.Write('[');
            stream.WriteLine();

            InsertSpacing(stream, depth + 1);
            stream.Write('[');

            //Indexing is done incorrectly. We're currently getting the last elements in every object at i
            //Scratch the last scentence. This is Daniel on drugs, I'm pretty sure I found a way to iterate
            //over the array. As you can tell it's quite simple, so you should find a stackoverflow answer
            //and make sure the implementation doesn't have any errors. In particular the ?: expression in 
            //the second for loops is scetchy as shit.
            int[] indexes = new int[array.Rank];
            for (int d = 0; d < array.Rank; d++)
            {
                for (int i = d == 0 ? 0 : 1; i < array.GetLength(d); i++)
                {
                    indexes[d] = i;

                    Data obj = array.GetValue(indexes) as Data;

                    if (obj == null)
                        throw new NullReferenceException("Null data - " + array.GetValue(indexes));

                    stream.Write(' ');
                    BuildPrettyString(obj, stream, depth + 1);

                    if (i == array.GetLength(0) - 1)
                        stream.Write(' ');
                    else
                        stream.Write(',');
                }
            }

            stream.Write(']');
            stream.Write(',');
            stream.WriteLine();

            InsertSpacing(stream, depth);
            stream.Write(']');
        }

        /// <summary>
        /// Writes the pretty JSON output data to the given stream.
        /// </summary>
        /// <param name="data">The data to print.</param>
        /// <param name="outputStream">Where to write the printed data.</param>
        public static void PrettyJson(Data data, TextWriter outputStream)
        {
            BuildPrettyString(data, outputStream, 0);
        }

        /// <summary>
        /// Returns the data in a pretty printed JSON format.
        /// </summary>
        public static string PrettyJson(Data data)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                BuildPrettyString(data, writer, 0);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Writes the compressed JSON output data to the given stream.
        /// </summary>
        /// <param name="data">The data to print.</param>
        /// <param name="outputStream">Where to write the printed data.</param>
        public static void CompressedJson(Data data, StreamWriter outputStream)
        {
            BuildCompressedString(data, outputStream);
        }

        /// <summary>
        /// Returns the data in a relatively compressed JSON format.
        /// </summary>
        public static string CompressedJson(Data data)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                BuildCompressedString(data, writer);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Utility method that converts a double to a string.
        /// </summary>
        private static string ConvertDoubleToString(double d)
        {
            if (Double.IsInfinity(d) || Double.IsNaN(d))
                return d.ToString(CultureInfo.InvariantCulture);

            string doubledString = d.ToString(CultureInfo.InvariantCulture);

            // NOTE/HACK: If we don't serialize with a period or an exponent,
            // then the number will be deserialized as an Int64, not a double.
            if (doubledString.Contains(".") == false &&
                doubledString.Contains("e") == false &&
                doubledString.Contains("E") == false)
            {
                doubledString += ".0";
            }

            return doubledString;
        }
    }
}