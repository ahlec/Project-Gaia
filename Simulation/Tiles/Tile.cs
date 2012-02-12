using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.GUI;
using Simulation.Buildings;

namespace Simulation.Tiles
{
    public class Tile// : ITile
    {
        private Vector2 _location, _sizeOfMap;
        private Color _backColor, _borderColor;
        public Tile(int x, int y, Vector2 sizeOfMap)
        {
            _location = new Vector2(x, y);
            _sizeOfMap = sizeOfMap;
            _backColor = Color.LightGray;
            _borderColor = Color.Black;
            IsBuildable = true;
        }
        public int X { get { return (int)_location.X; } }
        public int Y { get { return (int)_location.Y; } }
        public Vector2 Location { get { return _location; } }
        public bool HasHover { get; set; }
        public Terrain TerrainType { get; set; }
        public bool IsBuildable { get; set; }
        public Building Building { get; set; }
        public Vector2 BuildingPieceHere { get; set; }
        public event EventHandler Click;
        public Color Color { get { return _backColor; } }
        public virtual void OnClick()
        {
            if (Click != null)
                Click.Invoke(this, new EventArgs());
        }

        public void Update(GameTime gameTime, PlayerGame playerGame, Vector2 centerTile, BuildingPlacementInfo placementInfo)
        {
            _backColor = Color.LightGray;
            if (placementInfo == null)
            {
                if (centerTile == _location)
                    _backColor = Color.LightBlue;
                if (HasHover)
                    _backColor = Color.LightSalmon;
            } else
            {
                if (placementInfo.TopTilePlacementLocation.X <= X &&
                    placementInfo.TopTilePlacementLocation.X + placementInfo.Height > X &&
                    placementInfo.TopTilePlacementLocation.Y <= Y &&
                    placementInfo.TopTilePlacementLocation.Y + placementInfo.Width > Y)
                    _backColor = (placementInfo.CanPlace ? Color.Green : Color.Red);
                else
                    _backColor = (IsBuildable ? Color.PaleGreen : Color.LightPink);
            }
        }
    }
}
