using System;
using System.Linq;

namespace TicTacToe
{
    public class Program
    {
        static int MaxDepth = GameField.FieldSize * GameField.FieldSize;

        public static void Main(string[] args)
        {
            var field = new GameField();
            field.Print();


            for (int movesCount = MaxDepth; movesCount > 0 && field.GetScore() == 0; movesCount--)
            {
                var input = Console.ReadLine().Split(' ').Select(int.Parse);
                field.AddO(input.First(), input.Last());
                movesCount--;

                if (movesCount == 0)
                {
                    field.Print();
                    break;
                }

                // Computer move:
                var bestMove = BeginSearch(field, movesCount);
                field.AddX(bestMove.Item1, bestMove.Item2);

                field.Print();
            }

            Console.WriteLine("Game over");
            Console.ReadLine();
        }


        // Begin - maximizing
        static Tuple<int, int> BeginSearch(GameField node, int depth)
        {
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            int v = int.MinValue;
            Tuple<int, int> bestMove = null;

            foreach (var child in node.GetPossibleXMoves())
            {
                v = Math.Max(v, AlphaBeta(child, depth - 1, alpha, beta, false));

                if (v > alpha)
                {
                    alpha = Math.Max(alpha, v);
                    bestMove = child.PreviousMove;
                }

                if (beta <= alpha)
                    break; // beta cut - off                    
            }

            return bestMove;
        } 


        /// <summary> Alpha beta pruning algorithm where X is the AI (maximizing) and O is the player (minimizing) </summary>
        static int AlphaBeta(GameField node, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0 || node.GetScore() != 0)
            {
               return node.GetScore();
            }

            if (maximizingPlayer)
            {
                int v = int.MinValue;

                foreach (var child in node.GetPossibleXMoves())
                {
                    v = Math.Max(v, AlphaBeta(child, depth - 1, alpha, beta, false));
                    alpha = Math.Max(alpha, v);

                    if (beta <= alpha)
                        break; // beta cut - off                    
                }

                return v;
            }
            else
            {
                int v = int.MaxValue;
                foreach (var child in node.GetPossibleOMoves())
                {
                    v = Math.Min(v, AlphaBeta(child, depth - 1, alpha, beta, true));
                    beta = Math.Min(beta, v);

                    if (beta < alpha)
                        break; // alpha cut - off
                }

                return v;
           }
        }
    }
}
