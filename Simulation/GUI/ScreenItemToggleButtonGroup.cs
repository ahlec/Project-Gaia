using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.GUI
{
    public class ScreenItemToggleButtonGroup : ScreenItem
    {
        public ScreenItemToggleButtonGroup(Game game)
            : base(game, null, 0, 0, 0, 0)
        {
            Buttons = new List<ScreenItemToggleButton>();
        }
        public List<ScreenItemToggleButton> Buttons { get; set; }
        public void AddButton(ScreenItemToggleButton button)
        {
            button.Toggled += new EventHandler(buttonToggled);
            Buttons.Add(button);
        }

        void buttonToggled(object sender, EventArgs e)
        {
            ScreenItemToggleButton sndr = (ScreenItemToggleButton)sender;
        }
    }
}
