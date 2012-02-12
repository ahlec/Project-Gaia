using System;
using Microsoft.Xna.Framework;

namespace Simulation.GUI
{
    public class MouseEventArgs
    {
        public MouseEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        private int x, y;
        public int X { get { return x; } }
        public int Y { get { return y; } }
        public Vector2 Location { get { return new Vector2(x, y); } }
    }
}
