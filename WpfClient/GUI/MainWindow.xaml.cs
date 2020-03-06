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
using WpfClient.Controls;

namespace WpfChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void WorldView_ScoreUpdate(object sender, ScoreUpdateEventArgs e) {
            TextBox_Score.Text = $"Score: (Player 1) {e.LeftScore} x {e.RightScore} (Player 2)";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Space) {
                e.Handled = true;
                GameView.StartGame();
            }
        }
    }
}
