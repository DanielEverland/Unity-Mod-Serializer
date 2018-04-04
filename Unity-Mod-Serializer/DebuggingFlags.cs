using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    [Flags]
    public enum DebuggingFlags
    {
        None = 0,

        IDManager = 1 << 0,
    }
}
