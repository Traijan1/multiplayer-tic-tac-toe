using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {
    public class TicTacToe {
        public char[,] Field { get; private set; } = new char[3, 3];
        public char DefaultChar { get; private set; }
        public bool IsPlayerOneTurn { get; set; } = true;
        public char CurrentChar { get { return IsPlayerOneTurn ? 'X' : 'O'; } }

        public void SetFieldLength(int length) {
            if (length < 3)
                length = 3;

            Field = new char[length, length];
        }

        public void ClearField(char symbol = ' ') {
            for (int x = 0; x < Field.GetLength(0); x++) {
                for (int y = 0; y < Field.GetLength(1); y++)
                    Field[x, y] = symbol;
            }

            DefaultChar = symbol;
        }

        public bool SetChar(int x, int y) {
            if (x < 0 || y < 0 || x >= Field.GetLength(0) || y >= Field.GetLength(1))
                return false;

            Field[x, y] = CurrentChar;
            return true;
        }

        public void NextRound() =>
            IsPlayerOneTurn = !IsPlayerOneTurn;

        public TicTacToeState DidSomeoneWon() {
            bool allSame = true;

            for (int x = 0; x < Field.GetLength(0); x++) {
                allSame = true;
                for (int y = 0; y < Field.GetLength(1); y++) {
                    Console.WriteLine($"X: {x} | Y: {y} | Field: {Field[x, y]}");
                    if (Field[x, y] != CurrentChar)
                        allSame = false;
                }

                if (allSame)
                    return TicTacToeState.Win;
            }

            for (int x = 0; x < Field.GetLength(0); x++) {
                allSame = true;
                for (int y = 0; y < Field.GetLength(1); y++) {
                    if (Field[y, x] != CurrentChar)
                        allSame = false;
                }

                if (allSame)
                    return TicTacToeState.Win;
            }

            allSame = true;
            for (int x = 0; x < Field.GetLength(0); x++) {
                if (Field[x, x] != CurrentChar)
                    allSame = false;
            }

            if (allSame)
                return TicTacToeState.Win;

            allSame = true;
            for (int x = 0, y = Field.GetLength(1) - 1; x < Field.GetLength(0); x++, y--) {
                if (Field[x, y] != CurrentChar)
                    allSame = false;
            }

            if (allSame)
                return TicTacToeState.Win;

            foreach (var c in Field) {
                if (c == DefaultChar)
                    return TicTacToeState.OnGoing;
            }

            return TicTacToeState.Draw;
        }
    }
}
