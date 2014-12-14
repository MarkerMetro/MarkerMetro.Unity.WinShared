using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;
using PluginSource = Assets.Editor.MarkerMetro.PluginConfigHelper.PluginSource;
using BuildConfig = Assets.Editor.MarkerMetro.PluginConfigHelper.BuildConfig;

namespace Assets.Editor.MarkerMetro
{
    public static class PluginUpdateTool
    {
        /// <summary>
        /// Configure Plugin update source, local WinLegacy/WinIntegration solution directories.
        /// </summary>
        [MenuItem("MarkerMetro/Plugins/Configure", priority = 1)]
        public static void Configure()
        {
            EditorWindow.GetWindow<ConfigureWindow>(false, "Configure", true).Show();
        }

        /// <summary>
        /// Update Plugins.
        /// </summary>
        [MenuItem("MarkerMetro/Plugins/Update", priority = 2)]
        public static void UpdatePlugins()
        {
            if ((PluginSource)PluginConfigHelper.CurrentPluginSource == PluginSource.Nuget)
            {
                UpdateFromNuGet();
            }
            else
            {
                UpdateLocal();
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Attempt to execute the Update_NuGet_Packages.bat file.
        /// Show any errors/success in a message box.
        /// </summary>
        static void UpdateFromNuGet()
        {
            var cmdPath = "cmd.exe";
            string dir = PluginConfigHelper.NugetScriptsDir;
            string batchFilename = PluginConfigHelper.NugetScriptsFilename;

            Process process = null;

            try
            {
                process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = cmdPath;
                process.StartInfo.WorkingDirectory = dir;
                process.StartInfo.Arguments = "/c " + batchFilename;
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
                {
                    process.Close();
                }
            }
        }

        /// <summary>
        /// Attemp to build the csproj file using MSBuild exe call 
        /// and copy the resultant dlls to the appropriate plugins folders.
        /// </summary>
        static void UpdateLocal()
        {
            BuildPlugin();
        }

        static void BuildPlugin()
        {
            if (string.IsNullOrEmpty(PluginConfigHelper.WinLegacyDir) || string.IsNullOrEmpty(PluginConfigHelper.WinIntegrationDir))
            {
                EditorUtility.DisplayDialog("Build Plugin", "Error: Failed to get plugin directory.", "OK");
                return;
            }

            var cmdPath = "cmd.exe";
            string dir = PluginConfigHelper.NugetScriptsDir;
            var batchFilename = "Build_Local.bat";

            Process process = null;

            try
            {
                process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = cmdPath;
                process.StartInfo.WorkingDirectory = dir;
                process.StartInfo.Arguments = "/c " + batchFilename + " " + PluginConfigHelper.WinLegacyDir + " " + PluginConfigHelper.WinIntegrationDir +
                    " " + PluginConfigHelper.UnityPluginsDir + " " + ((BuildConfig)PluginConfigHelper.CurrentBuildConfig).ToString();
                process.EnableRaisingEvents = true;
                process.Start();

                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode == 0)
                {
                    EditorUtility.DisplayDialog("Build Plugin", "Build Plugin completed.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Build Plugin", "Build Plugin failed with exit code: " + exitCode + ".", "OK");
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Build Plugin", "Exception: " + e, "OK");
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
        }
    }
}