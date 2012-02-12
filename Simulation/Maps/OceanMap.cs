using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Tiles;
using Simulation.GUI;

namespace Simulation.Maps
{
    public class OceanMap : Map
    {
        public OceanMap(Game game, ApplicationSkin skin, int width, int height)
            : base(game, skin, width, height, Terrain.Water)
        {
        }
        public override Color  BackgroundColor { get { return Color.LightBlue; } }
    }
}
