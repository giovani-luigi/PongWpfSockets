using System;
using System.Drawing;
using System.Numerics;

namespace CommonLib.GamePong {

    /// <summary>
    /// This is an abstract (base) class, used to represent a player in the game.
    /// </summary>
    [Serializable]
    public abstract class Player : WorldObject {

        public int Score = 0;
        private bool paused = false;
        private readonly WorldSide playerSide;

        protected Player(World world, WorldSide playerSide) : base(world, world.PadWidth, world.PadHeight) {
            this.playerSide = playerSide;
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
            if (playerSide == WorldSide.Left) {
                Position = new PointF(World.PadDistanceFromEdge, (World.WorldHeight - World.PadHeight) / 2);
            } else if (playerSide == WorldSide.Right) {
                Position = new PointF(World.WorldWidth - World.PadDistanceFromEdge - World.PadWidth,
                    (World.WorldHeight - World.PadHeight) / 2);
            }
        }

        public override Vector2 GetNormal() {
            if (playerSide == WorldSide.Left) {
                return new Vector2(1, 0);
            } else if (playerSide == WorldSide.Right) {
                return new Vector2(-1, 0);
            } else {
                return base.GetNormal();
            }
        }

    }
}
