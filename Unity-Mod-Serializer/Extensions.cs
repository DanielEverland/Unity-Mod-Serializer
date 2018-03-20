using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    public static class Extensions
    {
        public static string ToJson(this object obj)
        {
            return Mods.Serialize(obj);
        }
    }
}
