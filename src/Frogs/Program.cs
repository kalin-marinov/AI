using System;
using System.Linq;

namespace Frogs
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Enter frogs count:");
            int frogsCount = int.Parse(Console.ReadLine());

            var initialState = new GameField(frogsCount);

            var result = DFS(initialState);
            result.PrintPath();

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        /// <summary> A Recursive Depth-First-Search implementation</summary>
        public static GameField DFS(GameField field)
        {
            if (field.IsDestination())
                return field;

            return field.GetPossibleMoves()
                .Select(move => DFS(move))
                .FirstOrDefault(dfsResult => dfsResult != null);
        }

        static void PrintPath(this GameField field)
        {
            field.PreviousState?.PrintPath();
            field.Print();
        }
    }
}
