using CommonLib.GamePong;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace WpfClient.Controls {

    public class ScoreUpdateEventArgs : EventArgs {
        public int LeftScore;
        public int RightScore;
    }

    /// <summary>
    /// Interaction logic for WorldView.xaml
    /// </summary>
    public partial class WorldView : UserControl {

        public delegate void ScoreUpdateEventHandler(object sender, ScoreUpdateEventArgs e);

        public event ScoreUpdateEventHandler ScoreUpdate;

        private Player playerLeft;
        private Player playerRight;
        private PongGame game;

        public WorldView() {
            
            InitializeComponent();

            // do not run any further if in design time
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            // make sure that on shutdown we invoke a method to cleanup all resources
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            
            // do not run any further if in design time
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            Keyboard.Focus(WorldCanvas);
        }

        /// <summary>
        /// Creates the game object
        /// </summary>
        public void InitializeGameAsServer(int port) {
            
            // create a world (set of configuration values) to use by the players
            World world = new World();

            // creates the player to control the left pad
            playerLeft = new NewKeyboardPlayer(world, WorldSide.Left, Key.Up, Key.Down, this);
            // creates the player to control the right pad
            playerRight = new RemotePlayer(world, WorldSide.Right);

            // creates the game object
            var newGame = new PongGameServer(world, playerLeft, playerRight);
            // attach event listeners
            newGame.GameRefreshed += Game_GameRefreshed;
            newGame.SideWallHit += Game_SideWallHit;
            newGame.NewConnection += ServerNewConnection;

            newGame.StartServer(port);

            game = newGame;
        }

        private void ServerNewConnection(object sender, CommonLib.Events.NewConnectionEventArgs e) {
            
            // dispatch the call to the UI thread to update the controls
            try {
                Dispatcher.Invoke(new Action(() => {
                    // do your UI changes here

                }));
            } catch (TaskCanceledException) {
                // catch on dispatcher shutdown
                // exit normally
            }
        }

        public void InitializeGameAsClient(IPAddress ip, int port) {

            // create a world (set of configuration values) to use by the players
            World world = new World();
            // creates the player to control the left pad
            playerLeft = new RemotePlayer(world, WorldSide.Left);
            // creates the player to control the right pad
            playerRight = new NewKeyboardPlayer(world, WorldSide.Right, Key.Up, Key.Down ,this);

            // creates the game object
            var newGame = new PongGameClient(world, playerLeft, playerRight);
            // attach event listeners
            newGame.GameRefreshed += Game_GameRefreshed;
            newGame.SideWallHit += Game_SideWallHit;

            newGame.ConnectToServer(ip, port);

            game = newGame;
        }

        public void InitializeSinglePlayer() {
            // create a world (set of configuration values) to use by the players
            World world = new World();
            // creates the player to control the left pad
            playerLeft = new NewKeyboardPlayer(world, WorldSide.Left, Key.W, Key.S, this);
            // creates the player to control the right pad
            playerRight = new NewKeyboardPlayer(world, WorldSide.Right, Key.Up, Key.Down, this);

            // creates the game object
            game = new PongGame(world, playerLeft, playerRight);
            // attach event listeners
            game.GameRefreshed += Game_GameRefreshed;
            game.SideWallHit += Game_SideWallHit;
        }

        public void StartGame() {
            game?.StartGame();
        }

        public void StopGame() {
            game?.StopGame();
        }

        /// <summary>
        /// Called whenever the dispatcher shut-down, which we can assume that is 
        /// a good moment to stop the background threads.
        /// </summary>
        private void Dispatcher_ShutdownStarted(object sender, EventArgs e) {
            game?.StopGame();
        }

        /// <summary>
        /// This callback is invoked whenever the game has any change, 
        /// either by player input, or by periodic background threads
        /// </summary>
        private void Game_GameRefreshed(object sender, EventArgs e) {
            // dispatch the call to the UI thread to update the elements positions
            try {
                Dispatcher.Invoke(new Action(() => {
                    // pad positions are set by the Canvas container (attached property)
                    PadLeft.SetValue(Canvas.TopProperty, (double)game.PlayerLeft.Position.Y);
                    PadRight.SetValue(Canvas.TopProperty, (double)game.PlayerRight.Position.Y);
                    // update ball position also by setting Canvas container attached properties
                    Ball.SetValue(Canvas.TopProperty, (double)game.Ball.Position.Y);
                    Ball.SetValue(Canvas.LeftProperty, (double)game.Ball.Position.X);
                }));
            } catch (TaskCanceledException) { 
                // catch on dispatcher shutdown
                // exit normally
            }
        }

        /// <summary>
        /// This callback is invoked whenever the ball hits the side walls
        /// </summary>
        private void Game_SideWallHit(object sender, SideWallHitEventArgs e) {
            try {
                Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("Boom!");
                    ScoreUpdate?.Invoke(this, 
                        new ScoreUpdateEventArgs() {
                            LeftScore = playerLeft.Score, 
                            RightScore = playerRight.Score }
                    );
                }));           
            } catch (TaskCanceledException) {
                // catch on dispatcher shutdown
                // exit normally
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e) {
            Debug.WriteLine($"Key down {e.Key}");
        }
    }
}
