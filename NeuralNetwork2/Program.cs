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
            var trainer = new MNistTraining();
            trainer.Changed += Trainer_Changed;

            var net = trainer.TrainNetwork();

            File.WriteAllText("weights1.txt", net.Weights[0].GetMatrixString());
            File.WriteAllText("weights2.txt", net.Weights[1].GetMatrixString());

            trainer.TestNetwork(net);
        }

        private static void Trainer_Changed(object sender, EpochChangeEventArgs e)
        {
            Console.WriteLine($"Epoch: {e.Epoch} / {e.MaxEpoch} ");
            Console.CursorTop--;
        }
    }
}
