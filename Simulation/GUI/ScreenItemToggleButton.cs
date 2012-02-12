using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.GUI
{
    public class ScreenItemToggleButton : ScreenItemButton
    {
        public ScreenItemToggleButton(Game game, ApplicationSkin skin, int x, int y, int width, int height,
            ScreenItemToggleButtonGroup group) :
            base(game, skin, x, y, width, height, null)
        {
            group.AddButton(this);
            this.group = group;
            OnClick += new ClickEventHandler(ToggleButtonMouseClick);
        }
        private ScreenItemToggleButtonGroup group;
        public ScreenItemToggleButtonGroup Group { get { return group; } }
        private bool value = false;
        public bool Value { get { return value; } set { this.value = value; } }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color backColor = Color.Gray;
            if (Value)
                backColor = (Hovered ? Color.White : Color.Silver);
            else if (!Value)
                backColor = (Hovered ? Color.LightGray : Color.Gray);

            if (Value)
                spriteBatch.FillRectangle(Position, Size, new Color(Color.White, 100));
            spriteBatch.DrawRectangle(Position, Size, Color.White);
        }

        public void ToggleButtonMouseClick(ScreenItem item, MouseEventArgs args)
        {
            if (!Enabled)
                return;
            if (!value)
            {
                value = true;
                foreach (ScreenItemToggleButton otherButton in group.Buttons)
                    if (!otherButton.Equals(this))
                        otherButton.value = false;
            }
            if (Toggled != null)
              Toggled.Invoke(this, new EventArgs());
        }

        public event EventHandler Toggled;
    }
}
