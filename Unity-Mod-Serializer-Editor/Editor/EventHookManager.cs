using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Callbacks;
using UnityEngine;
using UMS;

namespace UMS.Editor
{
    public static class EventHookManager
    {
        [DidReloadScripts]
        private static void CreateHook()
        {
            IDManager.EventHook hook = new IDManager.EventHook()
            {
                GetID = obj =>
                {
                    return EditorUtilities.GetID(obj);
                },
                CanGetID = obj =>
                {
                    return EditorUtilities.CanGetID(obj);
                }
            };

            IDManager.AssignHook(hook);
        }
    }
}
