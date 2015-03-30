using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <summary>
    /// Interaction logic for Coloumn.xaml
    /// </summary>
    public partial class Square : UserControl
    {
        public const int WIDTH = 100;
        public const int HEIGHT = 100;

        #region Properties

        [Description("What color is currently played at this square"), Category("Data")]
        public SquareState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;

                switch (_state)
                {
                    case SquareState.Black:
                        imageBox_Piece.Source = _blackPieceImage;
                        break;
                    case SquareState.Blank:
                        imageBox_Piece.Source = _blankPieceImage;
                        break;
                    case SquareState.Red:
                        imageBox_Piece.Source = _redPieceImage;
                        break;
                }
            } 
        }

        #endregion


        #region Private Vars

        private SquareState _state;

        private readonly BitmapImage _blackPieceImage = new BitmapImage(new Uri(@"Resources/BlackPiece.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _redPieceImage = new BitmapImage(new Uri(@"Resources/RedPiece.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _blankPieceImage = new BitmapImage(new Uri(@"Resources/BlankPiece.png", UriKind.RelativeOrAbsolute));

        #endregion

        #region Contructor

        public Square()
        {
            InitializeComponent();

            State = SquareState.Blank;
        }

        #endregion





        internal SquareSimple GetSimpleClone()
        {
            SquareSimple clone = new SquareSimple();
            clone.State = _state;
            return clone;
        }
    }
}
