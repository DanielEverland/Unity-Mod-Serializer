using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters
{
    public sealed class DateTimeConverter : DateConverter<DateTime>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            var dateTime = (DateTime)instance;
            serialized = new Data(dateTime.ToString(DateTimeFormatString));
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (data.IsString == false)
            {
                return Result.Fail("Date deserialization requires a string, not " + data.Type);
            }

            DateTime result;
            if (DateTime.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out result))
            {
                instance = result;
                return Result.Success;
            }

            // DateTime.TryParse can fail for some valid DateTime instances.
            // Try to use Convert.ToDateTime.
            if (GlobalConfig.AllowInternalExceptions)
            {
                try
                {
                    instance = Convert.ToDateTime(data.AsString);
                    return Result.Success;
                }
                catch (Exception e)
                {
                    return Result.Fail("Unable to parse " + data.AsString + " into a DateTime; got exception " + e);
                }
            }

            return Result.Fail("Unable to parse " + data.AsString + " into a DateTime");
        }
    }
}
