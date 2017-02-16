using NeuralNetwork.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class Program
    {
        public static void Main()
        {
            //SimpleTest();

            var trainImages = ImageDataReader.ReadImageFile(@"Data\train-images.idx3-ubyte").Take(1000).ToList();
            var trainLabels = ImageDataReader.ReadLabels(@"Data\train-labels.idx1-ubyte").Take(1000).ToList();

            var testImages = ImageDataReader.ReadImageFile(@"Data\t10k-images.idx3-ubyte").ToList();
            var testLabels = ImageDataReader.ReadLabels(@"Data\t10k-labels.idx1-ubyte").ToList();

            TestNet2(trainImages, trainLabels, testImages, testLabels);
        }

        private static void TestNet2(List<byte[]> trainImages, List<byte> trainLabels, List<byte[]> testImages, List<byte> testLabels)
        {
            var net = new Network(784, 100, 10);

            int epoch = 0;
            while (epoch < 100)
            {
                for (int i = 0; i < trainImages.Count; i++)
                {
                    var image = trainImages[i];
                    var input = image.Select(MapInput).ToArray();
                    var expected = DigitToArray(trainLabels[i]);

                    net.Calculate(input);

                    // Back propagate
                    var result = net.LayerValues.Last();

                    net.BackPropagate(expected);
                    net.Calculate(input);
                    var result2 = net.LayerValues.Last();


                    Console.WriteLine($"Trained for image {i} Initial:{ArrayToDigit(result)} After backprop: {ArrayToDigit(result)} Actual: {trainLabels[i]}");
                }
                Console.WriteLine("Epoch: " + epoch);
                trainImages = trainImages.Shuffle();   // This network cannot learn on shuffled input yet
                epoch++;
            }

            // Test:
            for (int i = 0; i < testImages.Count; i++)
            {
                var image = trainImages[i];
                var input = image.Select(b => (double)b).ToArray();
                var output = testLabels[i];

                net.Calculate(input);
                var result = net.LayerValues.Last();
                Console.WriteLine($"Processing image {i} Guess: {ArrayToDigit(result)} Actual: {output}");
            }
        }

        static void SimpleTest()
        {
            var net = new Network(6, 10, 5);
            var input = new[] { 0.8, 0.5, 0.1, 0.3, 0.6, 0.821 };

            net.Calculate(input);
            var expected = new[] { 0.01, 0.01, 0.7, 0.25, 0.01 };
            var result = net.LayerValues.Last();

            while (result.CalculateError(expected) > 0.00001)
            {
                Console.WriteLine($"Result: [{string.Join(",", result)}]");
                net.BackPropagate(expected);
                net.Calculate(input);
                result = net.LayerValues.Last();
            }
        }

        static byte ArrayToDigit(IList<double> data)
          => (byte)data.IndexOf(data.Max());


        static double[] DigitToArray(byte label)
        {
            switch (label)
            {
                case 0: return new[] { 0.999d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d };
                case 1: return new[] { 0.001d, 0.999d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d };
                case 2: return new[] { 0.001d, 0.001d, 0.999d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d };
                case 3: return new[] { 0.001d, 0.001d, 0.001d, 0.999d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d };
                case 4: return new[] { 0.001d, 0.001d, 0.001d, 0.001d, 0.999d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d };
                case 5: return new[] { 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.999d, 0.001d, 0.001d, 0.001d, 0.001d };
                case 6: return new[] { 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.999d, 0.001d, 0.001d, 0.001d };
                case 7: return new[] { 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.999d, 0.001d, 0.001d };
                case 8: return new[] { 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.999d, 0.001d };
                case 9: return new[] { 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.999d };
                default:
                    throw new ArgumentException("invalid label " + label);
            }
        }

        /// <summary> Maps a number from 0 to 255 to a decimal between 0 and 1 </summary>
        static double MapInput(byte b)
            => (double)b / 2550;

    }
}
