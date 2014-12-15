using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;
using PluginSource = Assets.Editor.MarkerMetro.PluginConfigHelper.PluginSource;
using BuildConfig = Assets.Editor.MarkerMetro.PluginConfigHelper.BuildConfig;

namespace Assets.Editor.MarkerMetro
{
    internal static class PluginUpdateTool
    {
        const float ExpectedUpdateTime = 15f;

        static Process CmdProcess;
        static float UpdateStartTime;
        static string ErrorMessage;

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
            if (CmdProcess != null)
            {
                DisplayDialog("Error: Still updating");
                return;
            }

            UpdateStartTime = (float)EditorApplication.timeSinceStartup;

            if ((PluginSource)PluginConfigHelper.CurrentPluginSource == PluginSource.Nuget)
            {
                UpdatePlugins(true);
            }
            else
            {
                if (string.IsNullOrEmpty(PluginConfigHelper.WinLegacyDir) || string.IsNullOrEmpty(PluginConfigHelper.WinIntegrationDir))
                {
                    DisplayDialog("Error: Failed to get plugin directory.");
                    return;
                }
                else
                {
                    UpdatePlugins(false);
                }
            }
        }

        /// <summary>
        /// Attempt to execute the Update_NuGet_Packages.bat file, or attemp to build the csproj file using MSBuild exe call 
        /// and copy the resultant dlls to the appropriate plugins folders.
        /// Show any errors/success in a message box.
        /// </summary>
        static void UpdatePlugins(bool fromNuGet)
        {
            var cmdPath = "cmd.exe";
            string dir = PluginConfigHelper.NugetScriptsDir;
            string batchFilename = fromNuGet ? PluginConfigHelper.NugetScriptsFilename : "Build_Local.bat";

            try
            {
                CmdProcess = new Process();
                CmdProcess.StartInfo.CreateNoWindow = true;
                CmdProcess.StartInfo.UseShellExecute = false;
                CmdProcess.StartInfo.FileName = cmdPath;
                CmdProcess.StartInfo.WorkingDirectory = dir;
                CmdProcess.StartInfo.Arguments = "/c " + batchFilename;
                if (!fromNuGet)
                {
                    CmdProcess.StartInfo.Arguments = CmdProcess.StartInfo.Arguments + " " + PluginConfigHelper.WinLegacyDir + " " + PluginConfigHelper.WinIntegrationDir +
                    " " + PluginConfigHelper.UnityPluginsDir + " " + ((BuildConfig)PluginConfigHelper.CurrentBuildConfig).ToString();
                }
                CmdProcess.StartInfo.RedirectStandardOutput = true;
                CmdProcess.EnableRaisingEvents = true;

                CmdProcess.OutputDataReceived += ProcessOutputDataReceived;
                EditorApplication.update += UpdateProgressBar;

                CmdProcess.Start();
                CmdProcess.BeginOutputReadLine();
            }
            catch (Exception e)
            {
                DisplayDialog("Exception: " + e);
                if (CmdProcess != null)
                {
                    CmdProcess.Close();
                    CmdProcess = null;
                }
            }
        }

        /// <summary>
        /// Display a progress bar while updating.
        /// </summary>
        static void UpdateProgressBar ()
        {
            if (CmdProcess == null || CmdProcess.HasExited)
            {
                UpdateEnded();

                if (CmdProcess != null)
                {
                    int exitCode = CmdProcess.ExitCode;
                    if (exitCode == 0)
                    {
                        DisplayDialog("Update Plugins completed.");
                    }
                    else
                    {
                        DisplayDialog("Update Plugins failed with exit code: " + exitCode);
                    }

                    CmdProcess.Close();
                    CmdProcess = null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        DisplayDialog(ErrorMessage);
                    }
                }
            }
            else
            {
                float progress = ((float)EditorApplication.timeSinceStartup - UpdateStartTime) / ExpectedUpdateTime;
                EditorUtility.DisplayProgressBar("Updating", "Updating plugins", progress);
            }
        }

        /// <summary>
        /// Remove progress bar and refresh AssetDatabase when update finishes.
        /// </summary>
        static void UpdateEnded()
        {
            EditorApplication.update -= UpdateProgressBar;
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Determine update success or fail from process output.
        /// </summary>
        static void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Contains(" error "))
            {
                ErrorMessage = e.Data;
                CmdProcess.Close();
                CmdProcess = null;
            }
        }

        static void DisplayDialog(string message)
        {
            EditorUtility.DisplayDialog("UpdatePlugins", message, "OK");
        }
    }
}