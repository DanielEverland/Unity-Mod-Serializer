using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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
