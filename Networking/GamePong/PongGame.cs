using CommonLib.GamePong;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace CommonLib.GamePong {

    /// <summary>
    /// Represents the game scene with all players/elements as 
    /// field members contained inside this class
    /// </summary>
    public class PongGame {

        private Ball _ball;
        private Player _playerLeft;
        private Player _playerRight;

        private Timer timerBall;
        protected bool started;
        private readonly World world;

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

        public IEnumerable<GameObject> WorldObjects { 
            get {
                return new List<GameObject>() {
                    Ball,
                    PlayerLeft,
                    PlayerRight 
                };
            }
        }

        public World World => world;

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

        internal PongGame(World world) {
            this.world = world;
            timerBall = new Timer(OnBallTimerTick);
            Ball = new Ball(world);
        }

        public PongGame(World world, Player playerLeft, Player playerRight) : this(world) {
            PlayerLeft = playerLeft;
            PlayerRight = playerRight;
        }

        public virtual void StartGame() {
            if (started) return;
            started = true; // set before everything else for cross thread safety
            PlayerLeft.Start();
            PlayerRight.Start();
            Ball.Paused = false;
            timerBall.Change(World.BallIntervalMs, World.BallIntervalMs);
        }

        public virtual void StopGame() {
            if (!started) return;
            PlayerLeft.Stop();
            PlayerRight.Stop();
            Ball.Paused = true;
            timerBall.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public virtual void SuspendGame() {
            PlayerLeft.Suspend();
            PlayerRight.Suspend();
            Ball.Paused = true;
            timerBall.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public virtual void RestartGame() {
            PlayerLeft.Resume();
            PlayerRight.Resume();
            timerBall.Change(World.BallIntervalMs, World.BallIntervalMs);
            Ball.Paused = false;
            Ball.ResetPosition();
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
