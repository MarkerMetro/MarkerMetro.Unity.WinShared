using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using CommandLineReader = MarkerMetro.Unity.WinShared.Editor.CommandLineReader;

namespace MarkerMetro.Unity.WinShared.Editor
{
    public static class MarkerMetroBuilder
    {
        [MenuItem("Tools/MarkerMetro/Build/All", priority = 1)]
        public static void BuildAll()
        {
            BuildMetroFromMenu();
            BuildWP8FromMenu();
            BuildUniversalFromMenu();
        }

        [MenuItem("Tools/MarkerMetro/Build/Windows Universal 8.1", priority = 2)]
        public static void BuildUniversalFromMenu ()
        {
            DoBuildUniversal(string.Empty);
        }

        public static void BuildUniversal()
        {
            string outputPath = CommandLineReader.GetCustomArgument("outputPath");
            if (String.IsNullOrEmpty(outputPath))
            {
                outputPath = CommandLineReader.GetCustomArgument("universalOutputPath");
            }

            DoBuildUniversal(outputPath);
        }

        public static void DoBuildUniversal (string outputPath)
        {
#if UNITY_5
            Build(BuildTarget.WSAPlayer,
#else
            Build(BuildTarget.MetroPlayer,
#endif
                outputPath,
                () =>
                {
#if UNITY_5
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WSA;
                    EditorUserBuildSettings.wsaSDK = WSASDK.UniversalSDK81;
#else
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Metro;
                    EditorUserBuildSettings.metroBuildType = MetroBuildType.VisualStudioCSharp;
                    EditorUserBuildSettings.metroSDK = MetroSDK.UniversalSDK81;
#endif
                }, true);
        }

        [MenuItem("Tools/MarkerMetro/Build/Windows 8.1", priority = 3)]
        public static void BuildMetroFromMenu ()
        {
            DoBuildMetro(string.Empty);
        }

        public static void BuildMetro()
        {
            string outputPath = CommandLineReader.GetCustomArgument("outputPath");
            if (String.IsNullOrEmpty(outputPath))
            {
                outputPath = CommandLineReader.GetCustomArgument("metroOutputPath");
            }

            DoBuildMetro(outputPath);
        }

        public static void DoBuildMetro (string outputPath)
        {
#if UNITY_5
            Build(BuildTarget.WSAPlayer,
#else
            Build(BuildTarget.MetroPlayer,
#endif
                outputPath,
                () =>
                {
#if UNITY_5
					EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WSA;
					EditorUserBuildSettings.wsaSDK = WSASDK.SDK81;
#else
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Metro;
                    EditorUserBuildSettings.metroBuildType = MetroBuildType.VisualStudioCSharp;
                    EditorUserBuildSettings.metroSDK = MetroSDK.SDK81;
#endif
                });
        }

        [MenuItem("Tools/MarkerMetro/Build/Windows Phone 8.0", priority = 4)]
        public static void BuildWP8FromMenu ()
        {
            DoBuildWP8(string.Empty);
        }

        public static void BuildWP8()
        {
            string outputPath = CommandLineReader.GetCustomArgument("outputPath");
            if (String.IsNullOrEmpty(outputPath))
            {
                outputPath = CommandLineReader.GetCustomArgument("wp8OutputPath");
            }

            DoBuildWP8(outputPath);
        }

        public static void DoBuildWP8 (string outputPath)
        {
            Build(BuildTarget.WP8Player,
                outputPath,
                () =>
                {
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WP8;
                });
        }

        public static void Build(BuildTarget target,
            string outputPath = null,
            Action beforeBuild = null,
            bool isUniversal = false)
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

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = GetPath(target, isUniversal);
            }

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

        static string GetPath(BuildTarget target, bool isUniversal)
        {
            string projectName = string.Empty;
            string defaultDir = string.Empty;

            if (isUniversal)
            {
                projectName = "Universal";
                defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../WindowsSolutionUniversal"));
            }
            else
            {
                projectName = target == BuildTarget.WP8Player ? "WindowsPhone" : "WindowsStore";
                defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../WindowsSolution/" + projectName));
            }
            
            string outputPath = EditorUtility.OpenFolderPanel("Choose Folder (" + projectName + ")", defaultDir, "");
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = defaultDir;
            }
            return outputPath;
        }
    }
}