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
        FeaturesManager fm = FeaturesManager.Instance;

        [MenuItem("Tools/MarkerMetro/Features", priority = 20)]
        static void Init()
        {
            EditorWindow.GetWindow<FeaturesWindow>(false, "Features", true).Show();
        }

        void DrawToggle (string title, bool value, Action<bool> onUpdate, 
            bool bold = false, bool limitWidth = false)
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
            }

        void OnGUI()
        {

            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            DrawToggle("IAP Disclaimer (One Time)", fm.IsIapDisclaimerEnabled,
                t => fm.IsIapDisclaimerEnabled = t);

            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Settings (Windows)", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            DrawToggle("Music/FX Control", fm.IsSettingsMusicFXControlEnabled,
                t => fm.IsSettingsMusicFXControlEnabled = t);

            DrawToggle("Notifications Control", fm.IsSettingsNotificationsControlEnabled,
                t => fm.IsSettingsNotificationsControlEnabled = t);

            EditorGUI.indentLevel--;

            DrawExceptionSettings();
            DrawMemoryDisplaySettings();
        }

        void DrawExceptionSettings()
        {
            EditorGUILayout.Separator();
            DrawToggle("Exception Logging", fm.IsExceptionLoggingEnabled,
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
                    DrawToggle(title, fm.IsExceptionLoggingEnabledForEnvironment(env), t =>
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

        void DrawMemoryDisplaySettings()
        {
            EditorGUILayout.Separator();
            DrawToggle("Display Memory Usage (Windows Phone)", fm.IsMemoryDisplayEnabled,
                t => fm.IsMemoryDisplayEnabled = t, true);

            if (fm.IsMemoryDisplayEnabled)
            {
                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal();

                Action<string, Environment> updateEnv = (title, env) =>
                {
                    DrawToggle(title, fm.IsMemoryDisplayEnabledForEnvironment(env), t =>
                    {
                        var envs = new HashSet<Environment>(fm.MemoryDisplayEnvironments);
                        if (t)
                        {
                            envs.Add(env);
                        }
                        else
                        {
                            envs.Remove(env);
                        }
                        fm.MemoryDisplayEnvironments = envs;
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