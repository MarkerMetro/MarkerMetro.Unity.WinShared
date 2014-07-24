using UnityEngine;
using UnityEditor;

/**
 * Optimizes memory usage by changing texture and audio settings.
 * To use enable/disable the constants defined below and import the assets.
 * Note: a "reimport all" may not work, you may have to select the assets and reimport.
 */
class MemoryOptimizer : AssetPostprocessor
{

    // If enabled, does 2 things:
    // 1 - Changes the NPOT strategy to "ToNearest";
    // 2 - Changes the compression to DXT1 or DXT5 if compression is set to something different than DTX1.
    // It only changes the meta file of the texture. The compression will be set for WINDOWS PHONE only, but
    // the NPOT strategy is global.
    const bool TextureCompressionEnabled = false;

    // If enabled, changes the "Max Size" of a texture, acording to it's original size and a factor defined by
    // TextureResizingFactor. ONLY FOR WINDOWS PHONE, don't worry about WSA as it only changes settings for WP8.
    // It only changes the meta file of the texture.
    // PLEASE NOTE that this occour after the processing of the texture, so the texture must be reimported again in
    // order for the changes that were made by this part of the script to take effect. So:
    // 1 - set these bools to true;
    // 2 - import the textures - meta files will change, but the imported internal texture won't;
    // 3 - set these bools to false - you don't want to resize/compress again;
    // 4 - reimport the textures - changes made to the meta files will take effect.
    const bool TextureResizingEnabled = false;

    // Factor that will define the max size of a texture.
    // Example: Texture is 640x480 with MaxSize = 4096.
    // Using TextureResizingFactor == 0, MaxSize = 512 (the nearest power of two that is smaller than the biggest dimension, 640).
    // Using TextureResizingFactor == 1, MaxSize = 256 (512 / 2).
    // Using TextureResizingFactor == 2, MaxSize = 128 (512 / 4).
    // It won't change anything if the resulting size is bigger than the current MaxSize.
    // Check the comments at the end of the file for details on resizing NGUI atlases.
    const int TextureResizingFactor = 1; // 1 = 1/2, 2 = 1/4, 3 = 1/8 ...

    // Compress audio assets.
    const bool AudioCompressionEnabled = false;

#pragma warning disable

    public void OnPreprocessTexture()
    {
        if (!TextureCompressionEnabled)
            return;

        TextureImporter textureImporter = assetImporter as TextureImporter;
        int originalMaxTextureSize;
        TextureImporterFormat format;

        var platformTextureSettings = textureImporter.GetPlatformTextureSettings("WP8", out originalMaxTextureSize, out format);

        if (format != TextureImporterFormat.DXT1)
        {
            if (textureImporter.DoesSourceTextureHaveAlpha())
                format = TextureImporterFormat.DXT5;
            else
                format = TextureImporterFormat.DXT1;
        }

        textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;

        textureImporter.SetPlatformTextureSettings("WP8", originalMaxTextureSize, format);
    }
    public void OnPostprocessTexture(Texture2D t)
    {
        if (!TextureResizingEnabled)
            return;

        TextureImporter textureImporter = assetImporter as TextureImporter;
        int originalMaxTextureSize;
        TextureImporterFormat format;

        var platformTextureSettings = textureImporter.GetPlatformTextureSettings("WP8", out originalMaxTextureSize, out format);

        float size = Mathf.Max(t.width, t.height);
        size = Mathf.Min(Mathf.Pow(2f, Mathf.Floor(Mathf.Log(size, 2f)) - TextureResizingFactor), originalMaxTextureSize);

        textureImporter.SetPlatformTextureSettings("WP8", (int)size, format);
    }

    public void OnPreprocessAudio()
    {
        if (!AudioCompressionEnabled)
            return;
        AudioImporter audioImporter = assetImporter as AudioImporter;
        audioImporter.loadType = AudioImporterLoadType.StreamFromDisc;
        audioImporter.compressionBitrate = 32000;
    }

#pragma warning restore
}

// If you plan to resize NGUI atlases as well, this may interest you. Put the above WP8 code on UISprite.cs
// (the location is defined by these 2 first and 2 last lines):
//
//               mSprite.width - mSprite.borderLeft - mSprite.borderRight,
//               mSprite.height - mSprite.borderBottom - mSprite.borderTop);
//#if UNITY_WP8
//              float resizingFactor = 0.5f // <- Ajust according to TextureResizingFactor.
//              mOuterUV.x *= resizingFactor;
//              mOuterUV.y *= resizingFactor;
//              mOuterUV.width *= resizingFactor;
//              mOuterUV.height *= resizingFactor;
//              mInnerUV.x *= resizingFactor;
//              mInnerUV.y *= resizingFactor;
//              mInnerUV.width *= resizingFactor;
//              mInnerUV.height *= resizingFactor;
//#endif
//              mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, tex.width, tex.height);
//              mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, tex.width, tex.height);