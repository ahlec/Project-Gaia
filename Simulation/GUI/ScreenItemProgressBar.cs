using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Graphics;

namespace Simulation.GUI
{
    public class ScreenItemProgressBar : ScreenItem
    {
        public ScreenItemProgressBar(Game game, ApplicationSkin skin, int x, int y, int width) 
            : base(game, skin, x, y, width, skin.ScreenItemSkins["ProgressBar"].Get<Int32>("Height"))
        {
        }
        public override float Height { get { return skin.ScreenItemSkins["ProgressBar"].Get<Int32>("Height"); } }
        public string Text { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        private int _currentValue;
        public int CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (value < MinValue || value > MaxValue)
                    throw new ArgumentOutOfRangeException();
                _currentValue = value;
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tinting = new Color(Color.White, (Opacity / 100f));
            spriteBatch.TileTexture(skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("EmptyProgress"),
                new Vector2(Position.X + skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("LeftEnd").Width,
                    Position.Y), new Vector2(Size.X - skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("LeftEnd").Width -
                    skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("RightEnd").Width, Height), tinting);
            spriteBatch.Draw(skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("LeftEnd"),
                Position, tinting);
            spriteBatch.Draw(skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("RightEnd"),
                new Vector2(Position.X + Size.X -
                    skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("RightEnd").Width, Position.Y), tinting);
          
            
            float usableWidth = Width - skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("LeftEnd").Width -
                skin.ScreenItemSkins["ProgressBar"].Get<Vector2>("LeftOffset").X - 
                skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("RightEnd").Width;
            float percent = (MaxValue > 0 ? ((float)CurrentValue / (float)MaxValue) : 1);
            Vector2 progressOrigin = new Vector2(Position.X +
                skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("LeftEnd").Width, Position.Y);
            Vector2 progressLocation = progressOrigin + new Vector2(usableWidth * percent, 0) +
                skin.ScreenItemSkins["ProgressBar"].Get<Vector2>("LeftOffset");
            spriteBatch.TileTexture(skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("Progress"),
                progressOrigin, new Vector2(progressLocation.X - progressOrigin.X , Size.Y), tinting);
            spriteBatch.Draw(skin.ScreenItemSkins["ProgressBar"].Get<Texture2D>("Transition"),
                new Vector2((int)progressLocation.X, (int)progressLocation.Y), tinting);

            Vector2 labelSize = skin.Fonts[10].MeasureString(Math.Round(percent * 100).ToString() + "%");
            spriteBatch.DrawString(skin.Fonts[10], Math.Round(percent * 100).ToString() + "%",
                new Vector2((int)(progressLocation.X - labelSize.X),
                    (int)(Position.Y + (Size.Y - labelSize.Y) / 2)),
                new Color(Color.Black, (Opacity / 100f)));
            base.Draw(gameTime, spriteBatch);
        }
    }
}
