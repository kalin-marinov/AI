using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public static class TrainingHelper
    {
        public static double[] CalculateError(this IList<double> actual, IList<double> expected)
           => Enumerable.Range(0, expected.Count).Select(i => expected[i] - actual[i]).ToArray();

        public static double[] Multiply(IList<double> first, IList<double> second)
           => Enumerable.Range(0, first.Count).Select(i => first[i] * second[i]).ToArray();



        /// <summary> dErr/dOut * dOut/dNet - production of the drivatives </summary>
        public static double OutputDelta(double expected, double actual)
            => -(expected - actual) *  (actual * (1 - actual));
        

        /// <summary> Back propagation implementation for three layer net only </summary>
        public static void BackPropagate(Network net, IList<double> expectedResult)
        {
            // Adjust Hidden -> Output weights
            var errors = new Dictionary<Neuron, double>(); // store those derivatives - we'll need 'em later

            var output = net.OutputLayer;
            for (int i = 0; i < output.Count; i++)
            {
                var neuron = output[i];
                var errorDelta = OutputDelta(neuron.GetValue(), expectedResult[i]);
                errors.Add(neuron, errorDelta);

                foreach (var synapse in neuron.InputNodes)
                {
                    // dErr/dWeight = dErr/dOut * dOut/dNet * dNet/dWeight
                    var gradient = errorDelta * synapse.SourceNeuron.GetValueWithoutActivation();
                    synapse.Weight += gradient;
                }
            }

            // Adjust Input -> Hidden weights
            var hidden = net.Layers[1];

            foreach (var neuron in hidden)
            {
                var value = neuron.GetValue();
                var derivative = value * (1 - value);

                // calculate sum of errorDelta of each connected output node
                var sum = 0.0;
                foreach (var synapse in neuron.OutputNodes)
                {
                    sum += synapse.Value * errors[synapse.TargetNeuron];
                }

                // Proceed to modifying the weights of the input nodes
                foreach (var synapse in neuron.InputNodes)
                {
                    var gradient = synapse.SourceNeuron.GetValue() * sum * derivative;
                    synapse.Weight += gradient;
                }
            }
        }
    }
}
