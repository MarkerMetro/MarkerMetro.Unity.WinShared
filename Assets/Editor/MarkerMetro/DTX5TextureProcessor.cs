using UnityEngine;
using UnityEditor;

class DTX5TexturePostprocessor : AssetPostprocessor {
    const bool Enabled = false; 
#pragma warning disable 
    /**
     * Change the import settings of the texture to import as DXT5.
     * This is preferable over the post process method since it changes the meta file so
     * you can commit the changes.
     * 
     * Attention! Reimport all doesn't work (it changes the import settings but somehow doesn't change the 
     * meta file). Also, filtering textures via search box in Unity (4.5f6) doesn't always list all textures.
     */
    public void OnPreprocessTexture()
    {
        if (!Enabled)
            return;
        TextureImporter textureImporter = assetImporter as TextureImporter;
        textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
        textureImporter.textureFormat = TextureImporterFormat.DXT5;
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
