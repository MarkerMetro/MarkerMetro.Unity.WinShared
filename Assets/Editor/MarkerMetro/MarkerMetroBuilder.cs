using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Assets.Editor.MarkerMetro
{
    public static class MarkerMetroBuilder
    {
        [MenuItem("MarkerMetro/Build.../All")]
        public static void BuildAll()
        {
            BuildMetro();
            BuildWP8();
        }

        [MenuItem("MarkerMetro/Build.../Windows Store Apps")]
        public static void BuildMetro()
        {
            string outputPath = MarkerMetro.CommandLineReader.GetCustomArgument("outputPath");
            if (String.IsNullOrEmpty(outputPath))
            {
                outputPath = MarkerMetro.CommandLineReader.GetCustomArgument("metroOutputPath");
            }

            Build(BuildTarget.MetroPlayer,
                outputPath,
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
            string outputPath = MarkerMetro.CommandLineReader.GetCustomArgument("outputPath");
            if (String.IsNullOrEmpty(outputPath))
            {
                outputPath = MarkerMetro.CommandLineReader.GetCustomArgument("wp8OutputPath");
            }

            Build(BuildTarget.WP8Player,
                outputPath,
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
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            outputPath = GetPath(target);

            if (beforeBuild != null)
            {
                beforeBuild();
            }

            var error = BuildPipeline.BuildPlayer(scenes, outputPath, target, BuildOptions.None);
            if (error != "")
            {
                throw new Exception(error); // ensures exit code != 0.
            }
        }

        private static string GetPath(BuildTarget target)
        {
            string projectName = target == BuildTarget.WP8Player ? "WindowsPhone" : "WindowsStore";
            string defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../WindowsSolution/" + projectName));
            string outputPath = EditorUtility.OpenFolderPanel("Choose Folder (" + projectName + ")", defaultDir, "");
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = defaultDir;
            }
            return outputPath;
        }
    }
}