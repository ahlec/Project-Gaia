using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simulation.GUI
{
    public class ScreenItemWindowButtons : ScreenItem
    {
        public ScreenItemWindowButtons(Game game, ApplicationSkin skin, int x, int y)
            : base(game, skin, x, y, 20, 20)
        {
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(Position, Size, (Hovered ? Color.LightBlue : Color.Blue));
            spriteBatch.DrawRectangle(Position, Size, Color.Black);
        }
    }
}
