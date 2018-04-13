using System;

namespace UMS
{
    [Flags]
    public enum DebuggingFlags
    {
        None = 0,

        IDManager                   = 1 << 0,
        IDManagerDeepComparer       = 1 << 1,
        IDManagerIEquatableComparer = 1 << 2,
        Serializer                  = 1 << 3,
    }
}
