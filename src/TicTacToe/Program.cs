using System;
using System.Linq;

namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var field = new GameField();

            field.AddX(1, 1);
            field.AddO(0, 1);

            field.Print();
        }


        /// <summary> Alpha beta pruning algorithm where X is the AI (maximizing) and O is the player (minimizing) </summary>
        public int AlphaBeta(GameField node, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0 || (!node.GetPossibleOMoves().Any() && !node.GetPossibleXMoves().Any()))
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
