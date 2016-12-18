using System;
using System.Text;

namespace KNN
{
    public class IrisItem
    {
        public double SepalLength { get; set; }

        public double SepalWidth { get; set; }

        public double PetalLength { get; set; }

        public double PetalWidth { get; set; }

        public IrisClass IrisClass { get; set; }

        public IrisItem(int sepalLength, int sepalWidth, int petalLength, int petalWidth, IrisClass irisClass)
        {
            SepalLength = sepalLength;
            SepalWidth = sepalWidth;
            PetalLength = petalLength;
            PetalWidth = petalWidth;
            IrisClass = irisClass;
        }

        public IrisItem(string line, string separator = ",")
        {
            var properties = line.Split(separator);

            SepalLength = double.Parse(properties[0]);
            SepalWidth = double.Parse(properties[1]);
            PetalLength = double.Parse(properties[2]);
            PetalWidth = double.Parse(properties[3]);
            IrisClass = properties[4].ToIrisClass();
        }

        public double DistanceTo(IrisItem other)
        {
            var dSepalLength = Math.Pow(this.SepalLength - other.SepalLength, 2);
            var dSepalWidth = Math.Pow(this.SepalWidth - other.SepalWidth, 2);
            var dPetalLength = Math.Pow(this.PetalLength - other.PetalLength, 2);
            var dPetalWidth = Math.Pow(this.PetalWidth - other.PetalWidth, 2);

            var distance = Math.Sqrt(dSepalLength + dSepalWidth + dPetalLength + dPetalWidth);
            return distance;
        }

        public override string ToString()
        =>     $"Sepal Length: {SepalLength}, SepalWidth: {SepalWidth}, PetalLength: {PetalLength}, " 
        +      $"PetalWidth: {PetalWidth}, Class: {IrisClass}";  
    }
}
