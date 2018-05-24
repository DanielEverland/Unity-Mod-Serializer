using System;

namespace UMS.Editor
{
    /// <summary>
    /// Contains hooks allowing us to call code in the editor project.
    /// Essentially a poor-mans version of using UNITY_EDITOR compile time conditions
    /// </summary>
    public static class Hooks
    {
        public static Func<Settings> GetSettings { get; set; }
    }
}