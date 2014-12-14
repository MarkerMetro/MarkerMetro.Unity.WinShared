using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;
using PluginSource = Assets.Editor.MarkerMetro.EditorPrefsHelper.PluginSource;
using BuildConfig = Assets.Editor.MarkerMetro.EditorPrefsHelper.BuildConfig;

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
            if ((PluginSource)EditorPrefsHelper.CurrentPluginSource == PluginSource.Nuget)
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
            string dir = EditorPrefsHelper.NugetScriptsDir;
            string batchFilename = EditorPrefsHelper.NugetScriptsFilename;

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
                    process.Close();
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
            if (string.IsNullOrEmpty(EditorPrefsHelper.WinLegacyDir) || string.IsNullOrEmpty(EditorPrefsHelper.WinIntegrationDir))
            {
                EditorUtility.DisplayDialog("Build Plugin", "Error: Failed to get plugin directory.", "OK");
                return;
            }

            string cmdPath = "cmd.exe";
            string dir = EditorPrefsHelper.NugetScriptsDir;
            string batchFilename = "Build_Local.bat";

            Process process = null;

            try
            {
                process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = cmdPath;
                process.StartInfo.WorkingDirectory = dir;
                process.StartInfo.Arguments = "/c " + batchFilename + " " + EditorPrefsHelper.WinLegacyDir + " " + EditorPrefsHelper.WinIntegrationDir + " " + ((BuildConfig)EditorPrefsHelper.CurrentBuildConfig).ToString();
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
                    process.Close();
            }
        }

        static void BuildPlugin(string pluginName)
        {
            string cmdPath = "cmd.exe";
            string dir = GetPluginDir(pluginName);
            if (string.IsNullOrEmpty(dir))
            {
                EditorUtility.DisplayDialog("Build " + pluginName, "Error: Failed to get plugin directory.", "OK");
                return;
            }
            string batchFilename = "Build.bat";

            Process process = null;

            try
            {
                process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = cmdPath;
                process.StartInfo.WorkingDirectory = dir;
                process.StartInfo.Arguments = "/c " + batchFilename + " " + ((BuildConfig)EditorPrefsHelper.CurrentBuildConfig).ToString();
                process.EnableRaisingEvents = true;
                process.Start();

                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode == 0)
                {
                    EditorUtility.DisplayDialog("Build " + pluginName, "Build " + pluginName + " completed.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Build " + pluginName, "Build " + pluginName +" failed with exit code: " + exitCode + ".", "OK");
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Build " + pluginName, "Exception: " + e, "OK");
            }
            finally
            {
                if (process != null)
                    process.Close();
            }
        }

        static string GetPluginDir (string pluginName)
        {
            if (pluginName == "WinLegacy")
            {
                return EditorPrefsHelper.WinLegacyDir;
            }
            else if (pluginName == "WinIntegration")
            {
                return EditorPrefsHelper.WinIntegrationDir;
            }
            return null;
        }
    }
}