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
            var expected = new[] { 0.25, 0.5, 0.7 };
            var result = net.GetOutputs();

            while (Math.Abs(result.CalculateError(expected).Sum()) > 0.00001)
            {
                Console.WriteLine($"Result: [{string.Join(",", result)}]");
                TrainingHelper.BackPropagate(net, expected);

                result = net.GetOutputs();
            }
        }
    }
}
