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
        delegate void DrawToggle(string title, bool value, Action<bool> onUpdate, 
            bool bold = false, bool limitWidth = false);

        [MenuItem("Tools/MarkerMetro/Features", priority = 20)]
        static void Init()
        {
            EditorWindow.GetWindow<FeaturesWindow>(false, "Features", true).Show();
        }

        void OnGUI()
        {
            // Executes an action in case the toggle value changed:
            DrawToggle draw = (title, value, onUpdate, bold, limitWidth) =>
            {
                var toggle = EditorGUILayout.ToggleLeft(title, value, 
                    bold ? EditorStyles.boldLabel : EditorStyles.label,
                    limitWidth? GUILayout.MaxWidth(70f) : GUILayout.MaxWidth(1000f));

                if (toggle != value) 
                {
                    onUpdate(toggle);
                    // Makes Unity notice the file change:
                    AssetDatabase.Refresh();
                }
            };

            var fm = FeaturesManager.Instance;

            EditorGUILayout.LabelField("Windows Store and Windows Phone", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            draw("IAP Disclaimer", fm.IsIapDisclaimerEnabled,
                t => fm.IsIapDisclaimerEnabled = t);

            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Windows Store", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            draw("Settings - Music/FX On/Off", fm.IsSettingsMusicFXOnOffEnabled,
                t => fm.IsSettingsMusicFXOnOffEnabled = t);

            draw("Settings - Reminders On/Off", fm.IsSettingsNotificationsOnOffEnabled,
                t => fm.IsSettingsNotificationsOnOffEnabled = t);

            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;
            draw("Exception Logging", fm.IsExceptionLoggingEnabled,
                t => fm.IsExceptionLoggingEnabled = t, true);
            
            if (fm.IsExceptionLoggingEnabled)
            {
                EditorGUI.indentLevel++;

                string apiKey = EditorGUILayout.TextField("API Key", fm.ExceptionLoggingApiKey);
                if (apiKey != fm.ExceptionLoggingApiKey)
                {
                    fm.ExceptionLoggingApiKey = apiKey;
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Auto Log", GUILayout.Width(70f));
                
                Action<string, Environment> updateEnv = (title, env) =>
                    {
                        draw(title, fm.IsExceptionLoggingEnabledForEnvironment(env), t =>
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
                            }, false, true);
                    };

                updateEnv("Debug", Environment.Dev);
                updateEnv("QA", Environment.QA);
                updateEnv("Master", Environment.Production);

                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }
    }
}