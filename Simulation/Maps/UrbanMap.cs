using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Tiles;
using Simulation.GUI;

namespace Simulation.Maps
{
    public class UrbanMap : Map
    {
        public UrbanMap(Game game, ApplicationSkin skin, int width, int height)
            : base(game, skin, width, height, Terrain.Grass)
        {
        }
        public override Color BackgroundColor { get { return Color.LightGray; } }
    }
}
