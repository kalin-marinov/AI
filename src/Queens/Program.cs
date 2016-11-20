using System;
using System.Linq;
using System.Text;

namespace Queens
{
    public class Program
    {
        const int MaxIterations = 1000;

        static int queensCount;
        static int[] queenPositions;

        /// <summary> Determines whether there is a queen on the given position </summary>
        static bool IsQueen(int row, int col) => queenPositions[col] == row;

        public static void Main(string[] args)
        {
            queensCount = int.Parse(Console.ReadLine());
            InitializeField();

            int i = 0;

            while (true)
            {
                for (int col = 0; col < queensCount; col++)
                {
                    int minConflicts = int.MaxValue;
                    int minConflictRow = queenPositions[col];

                    for (int row = 0; row < queensCount; row++)
                    {
                        int conflicts = GetConflicts(row, col);

                        if (conflicts < minConflicts)
                        {
                            minConflicts = conflicts;
                            minConflictRow = row;
                        }
                    }


                    // Move the queen to the minConflictRow if needed
                    if (minConflictRow != queenPositions[col])
                        queenPositions[col] = minConflictRow;

                    if (IsSolved())
                    {
                        Console.WriteLine(GetFieldString());
                        return;
                    }
                }

                i++;

                if (i % 1000 == 0)
                    InitializeField();
            }
        }

        static void InitializeField()
        {
            queenPositions = new int[queensCount];
            var rng = new Random();

            for (int col = 0; col < queensCount; col++)
            {
                int randomPos = rng.Next(0, queensCount);
                queenPositions[col] = randomPos;
            }
        }

        /// <summary> Get the number of queens conflicting the given position  </summary>
        static int GetConflicts(int row, int col)
        {
            // Amount of queens on the same row (different from the current one)
            var queensOnTheRow = Enumerable.Range(0, queensCount).Where(c => c != col).Count(c => IsQueen(row, c));
            int queensOnTheDiagonal = 0;

            // Search Down & Right
            queensOnTheDiagonal += SearchDiagonal(row, col, true, true);

            // Search Up & Left
            queensOnTheDiagonal += SearchDiagonal(row, col, false, false);

            // Search Up & Right
            queensOnTheDiagonal += SearchDiagonal(row, col, false, true);

            // Search Down & Left
            queensOnTheDiagonal += SearchDiagonal(row, col, true, false);

            return queensOnTheRow + queensOnTheDiagonal;
        }

        static bool IsSolved()
        {
            for (int col = 0; col < queensCount; col++)
            {
                if (GetConflicts(queenPositions[col], col) != 0)
                    return false;
            }

            return true;
        }


        static int SearchDiagonal(int row, int col, bool increaseRows, bool increaseCols)
        {
            int dRow = increaseRows ? 1 : -1;
            int dCol = increaseCols ? 1 : -1;

            int r = row + dRow;
            int c = col + dCol;
            int count = 0;

            while (0 <= r && r < queensCount && 0 <= c && c < queensCount)
            {
                if (IsQueen(r, c) && !(r == row && c == col))
                    count++;

                r += dRow;
                c += dCol;
            }

            return count;
        }


        static string GetFieldString()
        {
            var result = new StringBuilder();

            for (int row = 0; row < queensCount; row++)
            {
                for (int col = 0; col < queensCount; col++)          
                    result.Append(IsQueen(row, col) ? "*" : "_");
                
                result.AppendLine();
            }

            return result.ToString();
        }
    }
}
