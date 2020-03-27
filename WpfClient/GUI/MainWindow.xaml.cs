using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using WpfClient.Controls;

namespace WpfChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            // use console for log output
            CommonLib.Logging.Log.SetLogOutput(Console.Out);
        }

        private void WorldView_ScoreUpdate(object sender, ScoreUpdateEventArgs e) {
            TextBox_Score.Text = $"Score: (Player 1) {e.LeftScore} x {e.RightScore} (Player 2)";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.G) {
                e.Handled = true;
                GameView.StartGame(); 
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e) {
            int port;
            if (int.TryParse(TextBox_Server_Port.Text, out port)) {
                GameView.InitializeGameAsServer(port);
            } else {
                MessageBox.Show("Invalid port number");
            }
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e) {
            int port;
            if (!int.TryParse(TextBox_Port.Text, out port)) {
                MessageBox.Show("Invalid port number");
                return;
            }
            IPAddress ip;
            if (!IPAddress.TryParse(TextBox_IP.Text, out ip)) {
                MessageBox.Show("Invalid IP address");
                return;
            }
            GameView.InitializeGameAsClient(ip, port);
            GameView.Focus();
        }
    }
}
