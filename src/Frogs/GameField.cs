using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frogs
{
    public class GameField
    {
        const bool LEFT = true;
        const bool RIGHT = false;

        private int frogsCount;

        private int length;
        private bool?[] field;
        private int emptySpot;

        /// <summary> The field from which the current was generated, null if none </summary>
        public GameField PreviousState { get; private set; }

        /// <summary> Creates a game field by given field configuration </summary>
        public GameField(bool?[] field)
        {
            this.field = field;
            length = field.Length;
            emptySpot = field.IndexOf(null);
        }

        /// <summary> Creates the default/initial field by given frogs count </summary>
        public GameField(int frogsCount)
        {
            this.frogsCount = frogsCount;
            Initialize();
        }

        void Initialize()
        {
            length = 2 * frogsCount + 1;
            field = new bool?[length];
            emptySpot = length / 2;

            // First group should look to the RIGHT initially
            for (int i = 0; i < emptySpot; i++)
                field[i] = RIGHT;

            // Second group should look to the LEFT initially
            for (int i = emptySpot + 1; i < length; i++)
                field[i] = LEFT;
        }

        public void Print() => Console.WriteLine(this);

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var pos in Enumerable.Range(0, length))
            {
                if (field[pos] == LEFT) result.Append("<");
                else if (field[pos] == RIGHT) result.Append(">");
                else result.Append("_");
            }

            return result.ToString();
        }


        /// <summary> Determines weather the given field is the destination (the desired state) </summary>
        public bool IsDestination()
        {
            for (int i = 0; i < emptySpot; i++)
                if (field[i] != LEFT) return false;

            for (int i = emptySpot + 1; i < length; i++)
                if (field[i] != RIGHT) return false;

            return true;
        }

        /// <summary> Swaps the <see cref="emptySpot"/> with another item in the field by its given position </summary>
        public void MoveEmptySpot(int newPosition)
        {
            field[emptySpot] = field[newPosition];
            field[newPosition] = null;
            emptySpot = newPosition;
        }

        /// <summary>  Returns a collection of field-states that can be reached from the given one, after a single valid move </summary>
        public IEnumerable<GameField> GetPossibleMoves()
        {
            // There is a RIGHT looking frog on the left scenario:
            if (emptySpot > 0 && field[emptySpot - 1] == RIGHT)
                yield return GenererateAndMove(emptySpot - 1);

            // There is a RIGHT looking frog that can jump over a leftie scenario:
            if (emptySpot > 1 && field[emptySpot - 1] == LEFT && field[emptySpot - 2] == RIGHT)
                yield return GenererateAndMove(emptySpot - 2);

            // There is a LEFT looking frog on the right scenario:
            if (emptySpot < length - 1 && field[emptySpot + 1] == LEFT)
                yield return GenererateAndMove(emptySpot + 1);

            // There is a LEFT looking frog, that can jump over a rightie scenario:
            if (emptySpot < length - 2 && field[emptySpot + 1] == RIGHT && field[emptySpot + 2] == LEFT)
                yield return GenererateAndMove(emptySpot + 2);
        }

        /// <summary> Generates a copy of the current field, that has the empty spot moved to another position </summary>
        private GameField GenererateAndMove(int newEmptySpotPosition)
        {
            var fieldCopy = new bool?[length];
            Array.Copy(field, fieldCopy, length);

            var newState = new GameField(fieldCopy);
            newState.MoveEmptySpot(newEmptySpotPosition);
            newState.PreviousState = this;
            return newState;
        }
    }
}
