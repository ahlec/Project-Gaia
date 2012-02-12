using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Graphics;

namespace Simulation.GUI
{
    public class ScreenItemButton : ScreenItem
    {
        private SpriteFont font;
        public ScreenItemButton(Game game, ApplicationSkin skin, int x, int y, int width, int height, string text)
            : base(game, skin, x, y, width, height)
        {
            Text = text;
            font = game.Content.Load<SpriteFont>(@"GUI\guiFont");
        }
        public ScreenItemButton(Game game, ApplicationSkin skin, float x, float y, float width, float height, string text) :
            base (game, skin, x, y, width, height)
        {
            Text = text;
            font = game.Content.Load<SpriteFont>(@"GUI\guiFont");
        }
        public Texture2D Image { get; set; }
        public override bool GetMouseOver()
        {
            MouseState state = Mouse.GetState();
            Vector2[] vertice = new Vector2[4];
            vertice[0] = Vector2.Zero;
            vertice[1] = Size * Vector2.UnitX;
            vertice[2] = Size;
            vertice[3] = Size * Vector2.UnitY;
            for (int index = 0; index < 4; index++)
            {
                Vector2 originalVertex = vertice[index];
                vertice[index].X = (float)(originalVertex.X * Math.Cos(Rotation) -
                    originalVertex.Y * Math.Sin(Rotation)) + Position.X;
                vertice[index].Y = (float)(originalVertex.X * Math.Sin(Rotation) +
                    originalVertex.Y * Math.Cos(Rotation)) + Position.Y;
            }
            return vertice.PointInside(new Vector2(state.X, state.Y));
        }
        public string Text { get; set; }
        public float TextRotation { get; set; }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            Color backColor = Color.LightGray;
            if (Hovered)
                backColor = Color.White;
            if (!Enabled)
                backColor = Color.Gray;
            spriteBatch.FillRectangle(Position, Size, backColor, Rotation);
            spriteBatch.DrawRectangle(Position, Size, Color.Black, 1, Rotation);

            if (Image != null)
            {
                Vector2 imageSize = new Vector2((Image.Width > Width ? Width : Image.Width),
                    (Image.Height > Height ? Height : Image.Height));
                spriteBatch.Draw(Image, new Rectangle((int)((Width - imageSize.X) / 2f + X),
                    (int)((Height - imageSize.Y) / 2f + Y), (int)imageSize.X, (int)imageSize.Y),
                    Color.White);
            }

            if (Text != null)
            {
                Vector2 textSize = skin.Fonts[10].MeasureString(Text);
                spriteBatch.DrawString(skin.Fonts[10], Text, new Vector2((int)(X + (Width - textSize.X) / 2),
                    (int)(Y + (Height - textSize.Y) / 2)), Color.Black, TextRotation, Vector2.Zero,
                    1f, SpriteEffects.None, 0);
            }
        }
    }
}
