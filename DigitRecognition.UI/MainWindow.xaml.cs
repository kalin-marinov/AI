using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Point currentPoint = new Point();
        private byte[] pixels;
        private Network network;
        private string progressText;
        private string resultText;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //ImageHelpers.ExportMNistImages();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public string ProgressText
        {
            get { return progressText; }
            set { progressText = value; PropertyChanged(this, new PropertyChangedEventArgs(nameof(ProgressText))); }
        }

        public string ResultText
        {
            get { return resultText; }
            set { resultText = value; PropertyChanged(this, new PropertyChangedEventArgs(nameof(ResultText))); }
        }


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
            const double SCALE = 0.2;

            var target = new RenderTargetBitmap((int)element.RenderSize.Width, (int)element.RenderSize.Height,
                160, 160, PixelFormats.Pbgra32);

            target.Render(element);
            var scaled = target.Resize(SCALE);

            var encoder = new PngBitmapEncoder();
            var outputFrame = BitmapFrame.Create(scaled);

            encoder.Frames.Add(outputFrame);

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);

                var bmp = new Bitmap(ms);

                // 10 pixels padding - because the image contains the element borders
                var startX = (int)((CanvasBorder.Margin.Left + 5) * SCALE);
                var startY = (int)((CanvasBorder.Margin.Top + 5) * SCALE);

                var width = (int)((CanvasBorder.ActualWidth - 10) * SCALE);
                var height = (int)((CanvasBorder.ActualHeight - 10) * SCALE);

                var rect = new System.Drawing.Rectangle(startX, startY, width, height);

                var finalImage = bmp.CropImage(rect);
                var imgName = Guid.NewGuid().ToString() + ".png";

                pixels = finalImage.GetPixels();
                var checkImage = ImageHelpers.CreateBitMap(pixels);
                checkImage.Save(imgName);
                    
                //finalImage.Save(imgName);
                bitmapImage.Source = new BitmapImage(new Uri(System.IO.Path.Combine(Directory.GetCurrentDirectory(), imgName)));

                bmp.Dispose();
                finalImage.Dispose();
            }
        }

        private void guessBtn_Click(object sender, RoutedEventArgs e)
        {
            var input = pixels.Select(MNistTraining.MapInput).ToArray();
            var result = network.Calculate(input);

            ResultText = "The number is " + result.ToDigit();
        }

        private async void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            ProgressText = "Begin training";

            var train = new MNistTraining();
            train.Changed += (_, args) => ProgressText = $"Training | Epoch: {args.Epoch} / {args.MaxEpoch}";

            var task = Task.Run(() => train.TrainNetwork());
            network = await task;

            ProgressText = "Training finished";
            btn1.IsEnabled = true;
        }
    }
}
