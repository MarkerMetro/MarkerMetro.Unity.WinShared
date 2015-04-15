using UnityEditor;
using UnityEngine;

namespace MarkerMetro.Unity.WinShared.Editor
{
    /// <summary>
    /// This class contains methods of general purpose.
    /// </summary>
    public static class MarkerMetroTools
    {
        [MenuItem("Tools/Marker Metro/Preferences/Player Prefs/Delete All", priority = 20)]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Tools/Marker Metro/Preferences/Editor Prefs/Delete All", priority = 21)]
        public static void DeleteEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }
    }
}