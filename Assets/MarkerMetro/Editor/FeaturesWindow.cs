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

        [MenuItem("Tools/MarkerMetro/Features", priority = 20)]
        static void Init()
        {
            EditorWindow.GetWindow<FeaturesWindow>(false, "Features", true).Show();
        }


        void OnGUI()
        {
            // Executes an action in case the toggle value changed:
            Action<string, bool, Action<bool>> update = (title, pref, action) =>
            {
                var toggle = EditorGUILayout.Toggle(title, pref);
                if (toggle != pref)
                {
                    action(toggle);
                    // Makes Unity notice the file change:
                    AssetDatabase.Refresh();
                }
            };


            update("IAP Disclaimer", FeaturesManager.Instance.IsIapEnabled,
                t => FeaturesManager.Instance.IsIapEnabled = t);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Charm Settings (Windows Store)", EditorStyles.boldLabel);

            update("Music & Sound", FeaturesManager.Instance.IsAudioCharmEnabled,
                t => FeaturesManager.Instance.IsAudioCharmEnabled = t);

            update("Reminders", FeaturesManager.Instance.IsRemindersEnabled,
                t => FeaturesManager.Instance.IsRemindersEnabled = t);
        }
    }
}