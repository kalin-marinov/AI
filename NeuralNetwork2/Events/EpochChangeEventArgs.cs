using System;

namespace NeuralNetwork2.Events
{
    public class EpochChangeEventArgs : EventArgs
    {
        public int Epoch { get; set; }

        public int MaxEpoch { get; set; }

        public EpochChangeEventArgs(int epoch, int maxEpoch)
        {
            this.Epoch = epoch;
            this.MaxEpoch = maxEpoch;
        }
    }
}
