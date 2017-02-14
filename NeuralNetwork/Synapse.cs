using System;

namespace NeuralNetwork
{
    public class Synapse
    {
        public double Weight { get; set; }

        public Neuron SourceNeuron { get; set; }
        
        public Neuron TargetNeuron { get; set; }

        public double Value
             => SourceNeuron.GetValue() * Weight;



        static Random rng = new Random();

        /// <summary> Creates a synapse connecting the source and destination, with a random weight between 0 and 1</summary>
        public static Synapse Randomized(Neuron source, Neuron destination)
            => new Synapse { SourceNeuron = source,  TargetNeuron = destination, Weight = rng.NextDouble() };

    }
}
