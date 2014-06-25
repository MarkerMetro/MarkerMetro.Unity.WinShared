using UnityEngine;
using UnityEditor;

/**
 * Tries to apply a DXT5 compression when importing a texture.
 * This will change the texture meta file.
 */
class DTX5TexturePostprocessor : AssetPostprocessor {

    // ENABLE HERE TO MAKE THE SCRIPT WORK
    const bool Enabled = false;

#pragma warning disable

    public void OnPostprocessTexture (Texture2D t) {
        if(Enabled)
            EditorUtility.CompressTexture(t, TextureFormat.DXT5, 2); 
    } 
#pragma warning restore
} 