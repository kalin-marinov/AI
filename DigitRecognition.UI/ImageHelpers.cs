using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ScaleTransform = System.Windows.Media.ScaleTransform;

namespace DigitRecognition.UI
{
    public static class ImageHelpers
    {
        public static Bitmap CreateBitMap(byte[] pixels)
        {
            const int width = 28;
            const int height = 28;

            var bmp = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var colorByte = 255 - pixels[y + x];
                    var color = Color.FromArgb(colorByte, colorByte, colorByte);
                    bmp.SetPixel(x, y, color);

                }
            }

            return bmp;
        }

        public static IEnumerable<byte> GetPixels(this Bitmap test2)
        {
            for (int i = 0; i < test2.Width; i++)
            {
                for (int j = 0; j < test2.Height; j++)
                {
                    var color = test2.GetPixel(i, j);
                    if (color.R != color.G && color.G != color.B)
                        throw new ArgumentException("Pixels are not in the 256 shades of gray");

                    yield return (byte)(255 - color.R);
                }
            }
        }

        public static Bitmap CropImage(this Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            var g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        public static BitmapSource Resize(this BitmapSource img, double scale)
        {
            var s = new ScaleTransform(scale, scale);
            var res = new TransformedBitmap(img, s);
            return res;
        }



    }
}
