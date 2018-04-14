using System;

namespace UMS
{
    [Flags]
    public enum DebuggingFlags
    {
        None = 0,
        
        Serializer                  = 1 << 0,
        Reflection                  = 1 << 1,
        IDManager                   = 1 << 2,
        IDManagerDeepComparer       = 1 << 3,
        IDManagerIEquatableComparer = 1 << 4,
    }
}
