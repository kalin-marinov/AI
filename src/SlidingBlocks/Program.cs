using System;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

namespace SlidingBlocks
{
    public class Program
    {
        static OrderedBag<GameState> movesQueue;
        static HashSet<GameState> visitedStates;

        public static void Main()
        {
            Console.WriteLine("Please input the initial game state:");
            var field = GameState.ReadFromConsole();

            movesQueue = new OrderedBag<GameState> { field };
            visitedStates = new HashSet<GameState> { field };

            while (movesQueue.Any())
            {
                var result = PlayRound();

                if (result != null)
                {
                    PrintDirections(result);
                    return;
                }
            }
        }


        static GameState PlayRound()
        {
            var field = movesQueue.RemoveFirst();

            if (field.Score == 0) return field;

            if (field.ZeroPosition.Column > 0)
            {
                var newState = field.GenerateMove(Direction.Right);
                if (newState.Score == 0) return newState;
                AddToQueueIfNeeded(newState);
            }
            if (field.ZeroPosition.Column < field.RowsCount - 1)
            {
                var newState = field.GenerateMove(Direction.Left);
                if (newState.Score == 0) return newState;
                AddToQueueIfNeeded(newState);
            }
            if (field.ZeroPosition.Row > 0)
            {
                var newState = field.GenerateMove(Direction.Down);
                if (newState.Score == 0) return newState;
                AddToQueueIfNeeded(newState);
            }

            if (field.ZeroPosition.Row < field.RowsCount - 1)
            {
                var newState = field.GenerateMove(Direction.Up);
                if (newState.Score == 0) return newState;
                AddToQueueIfNeeded(newState);
            }

            return null;
        }

        static void AddToQueueIfNeeded(GameState newState)
        {
            if (!visitedStates.Contains(newState))
            {
                movesQueue.Add(newState);
                visitedStates.Add(newState);
            }
        }

        static void PrintDirections(GameState state)
        {
            var directions = new Stack<Direction>();

            Console.WriteLine(state.GenerationPath.Count);
            state.GenerationPath.ForEach(direction => Console.WriteLine(direction));
        }
    }
}
