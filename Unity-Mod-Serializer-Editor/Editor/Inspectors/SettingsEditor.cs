﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace UMS.Editor.Inspectors
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        private string FolderName { get { return (string)_folderNameField.GetValue(target); } set { _folderNameField.SetValue(target, value); } }
        private string CoreFolderName { get { return (string)_coreFolderName.GetValue(target); } set { _coreFolderName.SetValue(target, value); } }
        private bool SimulateBuildLoading { get { return (bool)_simulateBuildLoading.GetValue(target); } set { _simulateBuildLoading.SetValue(target, value); } }
        private string PredefinedAssembliesFolderName { get { return (string)_predefinedAssembliesFolderName.GetValue(target); } set { _predefinedAssembliesFolderName.SetValue(target, value); } }
        private List<string> PredefinedAssemblies { get { return (List<string>)_predefinedAssemblies.GetValue(target); } set { _predefinedAssemblies.SetValue(target, value); } }

        private BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private FieldInfo _folderNameField;
        private FieldInfo _coreFolderName;
        private FieldInfo _simulateBuildLoading;
        private FieldInfo _predefinedAssembliesFolderName;
        private FieldInfo _predefinedAssemblies;

        private ReorderableList _predefinedAssembliesList;

        private void OnEnable()
        {
            SetupFieldHooks();
            CreateReorderableList();
        }
        private void CreateReorderableList()
        {
            _predefinedAssembliesList = new ReorderableList(serializedObject, serializedObject.FindProperty("_predefinedAssemblies"));

            _predefinedAssembliesList.drawHeaderCallback += (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Predefined Assemblies");
            };

            _predefinedAssembliesList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect fixedRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                PredefinedAssemblies[index] = EditorGUI.TextField(fixedRect, PredefinedAssemblies[index]);
            };
        }
        private void SetupFieldHooks()
        {
            _folderNameField = typeof(Settings).GetField("_folderName", _bindingFlags);
            _coreFolderName = typeof(Settings).GetField("_coreFolderName", _bindingFlags);
            _simulateBuildLoading = typeof(Settings).GetField("_simulateBuildLoading", _bindingFlags);
            _predefinedAssembliesFolderName = typeof(Settings).GetField("_predefinedAssembliesFolderName", _bindingFlags);
            _predefinedAssemblies = typeof(Settings).GetField("_predefinedAssemblies", _bindingFlags);
        }
        public override void OnInspectorGUI()
        {
            DrawBuildSettings();

            EditorGUILayout.Space();

            DrawDebbugingSettings();

            EditorGUILayout.Space();

            DrawAdvancedSettings();
        }
        private void DrawBuildSettings()
        {
            EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);

            FolderName = EditorGUILayout.TextField(new GUIContent("Folder Name", "Name of the folder in which mods will be located. Relative to Application.dataPath, supports subfolders"), FolderName);
            CoreFolderName = EditorGUILayout.TextField(new GUIContent("Core Folder Name", "Name of the folder in which your ModPackage assets will be build. Relative to Folder Name"), CoreFolderName);
            PredefinedAssembliesFolderName = EditorGUILayout.TextField(new GUIContent("Assemblies Folder Name", "Name of the folder in which to put predefined assemblies. Relative to Core Folder"), PredefinedAssembliesFolderName);
        }
        private void DrawDebbugingSettings()
        {
            EditorGUILayout.LabelField("Debugging Settings", EditorStyles.boldLabel);

            GUIContent simulateBuildLoadingContent = new GUIContent("Simulate Build Loading",
                "Only affects edit mode. We usually just load the object references directly from the mod packages," +
                "this setting changes that, so we start by serializing all the packages to a temp folder, and then we" +
                "deserialize it in the same way we do when the game has been built");

            SimulateBuildLoading = EditorGUILayout.Toggle(simulateBuildLoadingContent, SimulateBuildLoading);
        }
        private void DrawAdvancedSettings()
        {
            EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            _predefinedAssembliesList.DoLayoutList();
        }
    }
}