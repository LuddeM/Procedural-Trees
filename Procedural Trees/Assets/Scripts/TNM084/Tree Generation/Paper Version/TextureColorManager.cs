using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// Adds color texture to the branches based
    /// on Simplex Noise
    /// </summary>
    public class TextureColorManager : MonoBehaviour
    {
        public float ZoomFactor = 50f;
        public int TextureWidth = 256;
        public int TextureHeight = 5;

        private float _offsetX;
        private float _offsetY;

        public Color BranchColor;

        public ColorInformation GetBranchColorWithTexture()
        {
            var texture = new Texture2D(TextureWidth, TextureHeight);
            _offsetX = Random.Range(0, 999999);
            _offsetY = Random.Range(0, 999999);

            // Loop through all of the pixels
            for (var xPos = 0; xPos < TextureWidth; xPos++)
            {
                for (var yPos = 0; yPos < TextureHeight; yPos++)
                {
                    var noisedColor = GetSimplexNoiseColor(xPos, yPos);
                    texture.SetPixel(xPos, yPos, noisedColor);
                }
            }

            // Make sure to apply the new color data
            texture.Apply();
            return new ColorInformation(BranchColor, texture);
        }

        private Color GetSimplexNoiseColor(int xPos, int yPos)
        {
            var xCoord = (float)xPos / TextureWidth * ZoomFactor + _offsetX;
            var yCoord = (float)yPos / TextureHeight * ZoomFactor + _offsetY;

            var coordSimplexNoise = ((float)Common.SimplexNoise.noise(xCoord, yCoord) + 1) / 2.0f;
            return new Color(coordSimplexNoise, coordSimplexNoise, coordSimplexNoise);
        }
    }

    public struct ColorInformation
    {
        public Color Color;
        public Texture2D Texture;

        public ColorInformation(Color color, Texture2D texture)
        {
            Color = color;
            Texture = texture;
        }
    }

}