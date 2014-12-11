using UnityEngine;
using UnityEditor;
using System.Collections;
using BuildConfig = MarkerMetroSettings.BuildConfig;
using PluginSource = MarkerMetroSettings.PluginSource;

public class MarkerMetroConfigureWindow : EditorWindow
{
    enum DirType
    {
        WinLegacy,
        WinIntegration,
        NuGet
    }

    string _winLegacyDir;
    string _winIntegrationDir;
    string _nugetDir;
    PluginSource _pluginSource;
    BuildConfig _buildConfig;

    void OnEnable ()
    {
        _pluginSource = (PluginSource)MarkerMetroSettings.CurrentPluginSource;
        _buildConfig = (BuildConfig)MarkerMetroSettings.CurrentBuildConfig;
        _winLegacyDir = MarkerMetroSettings.WinLegacyDir;
        _winIntegrationDir = MarkerMetroSettings.WinIntegrationDir;
        _nugetDir = MarkerMetroSettings.NugetScriptsDir;
    }

    void OnGUI ()
    {
        DrawUpdateSource();
        if (_pluginSource == PluginSource.Local)
            DrawBuildConfig();
        DrawChooseDir(DirType.WinLegacy);
        DrawChooseDir(DirType.WinIntegration);
        DrawChooseDir(DirType.NuGet);
    }

    void DrawUpdateSource ()
    {
        GUILayout.Space(5f);
        GUI.changed = false;
        _pluginSource = (PluginSource)EditorGUILayout.EnumPopup("Plugin Source", _pluginSource, GUILayout.MaxWidth(250f));
        if (GUI.changed)
            MarkerMetroSettings.CurrentPluginSource = (int)_pluginSource;
        GUILayout.Space(10f);
    }

    void DrawBuildConfig ()
    {
        GUI.changed = false;
        _buildConfig = (BuildConfig)EditorGUILayout.EnumPopup("Build Local", _buildConfig, GUILayout.MaxWidth(250f));
        if (GUI.changed)
            MarkerMetroSettings.CurrentBuildConfig = (int)_buildConfig;
        GUILayout.Space(10f);
    }

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
            dir = EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(dir))
            {
                SetDir(dirType, dir);
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10f);
        EditorGUILayout.EndVertical();
    }

    string GetDir (DirType dirType)
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
        }
        return dir;
    }

    void SetDir (DirType dirType, string dir)
    {
        switch (dirType)
        {
            case DirType.WinLegacy:
                _winLegacyDir = dir;
                MarkerMetroSettings.WinLegacyDir = _winLegacyDir;
                break;
            case DirType.WinIntegration:
                _winIntegrationDir = dir;
                MarkerMetroSettings.WinIntegrationDir = _winIntegrationDir;
                break;
            case DirType.NuGet:
                _nugetDir = dir;
                MarkerMetroSettings.NugetScriptsDir = _nugetDir;
                break;
        }
    }
}
