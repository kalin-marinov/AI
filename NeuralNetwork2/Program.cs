using NeuralNetwork.Readers;
using System;
using System.Collections.Generic;
using System.IO;
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

            var trainImages = ImageDataReader.ReadImageFile(@"Data\train-images.idx3-ubyte").Take(200).ToList();
            var trainLabels = ImageDataReader.ReadLabels(@"Data\train-labels.idx1-ubyte").Take(200).ToList();

            var testImages = ImageDataReader.ReadImageFile(@"Data\t10k-images.idx3-ubyte").Take(50).ToList();
            var testLabels = ImageDataReader.ReadLabels(@"Data\t10k-labels.idx1-ubyte").Take(50).ToList();

            var trainItems = trainImages.Zip(trainLabels, (img, lbl) => Tuple.Create(img, lbl)).ToList();

            TestNet2(trainItems, testImages, testLabels);
        }

        private static void TestNet2(List<Tuple<byte[], byte>> trainItems, List<byte[]> testImages, List<byte> testLabels)
        {
            var net = new Network(784, 100, 10);

            int epoch = 0;
            while (epoch < 200)
            {
                for (int i = 0; i < trainItems.Count; i++)
                {
                    var image = trainItems[i].Item1;
                    var input = image.Select(MapInput).ToArray();
                    var expected = DigitToArray(trainItems[i].Item2);

                    net.Calculate(input);

                    // Back propagate
                   // var result = net.LayerValues.Last();

                    net.BackPropagate(expected);
                    //net.Calculate(input);
                    //var result2 = net.LayerValues.Last();

                    //if (epoch > 200)
                    //    Console.WriteLine($"Trained for image {i} Initial:{ArrayToDigit(result)} After backprop: {ArrayToDigit(result)} Actual: {trainItems[i].Item2}  | Epoch {epoch}");

                }

                Console.WriteLine("Epoch: " + epoch);
                Console.CursorTop--;

                trainItems = trainItems.Shuffle();   // This network cannot learn on shuffled input yet
                epoch++;
            }

            File.WriteAllText("weights1.txt", net.Weights[0].GetMatrixString());
            File.WriteAllText("weights2.txt", net.Weights[0].GetMatrixString());

            // Test:
            int succeed = 0;

            for (int i = 0; i < testImages.Count; i++)
            {
                var image = testImages[i];
                var input = image.Select(b => (double)b).ToArray();
                var output = testLabels[i];

                net.Calculate(input);
                var result = net.LayerValues.Last();
                Console.WriteLine($"Processing image {i} Guess: {ArrayToDigit(result)} Actual: {output}");

                if (ArrayToDigit(result) == output)
                    succeed++;

            }

            Console.WriteLine($@"Sucess: {succeed} \ {testImages.Count}");
            Console.ReadLine();
        }

        static void SimpleTest()
        {
            var net = new Network(6, 10, 5);
            var rng = new Random();

            var input = Enumerable.Range(0, 6).Select(_ => rng.NextDouble()).ToArray();
            var expected = Enumerable.Range(0, 5).Select(_ => rng.NextDouble()).ToArray();

            net.Calculate(input);
            var result = net.LayerValues.Last();

            while (result.CalculateError(expected) > 0.0001)
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
                case 0: return new[] { 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 1: return new[] { 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 2: return new[] { 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 3: return new[] { 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 4: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 5: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 6: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d };
                case 7: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d };
                case 8: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d };
                case 9: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d };
                default:
                    throw new ArgumentException("invalid label " + label);
            }
        }

        /// <summary> Maps a number from 0 to 255 to a decimal between 0 and 1 </summary>
        static double MapInput(byte b)
            => (double)b / 2550;

    }
}
