using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    public enum DebuggingLevels
    {
        None = 0,

        Info = 1 << 0,
        Warning = 1 << 1,
        Error = 1 << 2,
    }
}
