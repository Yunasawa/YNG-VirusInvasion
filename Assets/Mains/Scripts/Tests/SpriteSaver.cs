using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

public class SpriteSaver : MonoBehaviour
{
    public Texture2D spriteSheet;

    [Button]
    public void SaveSpritesToSquarePNG()
    {
        if (spriteSheet == null)
        {
            Debug.LogError("Sprite sheet not assigned.");
            return;
        }

        string directoryPath = "Assets/SavedSquareSprites";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Load all sprites from the sprite sheet
        string spriteSheetPath = UnityEditor.AssetDatabase.GetAssetPath(spriteSheet);
        Object[] sprites = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(spriteSheetPath);
        foreach (Object obj in sprites)
        {
            if (obj is Sprite sprite)
            {
                SaveSpriteAsSquarePNG(sprite, directoryPath);
            }
        }
        Debug.Log("Sprites saved to " + directoryPath);
    }

    private void SaveSpriteAsSquarePNG(Sprite sprite, string directoryPath)
    {
        int maxSize = Mathf.Max((int)sprite.rect.width, (int)sprite.rect.height);
        Texture2D squareTexture = new Texture2D(maxSize, maxSize);
        Color[] clearPixels = new Color[maxSize * maxSize];
        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = Color.clear;
        }
        squareTexture.SetPixels(clearPixels);

        // Get the pixel data from the sprite and place it in the center of the square texture
        Color[] pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
        squareTexture.SetPixels((maxSize - (int)sprite.rect.width) / 2, (maxSize - (int)sprite.rect.height) / 2, (int)sprite.rect.width, (int)sprite.rect.height, pixels);
        squareTexture.Apply();

        // Encode texture into PNG
        byte[] bytes = squareTexture.EncodeToPNG();
        string filePath = Path.Combine(directoryPath, sprite.name + ".png");
        File.WriteAllBytes(filePath, bytes);
    }
}
