using NeuralNetwork.Readers;
using NeuralNetwork2.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class Program
    {
        public static void Main()
        {
            // TrainAndTest();
            TestWithSavedWeights();
        }

        private static void TrainAndTest()
        {
            var trainer = new MNistTraining();
            trainer.Changed += Trainer_Changed;

            var net = trainer.TrainNetwork(150, 80, 20000);

            File.WriteAllText("weights0.txt", net.Weights[0].GetMatrixString());
            File.WriteAllText("weights1.txt", net.Weights[1].GetMatrixString());

            trainer.TestNetwork(net);
        }

        public static void TestWithSavedWeights()
        {
            var trainer = new MNistTraining();
            var net = new Network(784, 50, 10);
            net.LoadWeightsFromFile(0, "weights0.txt");
            net.LoadWeightsFromFile(1, "weights1.txt");
            trainer.TestNetwork(net);
        }

        private static void Trainer_Changed(object sender, EpochChangeEventArgs e)
        {
            Console.WriteLine($"Epoch: {e.Epoch} / {e.MaxEpoch} ");
            Console.CursorTop--;
        }
    }
}
