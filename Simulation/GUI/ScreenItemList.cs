using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simulation.GUI
{
    public class ScreenItemList : ScreenItem
    {
        protected bool _listOpen = false;
        public ScreenItemList(Game game, ApplicationSkin applicationSkin, int x, int y, int width, int height, string label)
            : base(game, applicationSkin, x, y, width, height)
        {
            Label = label;
            Items = new List<ScreenItemListItem>();
            OnClick += new ClickEventHandler(ProcessOnClick);
        }

        void ProcessOnClick(ScreenItem screenItem, MouseEventArgs args)
        {
            if (!Enabled || Items.Count == 0)
                return;
            MouseState state = Mouse.GetState();
            if (base.GetMouseOver())
            {
                ToggleListOpen();
                return;
            }
            int indexOfSelectedItem = (int)((state.Y - Y - Height) / 16f);
            if (indexOfSelectedItem >= Items.Count)
                throw new ArgumentOutOfRangeException();
            if (ListItemSelected != null && Items[indexOfSelectedItem].Enabled)
            {
                ListItemSelected.Invoke(this, Items[indexOfSelectedItem]);
                ToggleListOpen();
            }
        }
        public string Label { get; set; }
        public List<ScreenItemListItem> Items { get; set; }
        public bool Open { get { return _listOpen; } }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            MouseState mouseState = Mouse.GetState();
            spriteBatch.FillRectangle(Position, Size, (Hovered ? Color.WhiteSmoke : Color.LightGray));
            spriteBatch.DrawString(Label, Position + new Vector2(10, 3), Color.Black);
            if (_listOpen)
            {
                for (int index = 0; index < Items.Count; index++)
                {
                    Vector2 topLeftCorner = new Vector2(X + 10, Y + Height - 1 + (index * 16));
                    Color listItemBackColor = Color.LightGray;
                    if (!Items[index].Enabled)
                        listItemBackColor = Color.Gray;
                    else if (mouseState.X >= topLeftCorner.X && mouseState.X <= X + Width &&
                        mouseState.Y >= topLeftCorner.Y && mouseState.Y < topLeftCorner.Y + 16)
                        listItemBackColor = Color.WhiteSmoke;
                    spriteBatch.FillRectangle(topLeftCorner, new Vector2(Width - 10, 16),
                        listItemBackColor);
                    spriteBatch.DrawString(Items[index].Label, topLeftCorner + new Vector2(10, 1), Color.Black);
                    if (index < Items.Count - 1)
                        spriteBatch.DrawLine(topLeftCorner + new Vector2(0, 15), topLeftCorner +
                            new Vector2(Width - 10, 15), Color.Black);
                }
                spriteBatch.DrawRectangle(X + 10, Y + Height - 1, Width - 10, 16 * Items.Count, Color.Black);
            }
            if (Items.Count > 0)
            {
                spriteBatch.DrawLine(X + Width - 20, Y + (Height - 10) / 2, X + Width - 10, Y + (Height - 10) / 2,
                  Color.Black);
                spriteBatch.DrawLine(X + Width - 20, Y + (Height - 10) / 2, X + Width - 15,
                    Y + Height - (Height - 10) / 2, Color.Black);
                spriteBatch.DrawLine(X + Width - 10, Y + (Height - 10) / 2, X + Width - 15,
                    Y + Height - (Height - 10) / 2, Color.Black);
            }
            spriteBatch.DrawRectangle(Position, Size, Color.Black);
        }
        protected bool draggingLeftMouseButton, draggingRightMouseButton;
        public override bool ProcessUpdateMouse()
        {
            bool processUpdateMouseValue = base.ProcessUpdateMouse();
            MouseState state = Mouse.GetState();
            if (GetMouseOver())
            {
                draggingLeftMouseButton = (state.LeftButton == ButtonState.Pressed);
                draggingRightMouseButton = (state.RightButton == ButtonState.Pressed);
            } else if (Open &&
                ((state.LeftButton == ButtonState.Pressed && !draggingLeftMouseButton) ||
                (state.RightButton == ButtonState.Pressed && !draggingRightMouseButton) ||
                (state.RightButton == ButtonState.Released && draggingRightMouseButton) ||
                (state.LeftButton == ButtonState.Released && draggingLeftMouseButton)))
            {
                ToggleListOpen();
                draggingLeftMouseButton = false;
                draggingRightMouseButton = false;
            }
            return processUpdateMouseValue;
        }
        public override Vector2 GetComponentSize()
        {
            return _size + (_listOpen ? (Items.Count * 16) * Vector2.UnitY : Vector2.Zero);
        }
        public override bool GetMouseOver()
        {
            MouseState state = Mouse.GetState();
            if (base.GetMouseOver())
                return true;
            else if (_listOpen && (state.X >= X + 10 && state.X <= Width && state.Y >= Y + Height - 1 &&
                state.Y <= Y + Height - 1 + 16 * Items.Count))
                return true;
            return false;
        }
        public void ToggleListOpen()
        {
            _listOpen = !_listOpen;
            Layer = (_listOpen ? 2 : 1);
            if (ListOpenToggled != null)
                ListOpenToggled.Invoke(this, new EventArgs());
        }
        public event EventHandler ListOpenToggled;
        public delegate void ListItemSelectedHandler(ScreenItemList list, ScreenItemListItem item);
        public event ListItemSelectedHandler ListItemSelected;
        public override string ToString()
        {
            return base.ToString() + " {'" + Label + "'}";
        }
    }
    public class ScreenItemListItem
    {
        public ScreenItemListItem(string label)
        {
            Label = label;
            Enabled = true;
        }
        public ScreenItemListItem(string label, object attachedInformation)
        {
            Label = label;
            Enabled = true;
            AttachedInformation = attachedInformation;
        }
        public object AttachedInformation { get; set; }
        public string Label { get; set; }
        public bool Enabled { get; set; }
    }
    public class ListItemSelectedEventHandler : EventArgs
    {
        public ListItemSelectedEventHandler(ScreenItemListItem item)
        {
            Item = item;
        }
        public ScreenItemListItem Item { get; set; }
    }
}
