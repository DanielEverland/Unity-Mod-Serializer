using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UMS
{
    [CreateAssetMenu(fileName = "UMS Settings.asset", menuName = "Modding/Settings", order = Utility.MENU_ITEM_PRIORITY)]
    public class Settings : ScriptableObject
    {
        public static string ModsDirectory { get { return Instance._folderName; } }
        public static string CoreFolderName { get { return Instance._coreFolderName; } }
        public static string PredefinedAssembliesFolderName { get { return Instance._predefinedAssembliesFolderName; } }
        public static bool SimulateBuildLoading { get { return Instance._simulateBuildLoading; } }
        public static IEnumerable<string> PredefinedAssemblies { get { return Instance._predefinedAssemblies; } }

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetSettings();

                return _instance;
            }
        }
        private static Settings _instance;

        public const string FILE_NAME = "UMS.config";

        [SerializeField]
        [Tooltip("The name of the folder")]
        private string _folderName = "Mods";
        [SerializeField]
        [Tooltip("The name of the subfolder where the core files will reside")]
        private string _coreFolderName = "Core";
        [SerializeField]
        [Tooltip("Forces UMS to deserialize mods the same way as when the game has been built")]
        private bool _simulateBuildLoading = false;
        [SerializeField]
        [Tooltip("The name of the folder where predefined assemblies will be serialized to")]
        private string _predefinedAssembliesFolderName = "Libraries";
        [SerializeField]
        [Tooltip("Name of assemblies to include in the core bundle. Any assembly you want to be included in UMS' reflection scheme should be included here")]
        private List<string> _predefinedAssemblies = new List<string>() { "Assembly-CSharp", "Unity-Mod-Serializer", };
        [SerializeField]
        private DebuggingFlags _debuggingFlags;

        public static IEnumerable<Assembly> GetPredefinedAssemblies()
        {
            LinkedList<Assembly> assemblies = new LinkedList<Assembly>();

            foreach (string assemblyName in PredefinedAssemblies)
            {
                assemblies.AddLast(Assembly.Load(assemblyName));
            }

            return assemblies;
        }
        private static Settings GetSettings()
        {
            if (Application.isEditor)
            {
                return Editor.Hooks.GetSettings();
            }
            else
            {
                return LoadFromJSON();
            }
        }
        private static Settings LoadFromJSON()
        {
            string json = File.ReadAllText(Application.dataPath + @"\" + FILE_NAME);
            Settings settings = CreateInstance<Settings>();

            JsonUtility.FromJsonOverwrite(json, settings);

            return settings;
        }
    }
}