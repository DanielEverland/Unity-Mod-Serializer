namespace UMS
{
    public enum DebuggingLevels
    {
        None = 0,

        Verbose = 1 << 0,
        Info    = 1 << 1,
        Warning = 1 << 2,
        Error   = 1 << 3,
    }
}
