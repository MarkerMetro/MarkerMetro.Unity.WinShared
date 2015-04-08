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
        [MenuItem("Tools/MarkerMetro/Build/Windows Universal 8.1", priority = 1)]
        public static void BuildUniversalFromMenu()
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

        public static void DoBuildUniversal(string outputPath)
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

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = GetPath(target);
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

            //Unity creates unused assets no matter what
            //this hack removes those assets after the compilation
            RemoveIncorrectAssets(outputPath);
        }

        static void RemoveIncorrectAssets(string outputPath)
        {
            string projectName = PlayerSettings.productName;
            string assetsPath = outputPath + "/" + projectName + "/" + projectName + ".Windows/Assets/";

            //removing unused assets from WINDOWS project
            File.Delete(assetsPath + "WideTile.scale-100.png");

            File.Delete(assetsPath + "MediumTile.scale-100.png");

            //removing unused assets from WINDOWS_PHONE project
            assetsPath = outputPath + "/" + projectName + "/" + projectName + ".WindowsPhone/Assets/";
            File.Delete(assetsPath + "WideTile.scale-100.png");

            File.Delete(assetsPath + "SmallTile.scale-240.png");

            File.Delete(assetsPath + "MediumTile.scale-240.png");

            File.Delete(assetsPath + "MediumTile.scale-100.png");
        }

        static string GetPath(BuildTarget target)
        {
            string projectName = "Universal";
            string defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../WindowsSolutionUniversal"));

            string outputPath = EditorUtility.OpenFolderPanel("Choose Folder (" + projectName + ")", defaultDir, "");
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = defaultDir;
            }
            return outputPath;
        }
    }
}