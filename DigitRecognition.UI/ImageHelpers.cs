using NeuralNetwork.Readers;
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

            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var colorByte = 255 - pixels[i];
                    var color = Color.FromArgb(255, colorByte, colorByte, colorByte);
                    bmp.SetPixel(x, y, color);
                    i++;
                }
            }

            return bmp;
        }

        public static byte[] GetPixels(this Bitmap test2)
        {
            var result = new byte[test2.Width * test2.Height];
            int counter = 0;

            for (int i = 0; i < test2.Width; i++)
            {
                for (int j = 0; j < test2.Height; j++)
                {
                    var color = test2.GetPixel(j, i);
                    if (color.R != color.G && color.G != color.B)
                        throw new ArgumentException("Pixels are not in the 256 shades of gray");

                    result[counter++] = (byte)(255 - color.R);
                }
            }

            return result;
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

        public static void ExportMNistImages()
        {
            var testImages = ImageDataReader.ReadImageFile(@"Data\t10k-images.idx3-ubyte").Take(50).ToList();
            var testLabels = ImageDataReader.ReadLabels(@"Data\t10k-labels.idx1-ubyte").Take(50).ToList();

            for (int i = 0; i < testImages.Count; i++)
            {
                var bmp = CreateBitMap(testImages[i]);
                bmp.Save($"image_{i}.png");
            }
        }

    }
}
