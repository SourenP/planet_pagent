using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public Texture2D GenerateTexture(int width, int height, float scale)
    {
        Texture2D texture = new Texture2D(width, height);

        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
            {
                Color color = CalculateColor((float)x / width, (float)y / height, scale);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();


        return texture;

    }
    Color CalculateColor(float x, float y, float scale)
    {
        float sample = Mathf.PerlinNoise(x * scale, y * scale);
        return new Color(sample, sample, sample);
    }
}
