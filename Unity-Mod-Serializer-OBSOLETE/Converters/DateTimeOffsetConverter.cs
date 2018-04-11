using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters
{
    public sealed class DateTimeOffsetConverter : DateConverter<DateTimeOffset>
    {
        private const string FORMAT_STRING = @"o";

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            var dateTimeOffset = (DateTimeOffset)instance;
            serialized = new Data(dateTimeOffset.ToString(FORMAT_STRING));
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if(data.IsString == false)
            {
                return Result.Fail("Date deserialization requires a string, not " + data.Type);
            }

            DateTimeOffset result;
            if (DateTimeOffset.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out result))
            {
                instance = result;
                return Result.Success;
            }

            return Result.Fail("Unable to parse " + data.AsString + " into a DateTimeOffset");
        }
    }
}
