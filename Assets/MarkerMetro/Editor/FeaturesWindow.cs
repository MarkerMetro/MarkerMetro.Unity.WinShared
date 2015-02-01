using System;
using UnityEditor;
using System.Collections;
using UnityEngine;

namespace MarkerMetro.Unity.WinShared.Tools
{
    /// <summary>
    /// Creates the Features window - where the developer can choose which Windows
    /// features he/she wants in the game - and adds a menu option to open it. 
    /// You have to build in Unity in order for changes to take effect.
    /// </summary>
    public class FeaturesWindow : EditorWindow
    {
        delegate void RefAction(ref bool preference);

        [MenuItem("Tools/MarkerMetro/Features")]
        static void Init()
        {
            EditorWindow.GetWindow<FeaturesWindow>(false, "Features", true).Show();
        }


        void OnGUI()
        {
            // Executes an action in case the toggle value changed:
            Action<string, bool, Action<bool>> update = (title, pref, action) =>
            {
                var toggle = EditorGUILayout.ToggleLeft(title, pref);
                if (toggle != pref)
                {
                    action(toggle);
                    // Makes Unity notice the file change:
                    AssetDatabase.Refresh();
                }
            };



            EditorGUILayout.LabelField("Windows Store and Windows Phone", EditorStyles.boldLabel);

            update("IAP Disclaimer", FeaturesManager.Instance.IsIapDisclaimerEnabled,
                t => FeaturesManager.Instance.IsIapDisclaimerEnabled = t);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Windows Store", EditorStyles.boldLabel);

            update("Settings - Music/FX On/Off", FeaturesManager.Instance.IsSettingsMusicFXOnOffEnabled,
                t => FeaturesManager.Instance.IsSettingsMusicFXOnOffEnabled = t);

            update("Settings - Reminders On/Off", FeaturesManager.Instance.IsSettingsNotificationsOnOffEnabled,
                t => FeaturesManager.Instance.IsSettingsNotificationsOnOffEnabled = t);
        }
    }
}