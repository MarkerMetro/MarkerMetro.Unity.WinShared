using System;
using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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
            Action<string, bool, Action<bool>, bool> update = (title, pref, action, bold) =>
            {
                var toggle = EditorGUILayout.ToggleLeft(title, pref, bold ?
                    EditorStyles.boldLabel : EditorStyles.label);
                if (toggle != pref)
                {
                    action(toggle);
                    // Makes Unity notice the file change:
                    AssetDatabase.Refresh();
                }
            };

            var fm = FeaturesManager.Instance;

            EditorGUILayout.LabelField("Windows Store and Windows Phone", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            update("IAP Disclaimer", fm.IsIapDisclaimerEnabled,
                t => fm.IsIapDisclaimerEnabled = t, false);

            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Windows Store", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            update("Settings - Music/FX On/Off", fm.IsSettingsMusicFXOnOffEnabled,
                t => fm.IsSettingsMusicFXOnOffEnabled = t, false);

            update("Settings - Reminders On/Off", fm.IsSettingsNotificationsOnOffEnabled,
                t => fm.IsSettingsNotificationsOnOffEnabled = t, false);

            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;
            update("Exception Logging", fm.IsExceptionLoggingEnabled,
                t => fm.IsExceptionLoggingEnabled = t, true);
            
            if (fm.IsExceptionLoggingEnabled)
            {
                EditorGUI.indentLevel++;

                string apiKey = EditorGUILayout.TextField("API Key", fm.ExceptionLoggingApiKey);
                if (apiKey != fm.ExceptionLoggingApiKey)
                {
                    fm.ExceptionLoggingApiKey = apiKey;
                }

                EditorGUILayout.LabelField("Auto Log");
                EditorGUI.indentLevel++;
                Action<string, Environment> updateEnv = (title, env) =>
                    {
                        update(title, fm.IsExceptionLoggingEnabledForEnvironment(env), t =>
                            {
                                var envs = new HashSet<Environment>(fm.ExceptionLoggingAutoLogEnvironments);
                                if (t)
                                {
                                    envs.Add(env);
                                }
                                else
                                {
                                    envs.Remove(env);
                                }
                                fm.ExceptionLoggingAutoLogEnvironments = envs;
                            }, false);
                    };

                updateEnv("Debug", Environment.Dev);
                updateEnv("QA", Environment.QA);
                updateEnv("Master", Environment.Production);

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
        }
    }
}