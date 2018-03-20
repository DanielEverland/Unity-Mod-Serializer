using System;

namespace UMS.AOT
{
    /// <summary>
    /// Interface that AOT generated converters extend. Used to check to see if
    /// the AOT converter is up to date.
    /// </summary>
    public interface AOTConverter
    {
        Type ModelType { get; }
        AOTVersionInfo VersionInfo { get; }
    }
}