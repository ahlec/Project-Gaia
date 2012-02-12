using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.GUI
{
    public class ScreenItemNewNotice : ScreenItemImage
    {
        private Vector2 travelDirection, travelBounds, initialLocation;
        private bool moving;
        public ScreenItemNewNotice(Game game, ApplicationSkin skin, int x, int y, Vector2 travelDirection,
            Vector2 travelBounds) : base(game, skin, x, y, "New")
        {
            this.travelBounds = travelBounds;
            this.travelDirection = travelDirection;
            this.initialLocation = new Vector2(x, y);
            moving = true;
        }
        public bool Moving { get { return moving; } }
        public void Reset()
        {
            Position = initialLocation;
            moving = true;
        }
        public void Stop() { moving = false; }
        public override void Update(GameTime gameTime)
        {
            if (moving)
            {
                Vector2 traveledThusFar = new Vector2(X - initialLocation.X, Y - initialLocation.Y);
                if (traveledThusFar.X + travelDirection.X <= travelBounds.X)
                    X += travelDirection.X;
                if (traveledThusFar.Y + travelDirection.Y <= travelBounds.Y)
                    Y += travelDirection.Y;
                if (X > initialLocation.X + travelBounds.X)
                    X = initialLocation.X + travelBounds.X;
                if (Y > initialLocation.Y + travelBounds.Y)
                    Y = initialLocation.Y + travelBounds.Y;
                if (X == initialLocation.X + travelBounds.X && Y == initialLocation.Y + travelBounds.Y)
                {
                    moving = false;
                    if (MovingFinished != null)
                        MovingFinished.Invoke();
                }
            }
            base.Update(gameTime);
        }
        public event MovingFinishedHandler MovingFinished;
    }
    public delegate void MovingFinishedHandler();
}
