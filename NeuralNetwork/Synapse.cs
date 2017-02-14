using System;

namespace NeuralNetwork
{
    public class Synapse
    {
        public double Weight { get; set; }

        public Neuron SourceNeuron { get; set; }

        public double Value
             => SourceNeuron.GetValue() * Weight;



        static Random rng = new Random();

        /// <summary> Creates a synapse connected to the source, with a random weight between 0 and 1</summary>
        public static Synapse Randomized(Neuron source)
            => new Synapse { SourceNeuron = source, Weight = rng.NextDouble() };

    }
}
