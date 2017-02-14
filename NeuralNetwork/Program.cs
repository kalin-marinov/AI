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
            var net = new Network(3, 5, 3);
            var weights = net.GetWeights(0);

            net.SetInput(new[] { 3.0, 2.5, 8.1 });
            var result = net.GetOutputs();
        }

    }
}
