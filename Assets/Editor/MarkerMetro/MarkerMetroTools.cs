using UnityEditor;
using UnityEngine;

public static class MarkerMetroTools
{
    [MenuItem("MarkerMetro/Player Prefs/Delete All")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }


}
