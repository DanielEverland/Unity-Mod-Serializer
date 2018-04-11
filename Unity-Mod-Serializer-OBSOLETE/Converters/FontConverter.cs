using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.Converters
{
    public class FontConverter : DirectConverter<Font>
    {
        private const string KEY_FONTNAMES = "fontNames";
        private const string KEY_SIZE = "size";

        public override object CreateInstance(Data data, Type storageType)
        {
            return new Font();
        }
        protected override Result DoSerialize(Font model, Dictionary<string, Data> serialized)
        {
            Result result = Result.Success;

            result += SerializeMember(serialized, null, KEY_FONTNAMES, model.fontNames);
            result += SerializeMember(serialized, null, KEY_SIZE, model.fontSize);

            return result;
        }
        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Font model)
        {
            Result result = Result.Success;

            result += DeserializeMember(data, null, KEY_FONTNAMES, out string[] fontNames);
            result += DeserializeMember(data, null, KEY_SIZE, out int fontSize);

            model = Font.CreateDynamicFontFromOSFont(fontNames, fontSize);

            return result;
        }
    }
}
