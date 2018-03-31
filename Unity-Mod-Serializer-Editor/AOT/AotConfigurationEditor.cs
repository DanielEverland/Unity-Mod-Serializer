#if !NO_UNITY
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UMS.AOT;
using UMS.Reflection;
using UnityEditor;
using UnityEngine;

namespace UMS.Editor
{
    [InitializeOnLoad]
    public static class PlayStateNotifier
    {
        static PlayStateNotifier()
        {
            EditorApplication.playmodeStateChanged += ModeChanged;
        }

        private static void ModeChanged()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
            {
                Debug.Log("There are " + AOTCompilationManager.AotCandidateTypes.Count + " candidate types");
                foreach (AOTConfiguration target in Resources.FindObjectsOfTypeAll<AOTConfiguration>())
                {
                    var seen = new HashSet<string>(target.aotTypes.Select(t => t.FullTypeName));
                    foreach (Type type in AOTCompilationManager.AotCandidateTypes)
                    {
                        if (seen.Contains(type.FullName) == false)
                        {
                            target.aotTypes.Add(new AOTConfiguration.Entry(type));
                            EditorUtility.SetDirty(target);
                        }
                    }
                }
            }
        }
    }

    [CustomEditor(typeof(AOTConfiguration))]
    public class AOTConfigurationEditor : UnityEditor.Editor
    {
        [NonSerialized]
        private List<Type> _allAOTTypes;
        private List<Type> allAOTTypes
        {
            get
            {
                if (_allAOTTypes == null)
                    _allAOTTypes = FindAllAotTypes().ToList();
                return _allAOTTypes;
            }
        }

        private string[] options = new string[] { "On", "Off", "[?]" };
        private int GetIndexForState(AOTConfiguration.AotState state)
        {
            switch (state)
            {
                case AOTConfiguration.AotState.Enabled:
                    return 0;
                case AOTConfiguration.AotState.Disabled:
                    return 1;
                case AOTConfiguration.AotState.Default:
                    return 2;
            }

            throw new ArgumentException("state is invalid " + state);
        }
        private AOTConfiguration.AotState GetStateForIndex(int index)
        {
            switch (index)
            {
                case 0: return AOTConfiguration.AotState.Enabled;
                case 1: return AOTConfiguration.AotState.Disabled;
                case 2: return AOTConfiguration.AotState.Default;
            }

            throw new ArgumentException("invalid index " + index);
        }

        private IEnumerable<Type> FindAllAotTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    bool performAot = false;

                    // check for [Object]
                    {
                        var props = t.GetCustomAttributes(typeof(ObjectAttribute), true);
                        if (props != null && props.Length > 0) performAot = true;
                    }

                    // check for [Property]
                    if (!performAot)
                    {
                        foreach (PropertyInfo p in t.GetProperties())
                        {
                            var props = p.GetCustomAttributes(typeof(PropertyAttribute), true);
                            if (props.Length > 0)
                            {
                                performAot = true;
                                break;
                            }
                        }
                    }

                    if (performAot)
                        yield return t;
                }
            }
        }

        private enum OutOfDateResult
        {
            NoAot,
            Stale,
            Current
        }
        private OutOfDateResult IsOutOfDate(Type type)
        {
            string converterName = AOTCompilationManager.GetQualifiedConverterNameForType(type);
            Type converterType = TypeCache.GetType(converterName);
            if (converterType == null)
                return OutOfDateResult.NoAot;
            
            object instance_ = null;
            try
            {
                instance_ = Activator.CreateInstance(converterType);
            }
            catch (Exception) { }
            if (instance_ is AOTConverter == false)
                return OutOfDateResult.NoAot;
            var instance = (AOTConverter)instance_;

            var metatype = MetaType.Get(new Config(), type);
            if (AOTCompilationManager.IsAotModelUpToDate(metatype, instance) == false)
                return OutOfDateResult.Stale;

            return OutOfDateResult.Current;
        }

        private void DrawType(AOTConfiguration.Entry entry, Type resolvedType)
        {
            var target = (AOTConfiguration)this.target;

            EditorGUILayout.BeginHorizontal();

            int currentIndex = GetIndexForState(entry.State);
            int newIndex = GUILayout.Toolbar(currentIndex, options, GUILayout.ExpandWidth(false));
            if (currentIndex != newIndex)
            {
                entry.State = GetStateForIndex(newIndex);
                target.UpdateOrAddEntry(entry);
                EditorUtility.SetDirty(target);
            }

            string displayName = entry.FullTypeName;
            string tooltip = "";
            if (resolvedType != null)
            {
                displayName = resolvedType.CSharpName();
                tooltip = resolvedType.CSharpName(true);
            }
            GUIContent label = new GUIContent(displayName, tooltip);
            GUILayout.Label(label);

            GUILayout.FlexibleSpace();

            GUIStyle messageStyle = new GUIStyle(EditorStyles.label);
            string message;
            if (resolvedType != null)
            {
                message = GetAotCompilationMessage(resolvedType);
                if (string.IsNullOrEmpty(message) == false)
                {
                    messageStyle.normal.textColor = Color.red;
                }
                else
                {
                    switch (IsOutOfDate(resolvedType))
                    {
                        case OutOfDateResult.NoAot:
                            message = "No AOT model found";
                            break;
                        case OutOfDateResult.Stale:
                            message = "Stale";
                            break;
                        case OutOfDateResult.Current:
                            message = "\u2713";
                            break;
                    }
                }
            }
            else
            {
                message = "Cannot load type";
            }

            GUILayout.Label(message, messageStyle);

            EditorGUILayout.EndHorizontal();
        }

        private string GetAotCompilationMessage(Type type)
        {
            try
            {
                MetaType.Get(new Config(), type).EmitAotData(true);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "";
        }

        private Vector2 _scrollPos;
        public override void OnInspectorGUI()
        {
            var target = (AOTConfiguration)this.target;

            if (GUILayout.Button("Compile"))
            {
                if (Directory.Exists(target.outputDirectory) == false)
                    Directory.CreateDirectory(target.outputDirectory);

                foreach (AOTConfiguration.Entry entry in target.aotTypes)
                {
                    if (entry.State == AOTConfiguration.AotState.Enabled)
                    {
                        Type resolvedType = TypeCache.GetType(entry.FullTypeName);
                        if (resolvedType == null)
                        {
                            Debug.LogError("Cannot find type " + entry.FullTypeName);
                            continue;
                        }

                        try
                        {
                            string compilation = AOTCompilationManager.RunAotCompilationForType(new Config(), resolvedType);
                            string path = Path.Combine(target.outputDirectory, "AotConverter_" + resolvedType.CSharpName(true, true) + ".cs");
                            File.WriteAllText(path, compilation);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("AOT compiling " + resolvedType.CSharpName(true) + " failed: " + e.Message);
                        }
                    }
                }
                AssetDatabase.Refresh();
            }

            target.outputDirectory = EditorGUILayout.TextField("Output Directory", target.outputDirectory);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Set All");
            int newIndex = GUILayout.Toolbar(-1, options, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (newIndex != -1)
            {
                var newState = AOTConfiguration.AotState.Default;
                if (newIndex == 0)
                    newState = AOTConfiguration.AotState.Enabled;
                else if (newIndex == 1)
                    newState = AOTConfiguration.AotState.Disabled;

                for (int i = 0; i < target.aotTypes.Count; ++i)
                {
                    var entry = target.aotTypes[i];
                    entry.State = newState;
                    target.aotTypes[i] = entry;
                }
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            foreach (AOTConfiguration.Entry entry in target.aotTypes)
            {
                Type resolvedType = TypeCache.GetType(entry.FullTypeName);
                EditorGUI.BeginDisabledGroup(resolvedType == null || string.IsNullOrEmpty(GetAotCompilationMessage(resolvedType)) == false);
                DrawType(entry, resolvedType);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif