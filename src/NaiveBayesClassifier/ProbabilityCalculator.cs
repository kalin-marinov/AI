using System;
using System.Collections.Generic;

namespace NaiveBayesClassifier
{
    public class ProbabilityCalculator
    {
        Dictionary<AverageKey, float> probabilities = new Dictionary<AverageKey, float>();

        private int democrats;
        private int republicans;
        private int total;

        public ProbabilityCalculator(IEnumerable<VotesItem> votes)
        {
            var countPerTypeParam = new Dictionary<AverageKey, int>();

            foreach (var vote in votes)
            {
                if (vote.Type == Type.Democrat)
                    democrats++;

                else
                    republicans++;

                foreach (var param in vote.Parameters)
                {
                    var key = new AverageKey { Parameter = param.Key, Value = param.Value, Type = vote.Type };
                    countPerTypeParam.Increment(key); // Increases the value with 1
                }
            }

            // P(Type | Param == Value) = (Count of entries of the type with Param == Value) / (Total for Type)
            foreach (var count in countPerTypeParam)
            {
                var probability = count.Key.Type == Type.Democrat ? (float)count.Value / democrats
                                                                  : (float)count.Value / republicans;
                probabilities.Add(count.Key, probability);
            }

            total = democrats + republicans;
        }



        /// <summary>
        /// Gets the conditional probability for a given type if the given parameter has the given value
        /// <para> ex: P(Republican | paramter1 == true) </para>
        /// </summary>
        public float GetProbability(Type type, int parameter, bool? value)
        {
            var key = new AverageKey { Type = type, Parameter = parameter, Value = value };
            return probabilities.ContainsKey(key) ? probabilities[key] : 0;
        }

        /// <summary> Gets the probability for a type </summary>
        public float GetProbability(Type type)
            => type == Type.Democrat ? (float)democrats / total
                                     : (float)republicans / total;


        /// <summary> Simple Tuple of 3 parameters - very useful for dictonary key </summary>
        private class AverageKey : IEquatable<AverageKey>
        {
            public int Parameter { get; set; }

            public bool? Value { get; set; }

            public Type Type { get; set; }

            public bool Equals(AverageKey other)
                => this.Parameter == other.Parameter && this.Value == other.Value && this.Type == other.Type;

            public override int GetHashCode()
            {
                int hash = 17;

                unchecked
                {
                    hash *= 23 + Parameter;
                    hash *= 23 + Value.GetHashCode();
                    hash *= 23 + Type.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
