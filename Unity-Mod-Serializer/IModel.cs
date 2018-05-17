using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;

namespace UMS
{
    public interface IModel
    {
        System.Type ModelType { get; }

        void CreateModel(MetaType type);
    }
}
