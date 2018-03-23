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

            if(CanGetID(obj))
            {
                return _hook.GetID(obj);
            }

            throw new ArgumentException(obj + " isn't UnityObject");
        }
        public static bool CanGetID(object obj)
        {
            if (_hook == null)
                throw new NullReferenceException("There's no event hook assigned!");

            return _hook.CanGetID(obj);
        }
        public static void AssignHook(EventHook hook)
        {
            _hook = hook;
        }

        public class EventHook
        {
            public Func<object, string> GetID;
            public Func<object, bool> CanGetID;
        }
    }
}
