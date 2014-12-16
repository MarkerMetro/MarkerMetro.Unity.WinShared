using UnityEngine;
using UnityEditor;
using System.Collections;
using BuildConfig = Assets.Editor.MarkerMetro.PluginConfigHelper.BuildConfig;
using PluginSource = Assets.Editor.MarkerMetro.PluginConfigHelper.PluginSource;

namespace Assets.Editor.MarkerMetro
{
    internal sealed class ConfigureWindow : EditorWindow
    {
        enum DirType
        {
            WinLegacy,
            WinIntegration,
            NuGet,
            BuildLocal
        }

        string _winLegacyDir;
        string _winIntegrationDir;
        string _nugetDir;
        string _buildLocalDir;
        PluginSource _pluginSource;
        BuildConfig _buildConfig;

        void OnEnable()
        {
            _pluginSource = (PluginSource)PluginConfigHelper.CurrentPluginSource;
            _buildConfig = (BuildConfig)PluginConfigHelper.CurrentBuildConfig;
            _winLegacyDir = PluginConfigHelper.WinLegacyDir;
            _winIntegrationDir = PluginConfigHelper.WinIntegrationDir;
            _nugetDir = PluginConfigHelper.NugetScriptsDir;
            _buildLocalDir = PluginConfigHelper.BuildLocalScriptsDir;
        }

        void OnGUI()
        {
            DrawUpdateSource();
            if (_pluginSource == PluginSource.Local)
            {
                DrawBuildConfig();
                DrawChooseDir(DirType.WinLegacy);
                DrawChooseDir(DirType.WinIntegration);
                DrawChooseDir(DirType.BuildLocal);
            }
            else
            {
                DrawChooseDir(DirType.NuGet);
            }
        }

        /// <summary>
        /// GUI for Update Source.
        /// </summary>
        void DrawUpdateSource()
        {
            GUILayout.Space(5f);
            GUI.changed = false;
            _pluginSource = (PluginSource)EditorGUILayout.EnumPopup("Plugin Source", _pluginSource, GUILayout.MaxWidth(250f));
            if (GUI.changed)
            {
                PluginConfigHelper.CurrentPluginSource = (int)_pluginSource;
            }
            GUILayout.Space(10f);
        }

        /// <summary>
        /// GUI for Build Config.
        /// </summary>
        void DrawBuildConfig()
        {
            GUI.changed = false;
            _buildConfig = (BuildConfig)EditorGUILayout.EnumPopup("Build Local", _buildConfig, GUILayout.MaxWidth(250f));
            if (GUI.changed)
            {
                PluginConfigHelper.CurrentBuildConfig = (int)_buildConfig;
            }
            GUILayout.Space(10f);
        }

        /// <summary>
        /// GUI for selecting directory.
        /// </summary>
        void DrawChooseDir(DirType dirType)
        {
            string dir = GetDir(dirType);

            EditorGUILayout.BeginVertical();
            GUILayout.Label(dirType.ToString() + " Dir:");
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            GUILayout.Space(4f);
            GUILayout.Label(dir, "AS TextArea", GUILayout.Height(20f));
            EditorGUILayout.EndVertical();
            GUILayout.Space(3f);
            if (GUILayout.Button("Choose", "LargeButtonMid", GUILayout.Height(20f), GUILayout.ExpandWidth(false)))
            {
                string defaultDir = string.IsNullOrEmpty(dir) ? Application.dataPath : dir;
                dir = EditorUtility.OpenFolderPanel("Choose Folder", defaultDir, "");
                if (!string.IsNullOrEmpty(dir))
                {
                    SetDir(dirType, dir);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Return cached dir based on DirType.
        /// </summary>
        string GetDir(DirType dirType)
        {
            string dir = string.Empty;
            switch (dirType)
            {
                case DirType.WinLegacy:
                    dir = _winLegacyDir;
                    break;
                case DirType.WinIntegration:
                    dir = _winIntegrationDir;
                    break;
                case DirType.NuGet:
                    dir = _nugetDir;
                    break;
                case DirType.BuildLocal:
                    dir = _buildLocalDir;
                    break;
            }
            return dir;
        }

        /// <summary>
        /// Store dir of DirType to EditorPrefs.
        /// </summary>
        void SetDir(DirType dirType, string dir)
        {
            dir = System.IO.Path.GetFullPath(dir);
            switch (dirType)
            {
                case DirType.WinLegacy:
                    _winLegacyDir = dir;
                    PluginConfigHelper.WinLegacyDir = _winLegacyDir;
                    break;
                case DirType.WinIntegration:
                    _winIntegrationDir = dir;
                    PluginConfigHelper.WinIntegrationDir = _winIntegrationDir;
                    break;
                case DirType.NuGet:
                    _nugetDir = dir;
                    PluginConfigHelper.NugetScriptsDir = _nugetDir;
                    break;
                case DirType.BuildLocal:
                    _buildLocalDir = dir;
                    PluginConfigHelper.BuildLocalScriptsDir = _buildLocalDir;
                    break;
            }
        }
    }
}