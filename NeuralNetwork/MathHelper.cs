using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public static class MathHelper
    {
        public static double Sigmoid(double value)
            => 1.0d / (1.0d + Math.Exp(-value));
    }
}
