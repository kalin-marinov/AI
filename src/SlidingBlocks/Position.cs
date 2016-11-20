using System;

namespace SlidingBlocks
{
    public class Position
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public Position(int row, int col)
        {
            this.Row = row;
            this.Column = col;
        }

        public override string ToString() => $"({Row} {Column})";

        /// <summary> Calculates the manhattan distance to another position </summary>
        public int DistanceTo(Position other)
            => Math.Abs(this.Row - other.Row) + Math.Abs(this.Column - other.Column);


        /// <summary> Returns a new position that is right next to the current in the given direction</summary>
        public Position GetAdjacent(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right: return new Position(Row, Column - 1);
                case Direction.Left: return new Position(Row, Column + 1);
                case Direction.Down: return new Position(Row - 1, Column);
                case Direction.Up: return new Position(Row + 1, Column);
                default: throw new System.Exception();
            }
        }
    }
}
