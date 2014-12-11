using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;
using PluginSource = Assets.Editor.MarkerMetro.MarkerMetroSettings.PluginSource;

namespace Assets.Editor.MarkerMetro
{
    public static class MarkerMetroPlugins
    {
        /// <summary>
        /// Configure Plugin update source, local WinLegacy/WinIntegration solution directories
        /// </summary>
        [MenuItem("MarkerMetro/Plugins/Configure", priority = 1)]
        public static void Configure()
        {
            EditorWindow.GetWindow<MarkerMetroConfigureWindow>(false, "Configure", true).Show();
        }

        /// <summary>
        /// Update Plugins
        /// </summary>
        [MenuItem("MarkerMetro/Plugins/Update", priority = 2)]
        public static void UpdatePlugins()
        {
            if ((PluginSource)MarkerMetroSettings.CurrentPluginSource == PluginSource.Nuget)
            {
                UpdateFromNuGet();
            }
            else
            {
                UpdateLocal();
            }
        }

        /// <summary>
        /// Attempt to execute the Update_NuGet_Packages.bat file.
        /// Show any errors/success in a message box.
        /// </summary>
        static void UpdateFromNuGet()
        {
            string cmdPath = "cmd.exe";
            string nugetUpdateDir = MarkerMetroSettings.NugetScriptsDir;
            string nugetUpdateFilename = MarkerMetroSettings.NugetScriptsFilename;

            Process process = null;

            try
            {
                process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = cmdPath;
                process.StartInfo.WorkingDirectory = nugetUpdateDir;
                process.StartInfo.Arguments = "/c " + nugetUpdateFilename;
                process.EnableRaisingEvents = true;
                process.Start();

                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode == 0)
                {
                    EditorUtility.DisplayDialog("Update from NuGet", "Update from NuGet completed.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Update from NuGet", "Update from NuGet failed with exit code: " + exitCode, "OK");
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Update from NuGet", "Exception: " + e, "OK");
            }
            finally
            {
                if (process != null)
                    process.Close();
            }
        }

        /// <summary>
        /// Attemp to build the csproj file using MSBuild exe call 
        /// and copy the resultant dlls to the appropriate plugins folders
        /// </summary>
        static void UpdateLocal()
        {
            // TODO
        }
    }
}