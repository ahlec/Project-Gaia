using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simulation.GUI
{
    public class ScreenItem : DrawableGameComponent
    {
        public ScreenItem(Game game, ApplicationSkin applicationSkin, int x, int y, int width, int height)
            : this(game, applicationSkin, (float)x, (float)y, (float)width, (float)height) { }
        public ScreenItem(Game game, ApplicationSkin applicationSkin, float x, float y, float width, float height)
            : base(game)
        {
            _position = new Vector2(x, y);
            _size = new Vector2(width, height);
            skin = applicationSkin;
        }
        protected ApplicationSkin skin;
        public ApplicationSkin Skin { get { return skin; } }
        protected Vector2 _position, _size;
        protected int _layer = 1;
        protected bool _hovered = false, _mouseDown = false;
        public virtual Vector2 Position { get { return _position; } set { _position = value; } }
        public virtual Vector2 Size { get { return _size; } set { _size = value; } }
        public float X { get { return _position.X; } set { _position.X = value; } }
        public float Y { get { return _position.Y; } set { _position.Y = value; } }
        public virtual float Width { get { return _size.X; } set { _size.X = value; } }
        public virtual float Height { get { return _size.Y; } set { _size.Y = value; } }
        public int Layer { get { return _layer; } set { _layer = value; } }
        public bool Hovered { get { return _hovered; } }
        public object AttachedInformation { get; set; }
        public object Parent { get; set; }
        public float Rotation { get; set; }
        private float opacity = 100f;
        public float Opacity
        {
            get { return opacity; }
            set
            {
                if (value > 100)
                    opacity = 100;
                else if (value < 0)
                    opacity = 0;
                else
                    opacity = value;
            }
        }

        protected bool hasTooltip = false;
        protected string tooltip = null;
        public string Tooltip
        {
            get { return tooltip; }
            set
            {
                tooltip = ((value == null || value.Length == 0) ? null : value);
                hasTooltip = (tooltip != null);
            }
        }
        public bool HasTooltip { get { return hasTooltip; } }

        public override void Update(GameTime gameTime)
        {
            if (!GetMouseOver() && _hovered)
            {
                _hovered = false;
                if (OnMouseLeave != null)
                    OnMouseLeave.Invoke(this, new EventArgs());
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// Processes mouse-based interactions with this screen item. This should be called before Update
        /// during each Update cycle, and should be used to (in proper Layer descending order) allow for
        /// the ScreenItem of the highest layer to get mouse interaction, and no lower-layered ScreenItems.
        /// Returns true if this ScreenItem would/has/did use mouse input, or false if the ScreenItem would
        /// be unaffected by the mouse during this iteration.
        /// </summary>
        /// <returns></returns>
        public virtual bool ProcessUpdateMouse()
        {
            if (!Enabled)
                return false;
            MouseState _state = Mouse.GetState();
            if (GetMouseOver())
            {
                if (!_hovered)
                {
                    _hovered = true;
                    if (OnMouseEnter != null)
                        OnMouseEnter.Invoke(this, new EventArgs());
                }
                if (!_mouseDown && _state.LeftButton == ButtonState.Pressed)
                {
                    _mouseDown = true;
                    if (OnMouseDown != null)
                        OnMouseDown.Invoke(this, new EventArgs());
                }
                else if (_mouseDown && _state.LeftButton == ButtonState.Released)
                {
                    _mouseDown = false;
                    if (OnMouseUp != null)
                        OnMouseUp.Invoke(this, new EventArgs());
                    if (OnClick != null)
                        OnClick.Invoke(this, new MouseEventArgs(_state.X, _state.Y));
                }
                return true;
            }
            else
            {
                if (_hovered)
                {
                    _hovered = false;
                    if (OnMouseLeave != null)
                        OnMouseLeave.Invoke(this, new EventArgs());
                }
            }
            return false;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime);
        }
        private const int TooltipPadding = 5;
        public void DrawTooltip(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (!hasTooltip)
                return;
            MouseState mouseState = Mouse.GetState();
            int widthOfTooltip = graphicsDevice.Viewport.Width - mouseState.X - 18; // right padding, width of cursor
            bool tooltipToRightOfMouse = true;
            if (widthOfTooltip < 240)
            {
                tooltipToRightOfMouse = false;
                widthOfTooltip = mouseState.X - 5; // left padding
            }
            string[] tooltipLines = Skin.TooltipFont.WordWrap(Tooltip,
                widthOfTooltip);
            Vector2 tooltipSize = Skin.TooltipFont.MeasureStringMultiline(tooltipLines);
            tooltipSize.X += 2 * TooltipPadding;
            Vector2 tooltipLocation = new Vector2(mouseState.X + (tooltipToRightOfMouse ? 13 : 0),
                (mouseState.Y + tooltipSize.Y + 5 <
                graphicsDevice.Viewport.Height ? mouseState.Y : mouseState.Y - tooltipSize.Y));
            spriteBatch.FillRectangle(tooltipLocation, tooltipSize, Color.Beige);
            spriteBatch.DrawRectangle(tooltipLocation, tooltipSize, Color.Black);
            spriteBatch.DrawStrings(Skin.TooltipFont, tooltipLines, tooltipLocation + new Vector2(TooltipPadding, 0),
                Color.Black);
        }
        public virtual bool GetMouseOver()
        {
            MouseState _state = Mouse.GetState();
            Vector2 mousePosition = new Vector2(_state.X, _state.Y);
            return (mousePosition.X >= _position.X && mousePosition.X <= _position.X + _size.X &&
                mousePosition.Y >= _position.Y && mousePosition.Y <= _position.Y + _size.Y);
        }
        public virtual Vector2 GetComponentSize()
        {
            return _size;
        }
        protected virtual Vector2 GetInternalComponentSize() { return GetComponentSize(); }

        public event ClickEventHandler OnClick;
        public event EventHandler OnMouseDown;
        public event EventHandler OnMouseUp;
        public event EventHandler OnMouseEnter;
        public event EventHandler OnMouseLeave;
    }
    public delegate void ClickEventHandler(ScreenItem screenItem, MouseEventArgs eventArgs);
}
