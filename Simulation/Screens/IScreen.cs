using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.GUI;

namespace Simulation.Screens
{
    public interface IScreen
    {
        void Initialize(Game game, ApplicationSkin applicationSkin, PlayerGame playerGame);
        void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);
        void Update(GameTime gameTime, ref PlayerGame playerGame);
    }
}
