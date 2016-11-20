namespace Knapsack
{
    public class Item
    {
        public long Weight { get; private set; }

        public long Value { get; private set; }

        public Item(long weight, long value)
        {
            this.Weight = weight;
            this.Value = value; 
        }
    }
}
