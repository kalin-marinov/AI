using System;
using System.Collections.Generic;
using System.Linq;

namespace Knapsack
{
    public class GenState
    {
        static Random rng = new Random(Seed: 13);

        /// <summary> If n-th item is true, it means the n-th item is taken  </summary>
        public bool[] SelectedItems { get; set; }

        public long Score { get; set; }

        public long Weight { get; set; }

        public GenState(bool[] selectedItems)
        {
            this.SelectedItems = selectedItems;
            var selectedItemIndexes = Enumerable.Range(0, selectedItems.Length).Where(i => selectedItems[i]);

            this.Weight = selectedItemIndexes.Sum(i => Program.AllItems[i].Weight);
            this.Score = Weight <= Program.MaxWeight ? selectedItemIndexes.Sum(i => Program.AllItems[i].Value) : 0;
        }

        public void Mutate()
        {
            var randomIndex = rng.Next(0, SelectedItems.Length);
            SelectedItems[randomIndex] = !SelectedItems[randomIndex];

            // Re-calculate score & weight
            var selectedItemIndexes = Enumerable.Range(0, SelectedItems.Length).Where(i => SelectedItems[i]);
            this.Weight = selectedItemIndexes.Sum(i => Program.AllItems[i].Weight);
            this.Score = Weight <= Program.MaxWeight ? selectedItemIndexes.Sum(i => Program.AllItems[i].Value) : 0;
        }

        public Tuple<GenState, GenState> CrossOver(GenState other)
        {
            var count = SelectedItems.Length;
            var randomIndex = rng.Next(0, SelectedItems.Length - 1);

            var firstChild = this.SelectedItems.Take(randomIndex).Concat(other.SelectedItems.Skip(randomIndex)).ToArray();
            var secondChild = other.SelectedItems.Take(randomIndex).Concat(this.SelectedItems.Skip(randomIndex)).ToArray();

            return Tuple.Create(new GenState(firstChild), new GenState(secondChild));
        }





        public static GenState GenerateRandomState()
        {
            var max = (int)Math.Pow(2, Program.AllItems.Count);
            var result = new bool[Program.AllItems.Count];

            var randomNumber = rng.Next(1, max);

            var boolArr = Convert.ToString(randomNumber, 2).Select(bit => bit == '1').ToArray();
            for (int i = 0; i < boolArr.Length; i++)
                result[i] = boolArr[i];

            return new GenState(result);
        }

        public static IEnumerable<GenState> GenerateRandomStates(int statesCount)
            => Enumerable.Range(0, statesCount).Select(_ => GenerateRandomState());

    }
}
