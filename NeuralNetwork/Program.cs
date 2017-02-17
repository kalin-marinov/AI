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
            SimpleTest();

            var trainImages = ImageDataReader.ReadImageFile(@"Data\train-images.idx3-ubyte").Take(200).ToList();
            var trainLabels = ImageDataReader.ReadLabels(@"Data\train-labels.idx1-ubyte").Take(200).ToList();

            var testImages = ImageDataReader.ReadImageFile(@"Data\t10k-images.idx3-ubyte").Take(50).ToList();
            var testLabels = ImageDataReader.ReadLabels(@"Data\t10k-labels.idx1-ubyte").Take(50).ToList();

            var trainItems = trainImages.Zip(trainLabels, (img, lbl) => Tuple.Create(img, lbl)).ToList();
            var net = new Network(784, 100, 10);

            int epoch = 0;

            while (epoch < 200)
            {
                ++epoch;
                // Train
                for (int i = 0; i < trainItems.Count; i++)
                {
                    var image = trainItems[i].Item1;
                    var input = image.Select(MapInput).ToArray();
                    var output = DigitToArray(trainItems[i].Item2);


                    // Back propagate
                    var result = net.Calculate(input);

                    TrainingHelper.BackPropagate(net, output);
                    var result2 = net.Calculate(input);

                    if (epoch > 100)
                        Console.WriteLine($"Trained for image {i} Guess: {ArrayToDigit(result)} After backrpop: {ArrayToDigit(result2)}  Actual: {trainItems[i].Item2}");
                }

                trainItems = trainItems.Shuffle();
                Console.WriteLine("Epoch: " + epoch);
            }



            // Test:
            for (int i = 0; i < testImages.Count; i++)
            {
                var image = testImages[i];
                var input = image.Select(b => (double)b).ToArray();
                var output = testLabels[i];

                var result = net.Calculate(input);
                Console.WriteLine($"Processing image {i} Guess: {ArrayToDigit(result)} Actual: {output}");
            }

            Console.ReadLine();
        }

        static void SimpleTest()
        {
            var net = new Network(6, 10, 5);
            var rng = new Random();

            var input = Enumerable.Range(0, 6).Select(_ => rng.NextDouble()).ToArray();
            var expected = Enumerable.Range(0, 5).Select(_ => rng.NextDouble()).ToArray();
          
            var result = net.Calculate(input);

            while (result.CalculateError(expected) > 0.0001)
            {
                Console.WriteLine($"Result: [{string.Join(",", result)}]");
                TrainingHelper.BackPropagate(net, expected);
                result = net.Calculate(input);
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
            => (double)b / 255;

    }
}
