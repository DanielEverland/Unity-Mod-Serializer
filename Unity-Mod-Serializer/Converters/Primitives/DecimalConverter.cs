using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class DecimalConverter : DirectConverter<decimal>
    {
        public override Result Serialize(decimal obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result Deserialize(Data data, ref decimal obj)
        {
            if (!data.IsDecimal)
                return Result.Error("Type mismatch. Expected Decimal", data);

            obj = data.AsDecimal;
            return Result.Success;
        }
    }
}
