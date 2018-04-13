using UnityEngine;
using UnityEditor;

namespace UMS.Editor
{
    public static class SettingsHandler
    {
        public static Settings GetSettings()
        {
            Settings toReturn;

            if (SearchForSettings(out toReturn))
            {
                return toReturn;
            }
            else
            {
                return CreateNewSettings();
            }
        }
        private static bool SearchForSettings(out Settings settings)
        {
            string[] guids = AssetDatabase.FindAssets("t:Settings");

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);

                if (obj is Settings)
                {
                    settings = obj as Settings;
                    return true;
                }
            }

            settings = null;
            return false;
        }
        private static Settings CreateNewSettings()
        {
            Settings newSettings = ScriptableObject.CreateInstance<Settings>();
            newSettings.name = "UMS Settings";

            Selection.activeObject = newSettings;

            AssetDatabase.CreateAsset(newSettings, "Assets/" + newSettings.name + ".asset");

            Debug.LogWarning("Couldn't find settings. Creating new object!", newSettings);
            Debug.Log("Feel free to move me somewhere else in your project", newSettings);

            return newSettings;
        }
    }
}
