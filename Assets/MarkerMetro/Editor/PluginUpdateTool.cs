using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;
using PluginSource = MarkerMetro.Unity.WinShared.Editor.PluginConfigHelper.PluginSource;
using PluginBuildConfig = MarkerMetro.Unity.WinShared.Editor.PluginConfigHelper.PluginBuildConfig;

namespace MarkerMetro.Unity.WinShared.Editor
{
    internal static class PluginUpdateTool
    {
        const float TimeoutTime = 120f;
        const float ExpectedUpdateTime = 15f;
        const int NumberOfNuGetInstalls = 3;

        static Process CmdProcess;
        static float UpdateStartTime;
        static float Progress;
        static string ErrorMessage;

        /// <summary>
        /// Configure Plugin update source, local WinLegacy/WinIntegration solution directories.
        /// </summary>
        [MenuItem("Tools/Marker Metro/Plugins/Configure", priority = 11)]
        public static void Configure()
        {
            EditorWindow.GetWindow<ConfigureWindow>(false, "Configure", true).Show();
        }

        /// <summary>
        /// Update Plugins.
        /// </summary>
        [MenuItem("Tools/Marker Metro/Plugins/Update", priority = 12)]
        public static void UpdatePlugins()
        {
            if (EditorApplication.isCompiling)
            {
                DisplayDialog("Plugins cannot be updated at present, please try again later.");
                return;
            }

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
            string dir = PluginConfigHelper.BuildScriptsDir;
            string batchFilename = fromNuGet ? PluginConfigHelper.NugetScriptsFilename : PluginConfigHelper.BuildLocalScriptsFilename;

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
                    " " + PluginConfigHelper.UnityPluginsDir + " " + ((PluginBuildConfig)PluginConfigHelper.CurrentBuildConfig).ToString();
                }
                else
                {
                    UnityEngine.Debug.Log(PluginConfigHelper.UnityPluginsDir);
                    CmdProcess.StartInfo.Arguments = CmdProcess.StartInfo.Arguments + " " + PluginConfigHelper.UnityPluginsDir;
                }
                CmdProcess.StartInfo.RedirectStandardOutput = true;
                CmdProcess.EnableRaisingEvents = true;

                CmdProcess.OutputDataReceived += ProcessOutputDataReceived;
                CmdProcess.ErrorDataReceived += ProcessErrorDataReceived;
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
                UpdateEnded();
            }
        }

        /// <summary>
        /// Display a progress bar while updating.
        /// </summary>
        static void UpdateProgressBar()
        {
            // Stop updating if Unity is compiling.
            if (EditorApplication.isCompiling)
            {
                DisplayDialog("Plugins cannot be updated at present, please try again later.");
                UpdateEnded();
                return;
            }

            if (CmdProcess == null || CmdProcess.HasExited)
            {
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
                UpdateEnded();
            }
            else
            {
                if ((float)EditorApplication.timeSinceStartup - UpdateStartTime > TimeoutTime)
                {
                    CmdProcess.Close();
                    CmdProcess = null;

                    DisplayDialog("Update Plugins timed out, please check Unity console for more info.");
                    UpdateEnded();
                }
                else
                {
                    EditorUtility.DisplayProgressBar("Updating", "Updating plugins", Progress);
                }
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
        /// Determine progress from process output and attempt to catch errors.
        /// </summary>
        static void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.StartsWith("progress"))
                {
                    string[] progressEcho = e.Data.Split(' ');
                    if (progressEcho.Length == 2 && float.TryParse(progressEcho[1], out Progress))
                    {
                        Progress *= 0.01f;
                    }
                    else
                    {
                        global::UnityEngine.Debug.Log(e.Data);
                    }
                }
                else
                {
                    global::UnityEngine.Debug.Log(e.Data);

                    if (e.Data.Contains("Successfully installed"))
                    {
                        Progress += 1f / NumberOfNuGetInstalls;
                    }

                }

                if (e.Data.Contains(" error "))
                {
                    ErrorMessage = e.Data;
                    CmdProcess.Close();
                    CmdProcess = null;
                }
            }
        }

        /// <summary>
        /// Log any error from the process.
        /// </summary>
        static void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            global::UnityEngine.Debug.LogError(e.Data);
            ErrorMessage = e.Data;
            CmdProcess.Close();
            CmdProcess = null;
        }

        static void DisplayDialog(string message)
        {
            EditorUtility.DisplayDialog("Update Plugins", message, "OK");
        }
    }
}