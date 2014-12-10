using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;

public static class MarkerMetroPlugins
{
    /// <summary>
    /// Attempt to execute the Update_NuGet_Packages.bat file.
    /// Show any errors/success in a message box.
    /// </summary>
    [MenuItem("MarkerMetro/Plugins/Refresh from Nuget")]
    public static void RefreshFromNuget()
    {
        string cmdPath = "cmd.exe";
        string nugetUpdateDir = Application.dataPath + "\\..\\NuGet\\";
        string nugetUpdateFilename = "Update_NuGet_Packages.bat";

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
