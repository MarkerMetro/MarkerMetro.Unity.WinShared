using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/**
 * Optimizes memory usage by changing texture settings.
 */
namespace MarkerMetro.Unity.WinShared.Editor
{
    class MemoryOptimizer : AssetPostprocessor
    {
        /**
         * Factor that will define the max size of a texture.
         * Example: Texture is 640x480 with MaxSize = 4096.
         *  Using TextureResizingFactor == 0: MaxSize = 512 (the nearest power of two that is smaller than the biggest dimension, 640).
         *  Using TextureResizingFactor == 1: MaxSize = 256 (512 / 2).
         *  Using TextureResizingFactor == 2: MaxSize = 128 (512 / 4).
         * It won't change anything if the resulting size is bigger than the current MaxSize.
         * Check the comments at the end of the file for details on resizing NGUI atlases.
         */
        const int TextureResizingFactor = 1; // 1 = 1/2, 2 = 1/4, 3 = 1/8 ...

        /**
         * Changes the max size of textures and their compression settings to use DXT.
         * These settings are persisted in the meta files of the textures.
         * This script reimports the textures that were changed, so all modifications will be already applied after the script has run.
         * 
         * The script will IGNORE all textures whose paths are added to a file "WindowsAssetIgnoreList.csv" in the root of the Unity project.
         * Simply create a CSV file using Excel with a list of paths.
         * Path example: Assets/MarkerMetro/Example/Resources/Ivan.jpg
         */
        [MenuItem("Tools/Marker Metro/Memory Optimization/Optimize Texture Settings", priority = 30)]
        public static void OptimizeSettings()
        {
            const string ignoreListPath = "Assets/MarkerMetro/Editor/MemoryOptimizerExcludeList.csv";
            List<string> ignoreList;
            if (File.Exists(ignoreListPath))
            {
                var csv = from line in File.ReadAllLines(ignoreListPath)
                          select (line.Split(',').Select(x => x.Trim())
                          .Where(x => !string.IsNullOrEmpty(x)).ToArray());

                ignoreList = csv.SelectMany(l => l).ToList();
            }
            else
            {
                ignoreList = new List<string>();
            }
            
            //  gets all texture files supported by Unity
            foreach (string s in AssetDatabase.GetAllAssetPaths()
                .Where(path => path.EndsWith(".psd", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".tif", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".tga", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".iff", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".pict", StringComparison.OrdinalIgnoreCase)))
            {
                if(ignoreList.Contains(s))
                {
                    Debug.Log("Memory Optimizer: Ignoring " + s);
                    continue;
                }

                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(s, typeof(Texture2D));
                TextureImporter importer = AssetImporter.GetAtPath(s) as TextureImporter;
                Debug.Log("Memory Optimizer: Processing " + s);
                DxtCompressTexture(importer);
                ResizeTexture(texture, importer);
                AssetDatabase.ImportAsset(s);
            }
        }

        /**
         * Does two things:
         * 1 - Changes the NPOT strategy to "ToNearest";
         * 2 - Changes the compression to DXT1 or DXT5 if compression is set to something different than DTX1.
         * It only changes the meta file of the texture.
         */
        static void DxtCompressTexture(TextureImporter textureImporter)
        {
            TextureImporterFormat format;
            TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImporterSettings);
            format = textureImporterSettings.textureFormat;

            if (format != TextureImporterFormat.DXT1)
            {
                if (textureImporter.DoesSourceTextureHaveAlpha())
                    format = TextureImporterFormat.DXT5;
                else
                    format = TextureImporterFormat.DXT1;
            }

            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;

            textureImporterSettings.textureFormat = format;
            textureImporter.SetTextureSettings(textureImporterSettings);
        }

        /**
         * Changes the "Max Size" of a texture, acording to its original size and a factor defined by TextureResizingFactor.
         * It only changes the meta file of the texture.
         */
        static void ResizeTexture(Texture2D texture, TextureImporter textureImporter)
        {
            TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImporterSettings);
           
            //grabbing the max texture dimension for use in size calculation
            float size = Mathf.Max(texture.width, texture.height);

            string s = "Memory Optimizer: Original size = " + size + "; Original max size = " + textureImporterSettings.maxTextureSize;

            // Getting the smallest texture size between original texture size, to be resized  by TextureResizingFactor, and maxTextureSize set in asset importer settings:
            size = Mathf.Min(Mathf.Pow(2f, Mathf.Floor(Mathf.Log(size, 2f)) - TextureResizingFactor), textureImporterSettings.maxTextureSize);

            Debug.Log(s + "; New max size = " + size);

            // we won't make any changes if the calculate size is lesser than the minimum on Unity dropdown box (32):
            if (size >= 32)
            {
                textureImporterSettings.maxTextureSize = (int)size;
                textureImporter.SetTextureSettings(textureImporterSettings);
            }
        }
    }
}

