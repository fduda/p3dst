namespace Aquila
{
    public class TextureUtility
    {
        public static void SaveToRGB(Texture2<Vector4> texture, System.Drawing.Bitmap bitmap)
        {
            int width = texture.Width;
            int height = texture.Height;

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* pixel = (byte*)data.Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Vector4 color = texture.Raw[y, x];
                        byte r = (byte)(color.R * 255);
                        byte g = (byte)(color.G * 255);
                        byte b = (byte)(color.B * 255);
                        pixel[0] = b;
                        pixel[1] = g;
                        pixel[2] = r;
                        pixel += 3;
                    }
                }
            }

            bitmap.UnlockBits(data);
        }

        // TODO unsafe version
        public static void LoadFromRGB(Texture2<Vector4> texture, System.Drawing.Bitmap bitmap)
        {
            int width = texture.Width;
            int height = texture.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color c = bitmap.GetPixel(x, y);
                    float r = c.R / 255.0f;
                    float g = c.G / 255.0f;
                    float b = c.B / 255.0f;
                    float a = 1.0f;
                    texture.Raw[y, x] = new Vector4(r, g, b, a);
                }
            }
        }
    }
}
