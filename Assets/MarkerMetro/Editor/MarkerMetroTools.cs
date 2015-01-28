﻿using UnityEditor;
using UnityEngine;

namespace Assets.Editor.MarkerMetro
{
    public static class MarkerMetroTools
    {
        [MenuItem("MarkerMetro/Player Prefs/Delete All")]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}