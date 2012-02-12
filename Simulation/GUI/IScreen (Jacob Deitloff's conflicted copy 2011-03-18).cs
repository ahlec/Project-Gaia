using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.GUI
{
    public interface IScreen
    {
        void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);
        void Update(GameTime gameTime, PlayerGame playerGame);
    }
}
