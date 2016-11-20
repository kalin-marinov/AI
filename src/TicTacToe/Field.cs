using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    public class GameField
    {
        public static int FieldSize = 3;
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
            throw new NotImplementedException();
        }

        public void Print() => Console.WriteLine(this);

        public IEnumerable<GameField> GetPossibleXMoves()
        {
            for (int row = 0; row < FieldSize; row++)
                for (int col = 0; col < FieldSize; col++)
                    if (field[row, col] == null)
                    {
                        var copy = this.Clone();
                        copy.AddX(row, col);
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
