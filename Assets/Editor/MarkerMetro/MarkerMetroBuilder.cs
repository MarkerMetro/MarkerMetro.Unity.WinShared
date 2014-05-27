using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

public static class MarkerMetroBuilder
{
    private const string settingsFile = "../BuildSettings.xml";

    [MenuItem("MarkerMetro/Build.../All")]
    public static void BuildAll()
    {
        BuildMetro();
        BuildWP8();
    }

    [MenuItem("MarkerMetro/Build.../Windows Store Apps")]
    public static void BuildMetro()
    {
        Build(BuildTarget.MetroPlayer, 
            MarkerMetro.CommandLineReader.GetCustomArgument("outputPath"),
            () =>
            {
                EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Metro;
                EditorUserBuildSettings.metroBuildType = MetroBuildType.VisualStudioCSharp;
                EditorUserBuildSettings.metroSDK = MetroSDK.SDK81;
            });
    }

    [MenuItem("MarkerMetro/Build.../Windows Phone 8")]
    public static void BuildWP8()
    {
        Build(BuildTarget.WP8Player,
            MarkerMetro.CommandLineReader.GetCustomArgument("outputPath"),
            () =>
            {
                EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WP8;
            });
    }

    public static void Build(BuildTarget target, 
        string outputPath = null, 
        Action beforeBuild = null)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("You shouldn't call this while playing.");
            return;
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(target);

        string[] scenes = EditorBuildSettings.scenes
            .Where(s=>s.enabled)
            .Select(s=>s.path)
            .ToArray();
        
        try
        {
            string path  = GetPath(target);
            outputPath = path;
        }
        catch(Exception)
        {
            if (String.IsNullOrEmpty(outputPath))
                outputPath = ".";
            Debug.LogWarning("Couldn't get settings file, using outputPath = " + outputPath);
        }

        if(beforeBuild!=null)
            beforeBuild();

        var error = BuildPipeline.BuildPlayer(scenes, outputPath, target, BuildOptions.None);
        if (error != "")
            throw new Exception(error); // ensures exit code != 0.
    }

private static string GetPath(BuildTarget target)
    {
        try
        {
            XDocument settings = XDocument.Load(settingsFile);
            return (from item in settings.Root.Element("output-paths").Descendants()
                    where item.Name.LocalName == target.ToString()
                    select item.Value).First();
        }
        catch (InvalidOperationException)
        {
            throw new ArgumentException(string.Format("Path not found for target {0}, check {1}.",
                    target.ToString(), settingsFile));
        }
    }
}
