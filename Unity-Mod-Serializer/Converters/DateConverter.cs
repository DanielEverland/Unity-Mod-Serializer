using System;
using System.Globalization;

namespace UMS.Converters
{
    /// <summary>
    /// Supports serialization for DateTime, DateTimeOffset, and TimeSpan.
    /// </summary>
    public abstract class DateConverter<T> : DirectConverter<T>
    {
        // The format strings that we use when serializing DateTime and
        // DateTimeOffset types.
        public const string DEFAULT_DATE_TIME_FORMAT_STRING = @"o";

        public string DateTimeFormatString
        {
            get
            {
                return Serializer.Config.CustomDateTimeFormatString ?? DEFAULT_DATE_TIME_FORMAT_STRING;
            }
        }
    }
}