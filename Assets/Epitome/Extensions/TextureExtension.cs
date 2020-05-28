using UnityEngine;

namespace Epitome
{
    public static class TextureExtension
    {
        public static void SetColor(this Texture2D self, float rgb, float a = 255)
        {
            self.SetColor(new Color(rgb / 255f, rgb / 255f, rgb / 255f, a / 255f));
        }

        public static void SetColor(this Texture2D self, float r, float g,float b, float a = 255)
        {
            self.SetColor(new Color(r / 255f, g / 255f, b / 255f, a / 255f));
        }

        public static void SetColor(this Texture2D self,Color color)
        {
            for (var x = 0; x < self.width; x++)
            {
                for (var y = 0; y < self.height; y++)
                {
                    self.SetPixel(x, y, color);
                }
            }
            self.Apply(false);
        }

        public static void SetColor(this Texture2D self, Color[] colors)
        {
            var mipCount = Mathf.Min(colors.Length, self.mipmapCount);

            for (var mip = 0; mip < mipCount; ++mip)
            {
                var cols = self.GetPixels(mip);

                for (var i = 0; i < cols.Length; ++i)
                {
                    cols[i] = Color.Lerp(cols[i], colors[mip], 0.33f);
                }
                self.SetPixels(cols, mip);
            }
            self.Apply(false);
        }
    }
}