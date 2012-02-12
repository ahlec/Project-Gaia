using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.GUI
{
    public static class Drawing2D
    {
        private static Texture2D pixel = null;
//        private static SpriteFont font = null;
        public static SpriteFont font;

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 pointA, Vector2 pointB, Color color, int thickness)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
                pixel.SetData(new[] { Color.White });
            }
            pointA = new Vector2((int)pointA.X, (int)pointA.Y);
            pointB = new Vector2((int)pointB.X, (int)pointB.Y);
            spriteBatch.Draw(pixel, pointA, null, color, (float)Math.Atan2((double)(pointB.Y - pointA.Y),
                (double)(pointB.X - pointA.X)), Vector2.Zero, new Vector2(Vector2.Distance(pointA, pointB), thickness),
                SpriteEffects.None,
                0);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 pointA, Vector2 pointB, Color color)
        {
            spriteBatch.DrawLine(pointA, pointB, color, 1);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color)
        {
            spriteBatch.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, 1);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, int thickness)
        {
            spriteBatch.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color, int thickness)
        {
            spriteBatch.DrawRectangle(position, size, color, thickness, 0);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
        {
            spriteBatch.DrawRectangle(position, size, color, 1, 0);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
        {
            spriteBatch.DrawRectangle(new Vector2(x, y), new Vector2(width, height), color, 1, 0);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, int thickness)
        {
            spriteBatch.DrawRectangle(new Vector2(x, y), new Vector2(width, height), color, thickness, 0);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.DrawRectangle(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.Width, rectangle.Height), color, 1, 0);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color, int thickness, float rotation)
        {
            Vector2[] vertice = new Vector2[4];
            vertice[0] = Vector2.Zero;
            vertice[1] = size * Vector2.UnitX;
            vertice[2] = size;
            vertice[3] = size * Vector2.UnitY;
            for (int index = 0; index < 4; index++)
            {
                Vector2 originalPoint = vertice[index];
                vertice[index].X = (int)(originalPoint.X * Math.Cos(rotation) -
                    originalPoint.Y * Math.Sin(rotation) + position.X);
                vertice[index].Y = (int)(originalPoint.X * Math.Sin(rotation) +
                    originalPoint.Y * Math.Cos(rotation) + position.Y);
            }
            spriteBatch.DrawLine(vertice[0], vertice[1], color, thickness);
            spriteBatch.DrawLine(vertice[1], vertice[2], color, thickness);
            spriteBatch.DrawLine(vertice[2], vertice[3], color, thickness);
            spriteBatch.DrawLine(vertice[3], vertice[0], color, thickness);
        }
        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
        {
            spriteBatch.FillRectangle(position, size, color, 0);
        }
        public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
        {
            spriteBatch.FillRectangle(new Vector2(x, y), new Vector2(width, height), color, 0);
        }
        public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.FillRectangle(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.Width, rectangle.Height), color, 0);
        }
        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color, float rotation)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
                pixel.SetData(new[] { Color.White });
            }
            spriteBatch.Draw(pixel, position, null, color, rotation, Vector2.Zero,
                size, SpriteEffects.None, 0);

        }
        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2[] points, Color color)
        {
            for (int index = 0; index < points.Length - 1; index++)
                spriteBatch.DrawLine(points[index], points[index + 1], color);
        }
        public static void DrawString(this SpriteBatch spriteBatch, string text, Vector2 location, Color color)
        {
            if (font == null)
                throw new ApplicationException("At this current point in time, arbitrary loading of unspecified " +
                    "font not designed. Must explicitly state a default font via static public field before calling this " +
                    "method.");
            if (text == null)
                text = "";
            spriteBatch.DrawString(font, text, location, color);
        }
        public static void DrawStrings(this SpriteBatch spriteBatch, SpriteFont spriteFont, string[] strings, Vector2 location, Color color)
        {
            int currentHeight = 0;
            foreach (string str in strings)
            {
                spriteBatch.DrawString(spriteFont, str, location + new Vector2(0, currentHeight), color);
                currentHeight += (int)spriteFont.MeasureString(str).Y;
            }
        }
    }
}
