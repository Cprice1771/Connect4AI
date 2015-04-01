using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectRConsole
{
    class Program
    {
        private static List<List<SquareSimple>> _boardList;
        private static Turn _turn;

        static void Main(string[] args)
        {
            Console.WriteLine("How many Rows?");

            int rows = int.Parse(Console.ReadLine());

            Console.WriteLine("How many Columns?");

            int cols = int.Parse(Console.ReadLine());

            Console.WriteLine("Who goes first?");

            string firstTurn = Console.ReadLine();
            
            if(firstTurn.Contains("Red"))
                _turn = Turn.Red;
            else
                _turn = Turn.Black;
                
            for (int i = 0; i < rows; i++)
            {
                _boardList.Add(new List<SquareSimple>());
                for (int j = 0; j < cols; j++)
                {
                    SquareSimple s = new SquareSimple();
                    s.State = SquareState.Blank;
                    _boardList[i].Add(s);
                }
            }

            PrintBoard(_boardList);

            while (true)
            {
                Console.WriteLine("What Coloumn do you wish to play in?");
                int play = int.Parse(Console.ReadLine());

                MakePlay(play);

                PrintBoard(_boardList);
            }


        }

        private static bool MakePlay(int col)
        {
            SquareSimple square = null;
            for (int i = 0; i < _boardList.Count; i++)
            {
                if (_boardList[i][col].State == SquareState.Blank)
                    square = _boardList[i][col];

            }

            //If the row is full or the square given was invalid, do nothing
            if (square == null || square.State != SquareState.Blank)
                return false;

            if (_turn == Turn.Red)
            {
                square.State = SquareState.Red;
                _turn = Turn.Black;
            }
            else
            {
                square.State = SquareState.Black;
                _turn = Turn.Red;
            }
            return true;

        }

        public static void PrintBoard(List<List<SquareSimple>> board)
        {
            for (int i = 0; i < board[0].Count; i++)
                Console.Write("\t" + i);

            Console.WriteLine();

            for (int i = 0; i < board.Count; i++)
            {
                Console.Write(i + "\t");
                for (int j = 0; j < board[i].Count; j++)
                {
                    string play = "";
                    switch (board[i][j].State)
                    {
                        case SquareState.Black:
                            play = "X";
                            break;
                        case SquareState.Red:
                            play = "O";
                            break;
                        default:
                            play = "-";
                            break;
                    }
                    Console.Write(play + "\t");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < board[0].Count; i++)
                Console.Write("\t" + i);

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
