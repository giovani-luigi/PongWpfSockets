using System;
using System.Drawing;
using System.Numerics;

namespace CommonLib.GamePong {

    /// <summary>
    /// This is an abstract (base) class, used to represent a player in the game.
    /// </summary>
    public abstract class Player : WorldObject {

        public int Score = 0;
        private bool paused = false;

        public WorldSide PlayerSide { get; private set; }

        protected Player(World world, WorldSide playerSide) : base(world, world.PadWidth, world.PadHeight) {
            this.PlayerSide = playerSide;
            MoveToInitialPosition();
        }
        
        public abstract void Start();

        public abstract void Stop();

        public virtual void Suspend() {
            paused = true;
        }

        public virtual void Resume() {
            paused = false;
        }

        public override PointF Position {
            get => base.Position;
            set {
                if (paused) return;
                base.Position = value;
            }
        }

        public void MoveToInitialPosition() {
            if (PlayerSide == WorldSide.Left) {
                Position = new PointF(World.PadDistanceFromEdge, (World.WorldHeight - World.PadHeight) / 2);
            } else if (PlayerSide == WorldSide.Right) {
                Position = new PointF(World.WorldWidth - World.PadDistanceFromEdge - World.PadWidth,
                    (World.WorldHeight - World.PadHeight) / 2);
            }
        }

        public override Vector2 GetNormal() {
            if (PlayerSide == WorldSide.Left) {
                return new Vector2(1, 0);
            } else if (PlayerSide == WorldSide.Right) {
                return new Vector2(-1, 0);
            } else {
                return base.GetNormal();
            }
        }

    }
}
