using UnityEngine;
using UnityEditor;

class DTX5TexturePostprocessor : AssetPostprocessor
{
    // STEP 1 - Set this to true begin enabling
    const bool Enabled = false;

    // STEP 2 - Right click on the texture in the editor you wish to apply the processing too and click 'Reimport'

#pragma warning disable
    /**
     * This script modifies the import settings of the texture to import as DXT5.
     * This is preferable over the post process method since it changes the meta file so
     * you can commit the changes.
     * 
     * Attention! Reimport all doesn't work (it changes the import settings but somehow doesn't change the 
     * meta file). Also, filtering textures via search box in Unity (4.5f6) doesn't always list all textures.
     * 
     */
    public void OnPreprocessTexture()
    {
#if UNITY_WP8
        if (!Enabled)
            return;

        TextureImporter textureImporter = assetImporter as TextureImporter;
        int originalMaxTextureSize = 1024; // Default maxTextureSize
        TextureImporterFormat originalTextureFormat;

        // Get the existing texture max size so we don't override it
        var platformTextureSettings = textureImporter.GetPlatformTextureSettings("WP8", out originalMaxTextureSize, out originalTextureFormat);

        textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
        
        // Force texture to advanced so we can set the DXT5 platform override
        textureImporter.textureType = TextureImporterType.Advanced;
        
        // Apply the WP8 specific override and keep the original texture max size
        textureImporter.SetPlatformTextureSettings("WP8", originalMaxTextureSize, TextureImporterFormat.DXT5);
#endif
    }

    /**
 * Tries to apply a DXT5 compression when importing a texture.
 * This doesn't actually change anything (meta file, the texture), only 
 * Unity's internal representation of the texture.
 */
    public void OnPostprocessTexture(Texture2D t)
    {
        //if(Enabled)
        //  EditorUtility.CompressTexture(t, TextureFormat.DXT5, 2); 
    }

#pragma warning restore
}