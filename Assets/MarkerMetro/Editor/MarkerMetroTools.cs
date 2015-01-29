using UnityEditor;
using UnityEngine;

namespace Assets.Editor.MarkerMetro
{
    public static class MarkerMetroTools
    {
        [MenuItem("Tools/MarkerMetro/Player Prefs/Delete All")]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}