using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters
{
    /// <summary>
    /// Direct converters do not support inheritance,
    /// and can only serialize direct instances of a type
    /// </summary>
    public interface IDirectConverter : IBaseConverter
    {
    }
}
