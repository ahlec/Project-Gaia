using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.GUI
{
    public class ScreenItemLabel : ScreenItem
    {
        public ScreenItemLabel(Game game, ApplicationSkin skin, int x, int y, int fontSize, string text)
            : base(game, skin, x, y, skin.Fonts[fontSize].MeasureString(text).X, skin.Fonts[fontSize].MeasureString(text).Y)
        {
            this.fontSize = fontSize;
            Text = text;
            TextAlignment = ScreenItemTextAlignment.Left;
            FontColor = Color.Black;
        }
        public ScreenItemLabel(Game game, ApplicationSkin skin, int x, int y, int fontSize, int maxWidth, string text)
            : base (game, skin, x, y, maxWidth, skin.Fonts[fontSize].MeasureStringMultiline(skin.Fonts[fontSize].WordWrap(text, maxWidth)).Y)
        {
            this.fontSize = fontSize;
            HasMaxWidth = true;
            MaxWidth = maxWidth;
            Text = text;
            TextAlignment = ScreenItemTextAlignment.Left;
            FontColor = Color.Black;
        }
        public ScreenItemLabel(Game game, ApplicationSkin applicationSkin, int x, int y, int fontSize, int width,
            int height, string text) :
            base(game, applicationSkin, x, y, width, height)
        {
            this.fontSize = fontSize;
            Text = text;
            TextAlignment = ScreenItemTextAlignment.Left;
            FontColor = Color.Black;
        }
        public ScreenItemLabel(Game game, ApplicationSkin skin, int x, int y, int fontSize, int width, int height, string text,
            ScreenItemTextAlignment alignment) : base(game, skin, x, y, width, height)
        {
            this.fontSize = fontSize;
            Text = text;
            TextAlignment = alignment;
            FontColor = Color.Black;
        }
        private int fontSize;
        public int FontSize { get { return fontSize; } }
        public bool HasMaxHeight { get; set; }
        public bool HasMaxWidth { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public string Text { get; set; }
        public ScreenItemTextAlignment TextAlignment { get; set; }
        public int PaddingLeft { get; set; }
        public Color FontColor { get; set; }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            string[] multilines = skin.Fonts[fontSize].WordWrap(Text, (HasMaxWidth ? MaxWidth : (int)Width));
            float lineY = 0;
            foreach (string line in multilines)
            {
                Vector2 textSize = skin.Fonts[fontSize].MeasureString(line);
                Vector2 textPosition = new Vector2(0, Position.Y + lineY);
                switch (TextAlignment)
                {
                    case ScreenItemTextAlignment.Center: textPosition.X = (int)(X + (Width - PaddingLeft - textSize.X) / 2); break;
                    case ScreenItemTextAlignment.Left: textPosition.X = Position.X; break;
                    case ScreenItemTextAlignment.Right: textPosition.X = Position.X + Size.X -
                        textSize.X; break;
                }
                spriteBatch.DrawString(skin.Fonts[fontSize], line, textPosition + PaddingLeft * Vector2.UnitX,
                    FontColor);
                lineY += textSize.Y;
            }
        }
        public override string ToString()
        {
            return base.ToString() + " {'" + Text + "'}";
        }
    }
}
