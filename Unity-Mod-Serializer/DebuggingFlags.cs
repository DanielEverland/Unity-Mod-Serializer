using System;

namespace UMS
{
    [Flags]
    public enum DebuggingFlags
    {
        None = 0,
        
        Serializer                  = 1 << 0,
        Reflection                  = 1 << 1,
    }
}
