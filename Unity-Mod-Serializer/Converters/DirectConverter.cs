using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters
{
    public abstract class DirectConverter<T> : BaseConverter<T>, IDirectConverter<T>
    {
    }
}
