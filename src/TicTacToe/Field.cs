using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public class GameField
    {
        public const int FieldSize = 3;
        public const bool X = true;
        public const bool O = false;

        private bool?[,] field;

        public GameField(bool?[,] fieldData)
        {
            field = fieldData;
        }

        public GameField()
        {
            field = new bool?[FieldSize, FieldSize];
        }


        public void AddX(int row, int col) => field[row, col] = X;

        public void AddO(int row, int col) => field[row, col] = O;

        public int GetScore()
        {
            // Check rows:
            for (int col = 0; col < FieldSize; col++)
            {
                if (Enumerable.Range(0, FieldSize).All(row => field[row, col] == X))
                    return 100;

                if (Enumerable.Range(0, FieldSize).All(row => field[row, col] == O))
                    return -100;
            }

            // Check cols:
            for (int row = 0; row < FieldSize; row++)
            {
                if (Enumerable.Range(0, FieldSize).All(col => field[row, col] == X))
                    return 100;

                if (Enumerable.Range(0, FieldSize).All(col => field[row, col] == O))
                    return -100;
            }

            // Check Diags
            if (Enumerable.Range(0, FieldSize).All(index => field[index, index] == X))
                return 100;

            if (Enumerable.Range(0, FieldSize).All(index => field[index, index] == O))
                return -100;


            if (Enumerable.Range(0, FieldSize).All(index => field[index, FieldSize - 1 - index] == X))
                return 100;

            if (Enumerable.Range(0, FieldSize).All(index => field[index, FieldSize - 1 - index] == O))
                return -100;

            return 0;
        }

        public Tuple<int, int> PreviousMove { get; set; }

        public void Print() => Console.WriteLine(this);

        public IEnumerable<GameField> GetPossibleXMoves()
        {
            for (int row = 0; row < FieldSize; row++)
                for (int col = 0; col < FieldSize; col++)
                    if (field[row, col] == null)
                    {
                        var copy = this.Clone();
                        copy.AddX(row, col);
                        copy.PreviousMove = Tuple.Create(row, col);
                        yield return copy;
                    }
        }

        public IEnumerable<GameField> GetPossibleOMoves()
        {
            for (int row = 0; row < FieldSize; row++)
                for (int col = 0; col < FieldSize; col++)
                    if (field[row, col] == null)
                    {
                        var copy = this.Clone();
                        copy.AddO(row, col);
                        copy.PreviousMove = Tuple.Create(row, col);
                        yield return copy;
                    }
        }

        public GameField Clone()
        {
            var newField = new bool?[FieldSize, FieldSize];

            for (int row = 0; row < FieldSize; row++)
                for (int col = 0; col < FieldSize; col++)
                    newField[row, col] = this.field[row, col];

            return new GameField(newField);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int row = 0; row < FieldSize; row++)
            {
                sb.Append("|");
                for (int col = 0; col < FieldSize; col++)
                {
                    if (field[row, col] == X)
                        sb.Append("X|");

                    else if (field[row, col] == O)
                        sb.Append("O|");

                    else
                        sb.Append(" |");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
