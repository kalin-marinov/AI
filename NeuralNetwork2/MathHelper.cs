using System;

namespace NeuralNetwork
{
    public static class MathHelper
    {
        public static double Sigmoid(double value)
            =>  1.0d / (1.0d + Math.Exp(-value));


        public static double[] Softmax(double[] netValues)
        {
            double sum = 0.0;
            for (int i = 0; i < netValues.Length; ++i)
                sum += Math.Exp(netValues[i]);

            double[] result = new double[netValues.Length];
            for (int i = 0; i < netValues.Length; ++i)
                result[i] = Math.Exp(netValues[i]) / sum;

            return result; // now scaled so that xi sum to 1.0
        }

    }

}
