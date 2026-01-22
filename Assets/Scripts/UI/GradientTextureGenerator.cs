using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GradientTextureGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Generate Pressure Gradient Texture")]
    static void GenerateGradientTexture()
    {
        int width = 256;
        int height = 32;
        
        Texture2D texture = new Texture2D(width, height);
        
        // Create gradient from red to yellow to green (reversed)
        for (int x = 0; x < width; x++)
        {
            float t = (float)x / width;
            Color color;
            
            if (t < 0.5f)
            {
                // Red to Yellow (0 to 0.5)
                color = Color.Lerp(Color.red, Color.yellow, t * 2f);
            }
            else
            {
                // Yellow to Green (0.5 to 1)
                color = Color.Lerp(Color.yellow, Color.green, (t - 0.5f) * 2f);
            }
            
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        
        // Save as PNG
        byte[] bytes = texture.EncodeToPNG();
        string path = "Assets/UI/PressureGradient.png";
        
        // Create directory if it doesn't exist
        System.IO.Directory.CreateDirectory("Assets/UI");
        
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
        
        IDebug.Log("Gradient texture created at: " + path);
        
        // Set texture import settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }
    }
#endif
}
