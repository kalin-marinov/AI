using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlidingBlocks
{
    /// <summary> Represents a state of the game. The game may transition in another state by moving the "0" element to an adjacent cell </summary>
    public class GameState : IComparable<GameState>, IEquatable<GameState>
    {
        byte[,] field;
        int? score;

        byte maxNumber => (byte)(field.Length - 1);

        public int RowsCount => field.GetLength(dimension: 0);

        public int ColumnsCount => field.GetLength(dimension: 1);

        public Position ZeroPosition { get; private set; }

        /// <summary> The directions in which the "0" item was moved from the start to reach this state </summary>
        public List<Direction> GenerationPath { get; private set; }

        public int Score => score.HasValue ? score.Value : CalculateScore();
         
        public GameState(byte[,] field, Position zeroPosition)
        {
            this.field = field;
            this.ZeroPosition = zeroPosition;
            this.GenerationPath = new List<Direction>();
        }


        /// <summary> Returns the score of the field compared to the <see cref="DesiredGoal"/> field, using Manhattan distance
        /// <para> 0 means the state is identical to the goal </para>
        /// </summary>
        private int CalculateScore()
        {
            int sum = 0;

            for (int row = 0; row < RowsCount; row++)
                for (int col = 0; col < ColumnsCount; col++)
                {
                    var number = field[row, col];

                    if (number == 0) continue;

                    var currentPosition = new Position(row, col);
                    var desiredPosition = new Position(
                        row: (number - 1) / ColumnsCount,
                        col: (number - 1) % ColumnsCount);

                    sum += currentPosition.DistanceTo(desiredPosition);
                }

            score = GenerationPath.Count + sum;
            return sum;
        }

        /// <summary> Moves the zero element to a new position </summary>
        public void MoveZero(Position newPosition)
        {
            field[ZeroPosition.Row, ZeroPosition.Column] = field[newPosition.Row, newPosition.Column];
            field[newPosition.Row, newPosition.Column] = 0;
            ZeroPosition = newPosition;
        }

        /// <summary> Creates a copy of the current field </summary>
        public GameState Clone()
        {
            var result = new byte[RowsCount, ColumnsCount];

            for (byte row = 0; row < RowsCount; row++)
                for (byte col = 0; col < ColumnsCount; col++)
                    result[row, col] = field[row, col];

            return new GameState(result, ZeroPosition);
        }

        /// <summary> Creates a copy of the current field with the "0" moved in the given direction </summary>
        public GameState GenerateMove(Direction direction)
        {
            var adjacentPos = ZeroPosition.GetAdjacent(direction);

            var fieldCopy = this.Clone();
            fieldCopy.MoveZero(adjacentPos);
            fieldCopy.GenerationPath.AddRange(this.GenerationPath);
            fieldCopy.GenerationPath.Add(direction);

            return fieldCopy;
        }

        public int CompareTo(GameState other) => this.Score.CompareTo(other.Score);

        public bool Equals(GameState other)
        {
            for (int row = 0; row < RowsCount; row++)
                for (int col = 0; col < ColumnsCount; col++)
                    if (this.field[row, col] != other.field[row, col])
                        return false;

            return true;
        }

        public override int GetHashCode()
        {
            unchecked // Int overflow is fine here
            {
                int hash = 17;
                for (int row = 0; row < RowsCount; row++)
                    for (int col = 0; col < ColumnsCount; col++)
                        hash = hash * 31 + field[row, col];

                return hash;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int row = 0; row < RowsCount; row++)
            {
                for (int col = 0; col < ColumnsCount; col++)
                {
                    sb.AppendFormat($"{field[row, col]} ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary> Reads a game field from the console input </summary>
        public static GameState ReadFromConsole()
        {
            int size = int.Parse(Console.ReadLine()) + 1;
            int rowsCount = (int)Math.Sqrt(size);
            var field = new byte[rowsCount, rowsCount];

            Position zeroPosition = null;

            for (int i = 0; i < rowsCount; i++)
            {
                var blocks = Console.ReadLine().Split(' ').Select(byte.Parse).ToArray();

                for (int j = 0; j < rowsCount; j++)
                {
                    field[i, j] = blocks[j];

                    if (blocks[j] == 0)
                        zeroPosition = new Position(i, j);
                    
                }
            }

            return new GameState(field, zeroPosition);
        }
    }
}
