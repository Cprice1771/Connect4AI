using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectRConsole
{
    class Program
    {
        static void Main(string[] args)
        {
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
