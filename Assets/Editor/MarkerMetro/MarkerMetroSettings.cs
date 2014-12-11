using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper class to Get/Set user config to EditorPrefs
/// </summary>
public static class MarkerMetroSettings
{
    public enum PluginSource
    {
        Nuget,
        Local
    }

    public enum BuildConfig
    {
        Debug,
        Release
    }

    /// <summary>
    /// Get/Set WinLegacy solution directory
    /// </summary>
    static public string WinLegacyDir
    {
        get { return EditorPrefs.GetString("MMWinLegacyDir", string.Empty); }
        set { EditorPrefs.SetString("MMWinLegacyDir", value); }
    }

    /// <summary>
    /// Get/Set WinIntegration solution directory
    /// </summary>
    static public string WinIntegrationDir
    {
        get { return EditorPrefs.GetString("MMWinIntegrationDir", string.Empty); }
        set { EditorPrefs.SetString("MMWinIntegrationDir", value); }
    }

    /// <summary>
    /// Get/Set Nuget scripts directory
    /// </summary>
    static public string NugetScriptsDir
    {
        get { return EditorPrefs.GetString("MMNugetScriptsDir", Application.dataPath + "\\..\\NuGet\\"); }
        set { EditorPrefs.SetString("MMNugetScriptsDir", value); }
    }

    /// <summary>
    /// Get/Set Nuget scripts filename
    /// </summary>
    static public string NugetScriptsFilename
    {
        get { return EditorPrefs.GetString("MMNugetScriptsFilename", "Update_NuGet_Packages.bat"); }
        set { EditorPrefs.SetString("MMNugetScriptsFilename", value); }
    }

    /// <summary>
    /// Get/Set current plugin update source
    /// </summary>
    static public int CurrentPluginSource
    {
        get { return EditorPrefs.GetInt("MMPluginSource", 0); }
        set { EditorPrefs.SetInt("MMPluginSource", value); }
    }

    /// <summary>
    /// Get/Set current build configuration
    /// </summary>
    static public int CurrentBuildConfig
    {
        get { return EditorPrefs.GetInt("MMBuildConfig", 0); }
        set { EditorPrefs.SetInt("MMBuildConfig", value); }
    }
}
