using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;

namespace UMS.Editor
{
    public static class HookCreator
    {
        [DidReloadScripts]
        private static void CreateHooks()
        {
            Hooks.GetSettings = SettingsHandler.GetSettings;
        }
    }
}
