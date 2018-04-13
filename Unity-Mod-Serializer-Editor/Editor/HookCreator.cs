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
