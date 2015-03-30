using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CreateGame();

        }

        public void CreateGame()
        {
            GameSettings gs = new GameSettings();
            gs.ShowDialog();

            Board board = new Board(gs.Rows, gs.Coloumns, gs.StartingTurn, gs.WinCount);
            board.GameOver += OnGameOverHandler;
            MainGrid.Children.Clear();

            MainGrid.Children.Add(board);

            this.Height = board.Height + 50;
            this.Width = board.Width + 50;
        }

        public void OnGameOverHandler(object sender, EventArgs e)
        {
            CreateGame();
        }
    }
}
