using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectRConsole
{

    public enum SquareState
    {
        Blank,
        Red,
        Black
    }

    public enum Turn
    {
        Red,
        Black
    }

    public enum GameState
    {
        Playing,
        RedWin,
        BlackWin,
        Tie,
        Setup
    }

    public enum MinMax
    {
        Min,
        Max
    }

    public enum Direction
    {
        Horizontal,
        Vertical,
        DiagonalUp,
        DiagonalDown
    }
}
