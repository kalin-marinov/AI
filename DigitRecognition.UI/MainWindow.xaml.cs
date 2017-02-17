using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bitmap = System.Drawing.Bitmap;

namespace DigitRecognition.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Point currentPoint = new Point();

        private byte[] pixels;
        private Network network;

        private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);
        }

        private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var line = new Line()
                {
                    StrokeThickness = 8,
                    Stroke = brush,
                    X1 = currentPoint.X - CanvasBorder.Margin.Left,
                    Y1 = currentPoint.Y - CanvasBorder.Margin.Top,
                    X2 = e.GetPosition(this).X - CanvasBorder.Margin.Left,
                    Y2 = e.GetPosition(this).Y - CanvasBorder.Margin.Top
                };

                currentPoint = e.GetPosition(this);
                paintSurface.Children.Add(line);
            }
        }


        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            RasterizeToBitmap(this);
            paintSurface.Children.Clear();
            guessBtn.IsEnabled = true;
        }

        public void RasterizeToBitmap(UIElement element)
        {
            const double Scale = 0.2;

            var target = new RenderTargetBitmap(
                (int)element.RenderSize.Width, (int)element.RenderSize.Height,
                160, 160, PixelFormats.Pbgra32);

            target.Render(element);
            var scaled = Resize(target, Scale);

            var encoder = new PngBitmapEncoder();
            var outputFrame = BitmapFrame.Create(scaled);

            encoder.Frames.Add(outputFrame);

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);

                var bmp = new Bitmap(ms);

                // 10 pixels padding
                var startX = (int)((CanvasBorder.Margin.Left + 10) * Scale);
                var startY = (int)((CanvasBorder.Margin.Top + 10) * Scale);

                var width = (int)((CanvasBorder.ActualWidth - 10) * Scale);
                var height = (int)((CanvasBorder.ActualHeight - 10) * Scale);

                var rect = new System.Drawing.Rectangle(startX, startY, width, height);

                var test2 = CropImage(bmp, rect);
                var imgName = Guid.NewGuid().ToString() + ".png";

                pixels = GetPixels(test2).ToArray();
                test2.Save(imgName);

                bitmapImage.Source = new BitmapImage(new Uri(System.IO.Path.Combine(Directory.GetCurrentDirectory(), imgName)));

                bmp.Dispose();
                test2.Dispose();
            }
        }

        private IEnumerable<byte> GetPixels(Bitmap test2)
        {
            for (int i = 0; i < test2.Width; i++)
            {
                for (int j = 0; j < test2.Height; j++)
                {
                    var color = test2.GetPixel(i, j);
                    if (color.R!= color.G && color.G != color.B)
                        throw new ArgumentException("Pixels are not in the 256 shades of gray");

                    yield return (byte)(255 - color.R);
                }
            }
        }

        public static Bitmap CropImage(Bitmap b, System.Drawing.Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            var g = System.Drawing.Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        BitmapSource Resize(BitmapSource img, double scale)
        {
            var s = new ScaleTransform(scale, scale);
            var res = new TransformedBitmap(img, s);
            return res;
        }

        private void guessBtn_Click(object sender, RoutedEventArgs e)
        {
            var input = pixels.Select(b => (double)b).ToArray();
            var result = network.Calculate(input);

            resultBlock.Text = "The number is " + Program.ArrayToDigit(result);
                
        }

        private async void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() => Program.TrainNetwork());
            network = await task;

            btn1.IsEnabled = true;
        }
    }
}
