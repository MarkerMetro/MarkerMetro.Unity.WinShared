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
        [MenuItem("Tools/Marker Metro/Build/Windows Universal 8.1", priority = 1)]
        public static void BuildUniversalFromMenu()
        {
            DoBuildUniversal(string.Empty);
        }

        [MenuItem("Tools/Marker Metro/Build/Windows Universal 10", priority = 2)]
        public static void BuildUniversal10FromMenu()
        {
            DoBuildUniversal10(string.Empty);
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

        public static void BuildUniversal10()
        {
            string outputPath = CommandLineReader.GetCustomArgument("outputPath");
            if (String.IsNullOrEmpty(outputPath))
            {
                outputPath = CommandLineReader.GetCustomArgument("universalOutputPath");
            }

            DoBuildUniversal10(outputPath);
        }

        public static void DoBuildUniversal(string outputPath)
        {
            Build(BuildTarget.WSAPlayer,
                outputPath,
                () =>
                {
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WSA;
                    EditorUserBuildSettings.wsaSDK = WSASDK.UniversalSDK81;
                });
        }

        public static void DoBuildUniversal10(string outputPath)
        {
            Build(BuildTarget.WSAPlayer,
                outputPath,
                () =>
                {
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WSA;
                    EditorUserBuildSettings.wsaSDK = WSASDK.UWP;
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

            if (beforeBuild != null)
            {
                beforeBuild();
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = GetPath(target);
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                var error = BuildPipeline.BuildPlayer(scenes, outputPath, target, BuildOptions.None);
                if (error != "")
                {
                    throw new Exception(error); // ensures exit code != 0.
                }
            }
            else
            {
                throw new Exception("No output path specified.");
            }

            //Unity creates unused assets no matter what
            //this hack removes those assets after the compilation
            if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81)
            {
                RemoveIncorrectAssets(outputPath);
            }
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
            string projectName;
            string defaultDir;

            if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81)
            {
                projectName = "Universal";
                defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../WindowsSolutionUniversal"));
            }
            else
            {
                projectName = "Universal 10";
                defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../Windows10"));
            }

            string outputPath = EditorUtility.OpenFolderPanel("Choose Folder (" + projectName + ")", defaultDir, "");
            return outputPath;
        }
    }
}