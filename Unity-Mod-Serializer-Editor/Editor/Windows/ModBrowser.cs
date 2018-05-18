using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UMS.Editor.Windows
{
    public class ModBrowser : EditorWindow
    {
        private const string ROOT = "Modding/File Browser";

        private static string DefaultFolder
        {
            get
            {
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            }
        }

        [MenuItem(ROOT, priority = Utility.MENU_ITEM_PRIORITY)]
        private static void BrowseFile()
        {
            string filePath = EditorUtility.OpenFilePanelWithFilters("Browse Mod File", DefaultFolder, new string[2] { "Mod Files", "mod" });

            if(filePath != string.Empty && filePath != null)
            {
                try
                {
                    Serializer.Initialize();
                    ModFile file = ModFile.Load(filePath);

                    CreateWindow(file);
                }
                catch (System.Exception)
                {
                    Debug.LogError("Issue loading mod file " + filePath);
                    throw;
                }
            }
        }
        public static void CreateWindow(ModFile file)
        {
            ModBrowser browser = GetWindow<ModBrowser>(true, string.Format("Mod Browser ({0})", file.FileName), true);
            browser.LoadFile(file);
            browser.Show();
            browser.minSize = _minSize;
        }
        public void LoadFile(ModFile file)
        {
            _file = file;

            //_ids = new List<ushort>(_file.IDs);
            _hierarchy = new ModBrowserTreeView();

            if (_ids.Count > 0)
                SelectEntry(_ids[0]);
        }
        
        private ModFile _file;
        private Vector2 _listScrollPos;
        private float _listWidth = 200;
        private ushort _selectedID;
        private Styles _styles = new Styles();
        private List<ushort> _ids;
        private ModBrowserTreeView _hierarchy;

        private const float PADDING = 3;
        private static readonly Vector2 _minSize = new Vector2(500, 300);

        private void OnGUI()
        {
            if (_file == null)
            {
                EditorGUILayout.LabelField("File is null");
                return;
            }
            
            DrawInspector();
            DrawList();
        }
        private void DrawInspector()
        {
            Rect inspectorRect = new Rect(_listWidth + PADDING, 0, position.width - (_listWidth + PADDING), position.height);

            _hierarchy.OnGUI(inspectorRect);
        }
        private void SelectEntry(ushort id)
        {
            _selectedID = id;
            ModFile.Entry entry = _file[_selectedID];

            _hierarchy.ActiveObject = entry;
            _hierarchy.Reload();
        }
        private void DrawList()
        {
            Rect listRect = new Rect(0, 0, _listWidth, position.height);
            Rect contentRect = GetListContentRect();

            if(Event.current.type == EventType.Repaint)
            {
                _styles.Box.Draw(listRect, string.Empty, false, false, false, false);
            }
            
            _listScrollPos = GUI.BeginScrollView(listRect, _listScrollPos, contentRect);
            
            for (int i = 0; i < _ids.Count; i++)
            {
                Rect itemRect = new Rect(1, i * EditorGUIUtility.singleLineHeight, contentRect.width, EditorGUIUtility.singleLineHeight);
                Rect worldRect = GetWorldRect(i, listRect);
                Event currentEvent = Event.current;

                ushort id = _ids[i];
                bool isSelected = id == _selectedID;
                ModFile.Entry entry = _file[id];

                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && worldRect.Contains(currentEvent.mousePosition))
                {
                    SelectEntry(id);

                    Repaint();
                }
                if (currentEvent.type == EventType.Repaint)
                {
                    // Draw the background
                    GUIStyle backgroundStyle = i % 2 == 0 ? _styles.OddBackground : _styles.EvenBackground;
                    backgroundStyle.Draw(itemRect, false, false, isSelected, false);

                    //Draw the text
                    string text = GetListItemText(id);
                    EditorGUI.LabelField(worldRect, new GUIContent(text));
                }
            }

            GUI.EndScrollView();
        }
        private string GetListItemText(ushort id)
        {
            ModFile.Entry entry = _file[id];
            List<string> information = new List<string>();

            information.Add(id.ToString());

            //if (entry.Data.IsDictionary)
            //{
            //    Dictionary<string, Data> dictionary = entry.Data.Dictionary;
            //    string nameKey = "name";

            //    if (dictionary.ContainsKey(nameKey))
            //    {
            //        Data nameData = dictionary[nameKey];

            //        if (nameData.IsString)
            //        {
            //            information.Add(nameData.String);
            //        }                    
            //    }
            //}            

            //TypeMetaData typeMetaData = entry.Data.GetMetaData<TypeMetaData>();
            //if (typeMetaData != null)
            //{
            //    information.Add($"({typeMetaData.Type.Name})");
            //}

            return string.Join(" ", information);
        }
        private Rect GetWorldRect(int i, Rect listRect)
        {
            return new Rect()
            {
                x = listRect.x,
                y = listRect.y + EditorGUIUtility.singleLineHeight * i,
                width = listRect.width,
                height = EditorGUIUtility.singleLineHeight,
            };
        }
        private Rect GetTextContentRect(string text)
        {
            return new Rect(Vector2.zero, _styles.MessageStyle.CalcSize(new GUIContent(text)));
        }
        private Rect GetListContentRect()
        {
            return Rect.zero;
            //return new Rect(0, 0, _listWidth, _file.IDs.Count() * EditorGUIUtility.singleLineHeight);
        }

        private class Styles
        {
            public Styles()
            {
                Box = new GUIStyle("CN Box");
                EvenBackground = new GUIStyle("CN EntryBackEven");
                OddBackground = new GUIStyle("CN EntryBackodd");
                MessageStyle = new GUIStyle("CN Message");
            }

            public GUIStyle Box;
            public GUIStyle EvenBackground;
            public GUIStyle OddBackground;
            public GUIStyle MessageStyle;
        }
    }
}
