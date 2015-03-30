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
using System.Windows.Shapes;

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for GameSettings.xaml
    /// </summary>
    public partial class GameSettings : Window
    {
        public Turn StartingTurn { get; set; }
        public int Coloumns { get; set; }
        public int Rows { get; set; }
        public int WinCount { get; set; }


        public GameSettings()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            StartingTurn = ColorComboBox.SelectedIndex == 0 ? Turn.Red : Turn.Black;
            Coloumns = int.Parse(ColsTextBox.Text);
            Rows = int.Parse(RowsTextBox.Text);
            WinCount = int.Parse(WinCountTextBox.Text);

            this.Close();

        }
    }
}
