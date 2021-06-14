using System.Drawing;

namespace BluePhoenix
{
    public class ImageGenerator
    {
        Bitmap image;
        public void Parse(string action)
        {

        }
        public void Create(int w, int h)
        {
            image = new Bitmap(w, h);
        }
        public void SetPixel(int x, int y, Color c)
        {
            image.SetPixel(x, y, c);
        }
        public void Fill(int x1, int y1, int x2, int y2, Color c)
        {
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    image.SetPixel(x, y, c);
                }
            }
        }
        public void Paste(int x, int y, Bitmap img)
        {
            for (int a = 0; a < img.Width; a++)
            {
                for (int b = 0; y < img.Height; b++)
                {
                    if (x + a < image.Width && y + b < image.Height && x + a >= 0 && y + b >= 0)
                    {
                        image.SetPixel(x + a, y + b, img.GetPixel(a, b));
                    }
                }
            }
        }
    }
}
