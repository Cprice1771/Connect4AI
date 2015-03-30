using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConnectFour
{

    public delegate void GameOverEvent(object sender, EventArgs e);

    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {

        #region Events

        public event GameOverEvent GameOver;

        protected virtual void OnGameOver(EventArgs e)
        {
            if (GameOver != null)
                GameOver(this, e);
        }

        #endregion

        private GameState _state;
        private Turn _turn;
        private List<List<Square>> _board;
        private int _winCount;
        private int _rows;
        private int _columns;
        private AI _ai;

        public Board() : this(6, 7, Turn.Red, 4)
        {
        }



        public Board(int rows, int columns, Turn turn, int winCount)
        {
            InitializeComponent();

            _ai = new AI();

            _winCount = winCount;
            _turn = turn;
            _state = GameState.Playing;
            _rows = rows;
            _columns = columns;
            _board =  new List<List<Square>>();

            int x = 0;
            int y = 0;

            for (int i = 0; i < rows; i++)
            {
                _board.Add(new List<Square>());
                for (int j = 0; j < columns; j++)
                {
                    Square sq = new Square();
                    sq.Margin = new Thickness(x, y, 0, 0);
                    _board[i].Add(sq);
                    grid.Children.Add(sq);
                    x += Square.WIDTH;
                }

                x = 0;
                y += Square.HEIGHT;
            }


            this.Height = Square.HEIGHT*rows;
            this.Width = Square.WIDTH*columns;



            if(turn == Turn.Black)
                MakePlay(AI.BestMove(GetSimpleBoard(_board), _turn, 2, _winCount));

            grid.UpdateLayout();
        }

        private List<List<SquareSimple>> GetSimpleBoard(List<List<Square>> _board)
        {
            List<List<SquareSimple>> simpleBoard = new List<List<SquareSimple>>();

            for (int i = 0; i < _board.Count; i++)
            {
                simpleBoard.Add(new List<SquareSimple>());
                for (int j = 0; j < _board[i].Count; j++)
                {
                    SquareSimple s = new SquareSimple();
                    s.State = _board[i][j].State;
                    simpleBoard[i].Add(s);
                }
            }

            return simpleBoard;
        }

        private void grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(_state != GameState.Playing)
                return;

            int col = (int)(e.GetPosition(this).X / Square.WIDTH);
            //If we tried to make an invalid play, do nothing 
            if(!MakePlay(col))
                return;;

            _state = CheckBoard();

            switch (_state)
            {
                case GameState.BlackWin:
                case GameState.RedWin:
                case GameState.Tie:
                    ShowResult();
                    break;
            }

            Stopwatch s = new Stopwatch();
            s.Start();
            MakePlay(AI.BestMove(GetSimpleBoard(_board), _turn, 1, _winCount));
            s.Stop();

            Console.WriteLine("Total AI time: " + ((double)s.ElapsedTicks / Stopwatch.Frequency) * 1000.0 + " ms");

            _state = CheckBoard();

            switch (_state)
            {
                case GameState.BlackWin:
                case GameState.RedWin:
                case GameState.Tie:
                    ShowResult();
                    break;
            }

            
        }

        private bool MakePlay(int col)
        {
            Square square = null;
            for (int i = 0; i < _board.Count; i++)
            {
                if (_board[i][col].State == SquareState.Blank)
                    square = _board[i][col];

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
            square.UpdateLayout();
            return true;

        }

        private void ShowResult()
        {
            switch (_state)
            {
                case GameState.BlackWin:
                    MessageBox.Show("Black wins!");
                    break;
                case GameState.RedWin:
                    MessageBox.Show("Red Wins!");
                    break;
                case GameState.Tie:
                    MessageBox.Show("The game was a Tie!");
                    break;
            }

            ResetBoard();

            OnGameOver(new EventArgs());

            

        }

        private void ResetBoard()
        {
            _state = GameState.Playing;

            _board = new List<List<Square>>();

            int x = 0;
            int y = 0;

            for (int i = 0; i < _rows; i++)
            {
                _board.Add(new List<Square>());
                for (int j = 0; j < _columns; j++)
                {
                    Square sq = new Square();
                    sq.Margin = new Thickness(x, y, 0, 0);
                    _board[i].Add(sq);
                    grid.Children.Add(sq);
                    x += Square.WIDTH;
                }

                x = 0;
                y += Square.HEIGHT;
            }

            grid.UpdateLayout();
        }

        private GameState CheckBoard()
        {
            for (int i = 0; i < _board.Count; i ++)
            {
                for (int j = 0; j < _board[i].Count; j++)
                {
                    GameState state = CheckSpot(i, j);
                    if (state != GameState.Playing)
                        return state;
                }
            }

            foreach(List<Square> row in _board)
                foreach(Square square in row)
                    if(square.State == SquareState.Blank)
                        return GameState.Playing;

            return GameState.Tie;
        }

        private GameState CheckSpot(int row, int col)
        {
            bool winner = false;
            SquareState startState = _board[row][col].State;

            if(startState == SquareState.Blank)
                return GameState.Playing;

            //check Vertically
            if (row <= _board.Count - _winCount)
            {
                winner = true;
                for (int i = 1; i < _winCount; i++)
                    if (_board[row + i][col].State != startState)
                        winner = false;
            }

            //check horizontally
            if (col <= _board[0].Count - _winCount && winner == false)
            {
                winner = true;
                for (int i = 1; i < _winCount; i++)
                    if (_board[row][col + i].State != startState)
                        winner = false;
            }

            //Check Diagonally going down
            if (row <= _board.Count - _winCount && col <= _board[0].Count - _winCount && winner == false)
            {
                winner = true;
                for (int i = 1; i < _winCount; i++)
                {
                    if (_board[row + i][col + i].State != startState)
                        winner = false;
                }
            }

            //Check Diagonally going up
            if (row > _winCount && col <= _board[0].Count - _winCount && winner == false)
            {
                winner = true;
                for (int i = 1; i < _winCount; i++)
                {
                    if (_board[row - i][col + i].State != startState)
                        winner = false;
                }
            }

            if(winner)
            {
                if (startState == SquareState.Red)
                    return GameState.RedWin;
                else
                    return GameState.BlackWin;
            }

            return GameState.Playing;
        }

    }
}
