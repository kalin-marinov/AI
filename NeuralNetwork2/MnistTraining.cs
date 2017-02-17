using NeuralNetwork.Readers;
using NeuralNetwork2.Events;
using System;
using System.Linq;

namespace NeuralNetwork
{
    public delegate void EpochChangeEventHandler(object sender, EpochChangeEventArgs e);

    public class MNistTraining
    {
        public event EpochChangeEventHandler Changed;

        public Network TrainNetwork(int maxEpoch = 200, int hiddenUnits = 100, int samplesCount = 200)
        {
            var trainImages = ImageDataReader.ReadImageFile(@"Data\train-images.idx3-ubyte").Take(samplesCount);
            var trainLabels = ImageDataReader.ReadLabels(@"Data\train-labels.idx1-ubyte").Take(samplesCount);
            var trainItems = trainImages.Zip(trainLabels, (img, lbl) => Tuple.Create(img, lbl)).ToList();

            var net = new Network(784, hiddenUnits, 10);

            int epoch = 0;
            while (epoch < maxEpoch)
            {
                for (int i = 0; i < trainItems.Count; i++)
                {
                    var image = trainItems[i].Item1;
                    var input = image.Select(MapInput).ToArray();
                    var expected = trainItems[i].Item2.ToProbabilityArray();

                    net.Calculate(input);

                    // Back propagate
                    // var result = net.LayerValues.Last();

                    net.BackPropagate(expected);
                    //net.Calculate(input);
                    //var result2 = net.LayerValues.Last();

                    //if (epoch > 200)
                    //    Console.WriteLine($"Trained for image {i} Initial:{ArrayToDigit(result)} After backprop: {ArrayToDigit(result)} Actual: {trainItems[i].Item2}  | Epoch {epoch}");

                }

                Changed?.Invoke(this, new EpochChangeEventArgs(epoch, maxEpoch));
                trainItems = trainItems.Shuffle();
                epoch++;
            }

            return net;
        }

        public void TestNetwork(Network net)
        {
            // Test:
            var testImages = ImageDataReader.ReadImageFile(@"Data\t10k-images.idx3-ubyte").Take(50).ToList();
            var testLabels = ImageDataReader.ReadLabels(@"Data\t10k-labels.idx1-ubyte").Take(50).ToList();

            int succeed = 0;
            for (int i = 0; i < testImages.Count; i++)
            {
                var image = testImages[i];
                var expected = testLabels[i];
                var input = image.Select(MapInput).ToArray();

                var result = net.Calculate(input).ToDigit();
                Console.WriteLine($"Processing image {i} Guess: {result} Actual: {expected}");

                if (result == expected)
                    succeed++;

            }

            Console.WriteLine($@"Sucess: {succeed} \ {testImages.Count}");
        }

        /// <summary> Maps a number from 0 to 255 to a decimal between 0 and 1 </summary>
        public static double MapInput(byte b)
            => (double)b / 255;

    }
}
