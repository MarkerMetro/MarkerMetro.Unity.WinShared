using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;

public static class MarkerMetroPlugins
{
    /// <summary>
    /// Configure local WinLegacy/WinIntegration solution directories
    /// </summary>
    [MenuItem("MarkerMetro/Plugins/Configure", priority = 1)]
    public static void Configure()
    {
        EditorWindow.GetWindow<MarkerMetroConfigureWindow>(false, "Configure", true).Show();
    }

    /// <summary>
    /// Build local WinLegacy/WinIntegration and copy the dlls to the appropriate plugin folders
    /// </summary>
    [MenuItem("MarkerMetro/Plugins/Build Local", priority = 2)]
    public static void BuildLocal()
    {

    }

    /// <summary>
    /// Attempt to execute the Update_NuGet_Packages.bat file.
    /// Show any errors/success in a message box.
    /// </summary>
    [MenuItem("MarkerMetro/Plugins/Refresh from Nuget", priority = 3)]
    public static void RefreshFromNuget()
    {
        string cmdPath = "cmd.exe";
        string nugetUpdateDir = MarkerMetroSettings.NugetScriptsDir;
        string nugetUpdateFilename = MarkerMetroSettings.NugetScriptsFilename;

        Process process = null;

        try {
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
                EditorUtility.DisplayDialog("Refresh from Nuget", "Refresh from Nuget completed.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Refresh from Nuget", "Refresh from Nuget failed with exit code: " + exitCode, "OK");
            }
        } 
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Refresh from Nuget", "Exception: " + e, "OK");
        }
        finally
        {
            if (process != null)
                process.Close();
        }
    }
}
