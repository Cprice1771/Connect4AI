using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectRConsole
{
    public class SquareSimple
    {
        public SquareState State { get; set; }

        public SquareSimple()
        {
            State = SquareState.Blank;
        }

        public SquareSimple Clone()
        {
            SquareSimple ss = new SquareSimple();
            switch (State)
            {
                case SquareState.Black:
                    ss.State = SquareState.Black;
                    break;
                case SquareState.Red:
                    ss.State = SquareState.Red;
                    break;
                default:
                    ss.State = SquareState.Blank;
                    break;
            }
            return ss;
        }
    }
}
