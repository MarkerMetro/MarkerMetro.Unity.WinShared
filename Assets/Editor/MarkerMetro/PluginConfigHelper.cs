using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Assets.Editor.MarkerMetro
{
    /// <summary>
    /// Helper class to Get/Set user config to EditorPrefs.
    /// </summary>
    internal static class PluginConfigHelper
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
        /// Get/Set WinLegacy solution directory.
        /// </summary>
        static public string WinLegacyDir
        {
            get { return EditorPrefs.GetString("MMWinLegacyDir", string.Empty); }
            set { EditorPrefs.SetString("MMWinLegacyDir", value); }
        }

        /// <summary>
        /// Get/Set WinIntegration solution directory.
        /// </summary>
        static public string WinIntegrationDir
        {
            get { return EditorPrefs.GetString("MMWinIntegrationDir", string.Empty); }
            set { EditorPrefs.SetString("MMWinIntegrationDir", value); }
        }

        static public string UnityPluginsDir
        {
            get { return System.IO.Path.GetFullPath(Application.dataPath + "\\Plugins"); }
        }

        /// <summary>
        /// Get/Set Nuget scripts directory.
        /// </summary>
        static public string NugetScriptsDir
        {
            get { return EditorPrefs.GetString("MMNugetScriptsDir", System.IO.Path.GetFullPath(Application.dataPath + "\\..\\NuGet")); }
            set { EditorPrefs.SetString("MMNugetScriptsDir", value); }
        }

        /// <summary>
        /// Get/Set Visual Studio common tools directory (for access to vsvars.bat file).
        /// </summary>
        static public string VSCommonToolDir
        {
            get { return EditorPrefs.GetString("MMVSCommonToolDir", SearchVSCommonToolsDir()); }
            set { EditorPrefs.SetString("MMVSCommonToolDir", value); }
        }

        /// <summary>
        /// Get/Set Nuget scripts filename.
        /// </summary>
        static public string NugetScriptsFilename
        {
            get { return "Update_NuGet_Packages.bat"; }
        }

        /// <summary>
        /// Get/Set Nuget scripts filename.
        /// </summary>
        static public string BuildLocalScriptsFilename
        {
            get { return "Build_Local.bat"; }
        }

        /// <summary>
        /// Get/Set current plugin update source.
        /// </summary>
        static public int CurrentPluginSource
        {
            get { return EditorPrefs.GetInt("MMPluginSource", 0); }
            set { EditorPrefs.SetInt("MMPluginSource", value); }
        }

        /// <summary>
        /// Get/Set current build configuration.
        /// </summary>
        static public int CurrentBuildConfig
        {
            get { return EditorPrefs.GetInt("MMBuildConfig", 0); }
            set { EditorPrefs.SetInt("MMBuildConfig", value); }
        }

        /// <summary>
        /// Attemp to search for the path of vsvars.bat for the highest version of Visual Studio installed.
        /// </summary>
        static public string SearchVSCommonToolsDir ()
        {
            List<Version> vsVersions = new List<Version>() { new Version("15.0"), new Version("14.0"), new Version("13.0"), new Version("12.0"), new Version("11.0") };

            foreach (var version in vsVersions)
            {
                string dir = Environment.GetEnvironmentVariable(string.Format("VS{0}{1}COMNTOOLS", version.Major, version.Minor));
                if (!string.IsNullOrEmpty(dir))
                {
                    return System.IO.Path.GetFullPath(dir.Substring(0, dir.Length-1));
                }
            }

            return null;
        }
    }
}