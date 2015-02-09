using UnityEditor;
using UnityEngine;

namespace Assets.Editor.MarkerMetro
{
    public static class MarkerMetroTools
    {
        [MenuItem("Tools/MarkerMetro/Preferences/Player Prefs/Delete All", priority = 20)]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Tools/MarkerMetro/Preferences/Editor Prefs/Delete All", priority = 21)]
        public static void DeleteEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }
    }
}