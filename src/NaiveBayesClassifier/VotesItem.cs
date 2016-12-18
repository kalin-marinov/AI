using System.Collections.Generic;

namespace NaiveBayesClassifier
{
    public class VotesItem
    {
        public Type Type { get; set; }

        public Dictionary<int, bool?> Parameters { get; set; }

        public VotesItem(string line, string separator = ",")
        {
            var properties = line.Split(separator);
            Type = properties[0].ToType();
            Parameters = new Dictionary<int, bool?>();

            for (int i = 1; i <= 16; i++)
            {
                Parameters.Add(i, properties[i].ToBool());
            }
        }
    }
}
