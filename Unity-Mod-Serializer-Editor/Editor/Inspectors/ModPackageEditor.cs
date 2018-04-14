using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UMS.Editor.Inspectors
{
    [CustomEditor(typeof(ModPackage))]
    [CanEditMultipleObjects()]
    public class ModPackageEditor : UnityEditor.Editor
    {
        protected const float SERIALIZE_BUTTON_WIDTH = 200;

        protected ModPackage Target { get { return (ModPackage)target; } }
        protected ModPackageReorderableList _objectEntryList;

        private static readonly string _desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        private SerializedProperty _includeInBuildsProperty;

        protected virtual void OnEnable()
        {
            _objectEntryList = CreateList("_objectEntries");

            _includeInBuildsProperty = serializedObject.FindProperty("_includeInBuilds");
        }
        public override void OnInspectorGUI()
        {
            GUI.changed = false;

            if (Selection.objects.Length == 1)
            {
                EditorGUILayout.Space();
                _objectEntryList.Draw();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            DrawSettings();
            EditorGUILayout.Space();

            EditorGUILayout.Space();
            DrawSerializeButton();
            DrawDeserializeButton();
            EditorGUILayout.Space();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(target);
            }
        }
        protected virtual void DrawSettings()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_includeInBuildsProperty, new GUIContent("Include In Builds"));
        }
        protected virtual void DrawSerializeButton()
        {
            GUIContent buttonText = new GUIContent("Serialize", "Serialize this package. To serialize all packages use Modding/Serialize All");
            GUIStyle buttonStyle = EditorStyles.largeLabel;

            Rect rect = GUILayoutUtility.GetRect(buttonText, buttonStyle);
            rect.x = (rect.width - SERIALIZE_BUTTON_WIDTH) / 2;
            rect.width = SERIALIZE_BUTTON_WIDTH;

            if (GUI.Button(rect, buttonText))
            {
                foreach (ModPackage package in Selection.objects)
                {
                    package.SaveToDesktop();
                }
            }
        }
        private void DrawDeserializeButton()
        {
            GUIContent buttonText = new GUIContent("Deserialize", "Deserializes package from Desktop. To deserialize all packages use Modding/Deserialize Desktop");
            GUIStyle buttonStyle = EditorStyles.largeLabel;

            Rect rect = GUILayoutUtility.GetRect(buttonText, buttonStyle);
            rect.x = (rect.width - SERIALIZE_BUTTON_WIDTH) / 2;
            rect.width = SERIALIZE_BUTTON_WIDTH;

            EditorGUI.BeginDisabledGroup(!CanDeserialize());
            if (GUI.Button(rect, buttonText))
            {
                string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
                string[] fileNamesOnDesktop = Directory.GetFiles(folderPath);

                foreach (ModPackage package in Selection.objects)
                {
                    string fullpath = string.Format("{0}/{1}", folderPath, package.FileName);

                    ModPackage.Load(fullpath);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        private bool CanDeserialize()
        {
            string[] fileNamesOnDesktop = Directory.GetFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));

            foreach (ModPackage package in Selection.objects)
            {
                if (fileNamesOnDesktop.Any(x => Path.GetFileName(x) == package.FileName))
                    return true;
            }

            return false;
        }
        protected virtual ModPackageReorderableList CreateList(string propertyName)
        {
            return new ModPackageReorderableList(Target, serializedObject, serializedObject.FindProperty(propertyName));
        }
    }
}
