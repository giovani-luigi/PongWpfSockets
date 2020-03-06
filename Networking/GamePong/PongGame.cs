using CommonLib.GamePong;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CommonLib.GamePong {

    public class PongGame {

        /// <summary> 
        /// Used for ball moved event 
        /// </summary>
        public delegate void GameChangedEventHandler(object sender, EventArgs e);
        public delegate void SideWallHitEventHandler(object sender, SideWallHitEventArgs e);

        /// <summary> 
        /// Raised when ball moves 
        /// </summary>
        public event GameChangedEventHandler GameRefreshed;
        public event SideWallHitEventHandler SideWallHit;

        private Timer timerBall;

        private bool started;

        public World World { get; }

        private Ball _ball;
        public Ball Ball {
            get { return _ball; }
            set {
                // dettach events of current ball if any
                if (_ball != null) {
                    _ball.ObjectMoved -= OnBallMoved;
                    _ball.SideWallHit -= OnBallHitSideWalls;
                }
                // attach the events of new ball object
                value.ObjectMoved += OnBallMoved;
                value.SideWallHit += OnBallHitSideWalls;
                // save reference
                _ball = value;
            }
        }

        private Player _playerLeft;
        public Player PlayerLeft {
            get { return _playerLeft; }
            set {
                // dettach events of current player if any
                if (_playerLeft != null) {
                    _playerLeft.ObjectMoved -= OnLeftPlayerMoved;
                }
                // attach the events of new player
                value.ObjectMoved += OnLeftPlayerMoved;
                // save reference
                _playerLeft = value;
            }
        }

        private Player _playerRight;
        public Player PlayerRight {
            get { return _playerRight; }
            set {
                // dettach events of current player if any
                if (_playerRight != null) {
                    _playerRight.ObjectMoved -= OnRightPlayerMoved;
                }
                // attach the events of new player
                value.ObjectMoved += OnRightPlayerMoved;
                // save reference
                _playerRight = value;
            }
        }

        public PongGame(World world) {
            World = world;
            timerBall = new Timer(OnBallTimerTick);
            Ball = new Ball(world);
        }

        public PongGame(World world, Player playerLeft, Player playerRight) : this(world) {
            PlayerLeft = playerLeft;
            PlayerRight = playerRight;
        }

        public void StartGame() {
            if (started) return;
            started = true; // set before everything else for cross thread safety
            PlayerLeft.Start();
            PlayerRight.Start();
            timerBall.Change(World.BallIntervalMs, World.BallIntervalMs);
        }

        public void StopGame() {
            if (!started) return;
            PlayerLeft.Stop();
            PlayerRight.Stop();
            timerBall.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SuspendGame() {
            PlayerLeft.Suspend();
            PlayerRight.Suspend();
            timerBall.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void RestartGame() {
            PlayerLeft.MoveToInitialPosition();
            PlayerRight.MoveToInitialPosition();
            PlayerLeft.Resume();
            PlayerRight.Resume();
            Ball.Reset();
            timerBall.Change(World.BallIntervalMs, World.BallIntervalMs);
        }

        private void OnBallTimerTick(object state) {
            var players = new List<Player> { PlayerRight, PlayerLeft };
            Ball.MoveNextBounceOn(players);
        }

        private void OnLeftPlayerMoved(object sender, ObjectMovedEventArgs args) {
            // evaluate all consequences from player movement
            GameRefreshed?.Invoke(this, new EventArgs()); // notify listeners
        }

        private void OnRightPlayerMoved(object sender, ObjectMovedEventArgs args) {
            // evaluate all consequences from player movement
            GameRefreshed?.Invoke(this, new EventArgs()); // notify listeners
        }

        private void OnBallMoved(object sender, EventArgs e) {
            // evaluate all consequences from the ball movement
            GameRefreshed?.Invoke(this, new EventArgs()); // notify listeners
        }
        
        private void OnBallHitSideWalls(object sender, SideWallHitEventArgs e) {
            // stop the game while we invoke external code
            SuspendGame();
            // update score for the opposite player
            if (e.side == WorldSide.Left) {
                PlayerRight.Score++;
            } else if (e.side == WorldSide.Right) {
                PlayerLeft.Score++;
            }
            // notify ball went out of the game region
            SideWallHit?.Invoke(this, e);
            // reset the game
            RestartGame();
        }

    }
}
