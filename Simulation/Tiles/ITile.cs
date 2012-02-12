using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.GUI;

namespace Simulation.Tiles
{
    public interface ITile
    {
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 tipLocation, Vector2 centerTile);
        void Update(GameTime gameTime, PlayerGame playerGame);

        bool HasHover { get; set; }
    }
    public static class TileExtensions
    {
        public static Texture2D isometricTile = null;
        public static void FillIsometricTile(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
        {
            if (isometricTile == null)
                throw new ApplicationException("Must load base isometric tile background before being able to " +
                    "call this method.");
            spriteBatch.Draw(isometricTile, new Rectangle((int)position.X, (int)position.Y,
                (int)size.X, (int)size.Y), color);
        }
        public static void DrawIsometricTile(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
        {
            spriteBatch.DrawIsometricTile(position, size, color, 1);
        }
        public static void DrawIsometricTile(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color, int thickness)
        {
            Vector2[] vertice = new Vector2[4];
            vertice[0] = position + new Vector2((int)(size.X / 2), 0);
            vertice[1] = position + new Vector2(0, (int)(size.Y / 2));
            vertice[2] = position + new Vector2((int)(size.X / 2), size.Y);
            vertice[3] = position + new Vector2(size.X, (int)(size.Y / 2));

            spriteBatch.DrawLine(vertice[0], vertice[3], color, thickness);
            spriteBatch.DrawLine(vertice[3], vertice[2], color, thickness);
            spriteBatch.DrawLine(vertice[2], vertice[1], color, thickness);
            spriteBatch.DrawLine(vertice[1], vertice[0], color, thickness);
        }
    }
}
