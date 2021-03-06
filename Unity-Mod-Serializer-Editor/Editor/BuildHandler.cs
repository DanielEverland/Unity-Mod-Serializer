﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Reflection;

namespace UMS.Editor
{
    public static class BuildHandler
    {
        private static string _pathToManagedFolder;
        private static string _pathToRootBuildFolder;
        private static string _pathToRootModsFolder;
        private static string _pathToCoreMods;
        private static string _pathToLibrary;

        [PostProcessBuild()]
        private static void PostBuild(BuildTarget target, string pathToBuiltProject)
        {
            _pathToRootBuildFolder = string.Format(@"{0}\{1}_Data", Path.GetDirectoryName(pathToBuiltProject), Path.GetFileNameWithoutExtension(pathToBuiltProject));
            _pathToManagedFolder = $@"{_pathToRootBuildFolder}\Managed";

            BuildMods();
        }
        private static void BuildMods()
        {
            BuildSettings();
            CreateModsDirectory();
            BuildCoreMods();
            CreateLibraries();
        }
        private static void BuildSettings()
        {
            string json = JsonUtility.ToJson(Settings.Instance, true);

            File.WriteAllText(_pathToRootBuildFolder + @"\" + Settings.FILE_NAME, json);
        }
        private static void BuildCoreMods()
        {
            CreateCoreModsFolder();

            string[] guids = AssetDatabase.FindAssets("t:ModPackage");

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);

                if (obj is ModPackage package)
                {
                    BuildMod(package);
                }
            }
        }
        private static void BuildMod(ModPackage package)
        {
            if (!package.IncludeInBuilds)
                return;

            try
            {
                package.Save(_pathToCoreMods);
            }
            catch (System.Exception)
            {
                Debugging.Error(DebuggingFlags.Serializer, $"Failed to serialize {package}");
                throw;
            }
        }
        private static void CreateCoreModsFolder()
        {
            _pathToCoreMods = string.Format($@"{_pathToRootModsFolder}\{Settings.CoreFolderName}");

            Directory.CreateDirectory(_pathToCoreMods);
        }
        private static void CreateModsDirectory()
        {
            _pathToRootModsFolder = string.Format($@"{_pathToRootBuildFolder}\{Settings.ModsDirectory}");

            Directory.CreateDirectory(_pathToRootModsFolder);
        }
        private static void CreateLibraries()
        {
            if (Settings.PredefinedAssemblies.Count() == 0)
                return;

            _pathToLibrary = string.Format($@"{_pathToRootModsFolder}\{Settings.PredefinedAssembliesFolderName}");

            Directory.CreateDirectory(_pathToLibrary);
            
            foreach (string assemblyName in Settings.PredefinedAssemblies)
            {
                File.Copy(AssemblyFilePath(assemblyName), string.Format(@"{0}\{1}.dll", _pathToLibrary, assemblyName));
            }
        }
        private static string AssemblyFilePath(string name)
        {
            foreach (string filePath in Directory.GetFiles(_pathToManagedFolder))
            {
                if(Path.GetFileNameWithoutExtension(filePath) == name)
                {
                    return filePath;
                }
            }

            throw new System.NullReferenceException($"Couldn't find {name}. It must be in the build project's Managed folder");
        }
    }
}