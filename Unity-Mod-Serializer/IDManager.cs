using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    public static class IDManager
    {
        private static EventHook _hook;

        public static string GetID(object obj)
        {
            if (_hook == null)
                throw new NullReferenceException("There's no event hook assigned!");

            if(CanGetGUID(obj))
            {
                return _hook.GetGUID(obj);
            }

            throw new ArgumentException(obj + " isn't UnityObject");
        }
        public static bool CanGetGUID(object obj)
        {
            if (_hook == null)
                throw new NullReferenceException("There's no event hook assigned!");

            return _hook.CanGetGUID(obj);
        }
        public static void AssignHook(EventHook hook)
        {
            _hook = hook;
        }

        public class EventHook
        {
            public Func<object, string> GetGUID;
            public Func<object, bool> CanGetGUID;
        }
    }
}
