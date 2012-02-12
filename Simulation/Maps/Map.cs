using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Buildings;
using Simulation.GUI;
using Simulation.Tiles;
using Simulation.Graphics;

namespace Simulation.Maps
{
    public class Map : DrawableGameComponent
    {
        private Dictionary<Vector2, Tile> _tiles;
        private List<IBuilding> _buildings = new List<IBuilding>();
        private int _width, _height;
        private Vector2 _centerTile;
        private Color _backColor;
        private List<Vector2> tileViewportRange = new List<Vector2>();
        private BuildingPlacementInfo _buildingPlacement = null;
        private Building currentlySelectedBuilding = null;
        private Texture2D minimap;
        private ApplicationSkin skin;
        private Vector2 minimapTileSize;

        public Building CurrentlySelectedBuilding { get { return currentlySelectedBuilding; } }
        public BuildingPlacementInfo BuildingPlacementInfo { get { return _buildingPlacement; } }

        public Map(Game game, ApplicationSkin skin, int width, int height, Terrain baseTerrain) : base(game)
        {
            this.skin = skin;
            _width = width;
            _height = height;
            _tiles = new Dictionary<Vector2, Tile>();
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    Tile tile = new Tile(x ,y, new Vector2(_width, _height));
                    tile.TerrainType = baseTerrain;
                    _tiles.Add(tile.Location, tile);
                }
            _backColor = Color.AliceBlue;
        }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public virtual Color BackgroundColor { get { return _backColor; } }
        public Vector2 CenterTile
        {
            get { return _centerTile; }
            set
            {
                if (value.X < 0 || value.X >= _width || value.Y < 0 || value.Y >= _height)
                    throw new ArgumentOutOfRangeException();
                _centerTile = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            minimap = new Texture2D(Game.GraphicsDevice, GlobalSettings.MinimapWidth, GlobalSettings.MinimapHeight,
                0, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Color);
            Color[] background = new Color[minimap.Width * minimap.Height];
            for (int index = 0; index < background.Length; index++)
                background[index] = Color.Purple;
            minimap.SetData<Color>(background);
            foreach (Tile tile in _tiles.Values)
            {
                Color tileColor = skin.Graphics[tile.TerrainType.ToString()].MonoColor;
                if (tile.Building != null &&
                    tile.Building.Textures[tile.BuildingPieceHere].MonoColor != Color.TransparentWhite)
                    tileColor = tile.Building.Textures[tile.BuildingPieceHere].MonoColor;
                SetMinimap(tile.Location, tileColor);
            }
        }
        protected void SetMinimap(Vector2 tile, Color newColor)
        {
            minimapTileSize = new Vector2((float)GlobalSettings.MinimapWidth / (float)_width,
                (float)GlobalSettings.MinimapHeight / (float)_height);
            Rectangle location = new Rectangle();
            location.X = (int)(minimapTileSize.X * tile.Y);
            location.Y = (int)(minimapTileSize.Y * tile.X);
            location.Width = (int)(minimapTileSize.X * (tile.Y + 1)) - (int)(minimapTileSize.X * tile.Y);
            location.Height = (int)(minimapTileSize.Y * (tile.X + 1)) - (int)(minimapTileSize.Y * tile.X);
            if (location.X + location.Width > minimap.Width)
                location.Width = minimap.Width - location.X;
            if (location.Y + location.Height > minimap.Height)
                location.Height = minimap.Height - location.Y;
            Color[] color = new Color[location.Width * location.Height];
            for (int y = 0; y < location.Height; y++)
                for (int x = 0; x < location.Width; x++)
                {
                    color[y * location.Width + x] = (x == 0 || y == 0 ? Color.LightGray : newColor);
                    if (tile.X == _width - 1 && x == location.Width - 1)
                        color[y * location.Width + x] = Color.LightGray;
                    if (tile.Y == _height - 1 && y == location.Height - 1)
                        color[y * location.Width + x] = Color.LightGray;
                }
            minimap.SetData<Color>(0, location, color, 0, color.Length, SetDataOptions.Discard);
        }
        public Texture2D Minimap { get { return minimap; } }

        public void ProcessMinimapMouse(Vector2 minimapTopLeft)
        {
            float rotationDegree = -MathHelper.PiOver4;
            MouseState mouseState = Mouse.GetState();
            Vector2 horizontalMinimapTopLeft = new Vector2((float)(minimapTopLeft.X *
                Math.Cos(rotationDegree) - minimapTopLeft.Y * Math.Sin(rotationDegree)),
                (float)(minimapTopLeft.X * Math.Sin(rotationDegree) + minimapTopLeft.Y *
                Math.Cos(rotationDegree)));
            Vector2 horizontalMouseLocation = new Vector2((float)(mouseState.X *
                Math.Cos(rotationDegree) - mouseState.Y * Math.Sin(rotationDegree)),
                (float)(mouseState.X * Math.Sin(rotationDegree) + mouseState.Y *
                Math.Cos(rotationDegree)));
            Vector2 mousePosition = new Vector2(horizontalMouseLocation.X - horizontalMinimapTopLeft.X,
                horizontalMouseLocation.Y - horizontalMinimapTopLeft.Y);
            mousePosition.X -= (float)(GlobalSettings.MinimapWidth * Math.Cos(MathHelper.PiOver4));
            Vector2 tileUnderMouse = new Vector2((int)Math.Floor(mousePosition.Y / minimapTileSize.Y),
                (int)Math.Floor(mousePosition.X / minimapTileSize.X));

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                CenterTile = tileUnderMouse;
                if (_tiles[tileUnderMouse].Building != null && BuildingSelected != null)
                    BuildingSelected.Invoke(_tiles[tileUnderMouse].Building, this);
            }

            output = tileUnderMouse.ToString();
        }

        public string output = null;

        private List<Vector2> GetVisibleTilesInOrder(Vector2 centerTilePixelLoc, Rectangle drawingViewport)
        {
            int currentX, currentY;
            int curStartX, curStartY;
            curStartY = curStartX = 0;
            List<Vector2> _order = new List<Vector2>();
            while (curStartY < Height)
            {
                currentX = 0;
                currentY = curStartY;
                while (currentY >= 0)
                {
                    _order.Add(new Vector2(currentX, currentY));
                    currentY--;
                    currentX++;
                }
                curStartY++;
            }
            curStartY = Height - 1;
            curStartX = 1;
            while (curStartX < Width)
            {
                currentX = curStartX;
                currentY = curStartY;
                while (currentX < Width)
                {
                    _order.Add(new Vector2(currentX, currentY));
                    currentY--;
                    currentX++;
                }
                curStartX++;
            }
            return _order;
        }

        class TileMouseInfo
        {
            public TileMouseInfo(Vector2 initialTile, MouseButtons button)
            {
                InitialTile = initialTile;
                Button = button;
            }
            public Vector2 InitialTile { get; set; }
            public MouseButtons Button { get; set; }
            public override string ToString()
            {
                return "{" + InitialTile.ToString() + ", " + Button.ToString() + "}";
            }
        }

        private TileMouseInfo _mouseInfo = null;

        public event BuildingPlacementBeganHandler BuildingPlacementBegan;
        public bool PlacingBuilding { get { return _buildingPlacement != null; } }

        public void Update(GameTime gameTime, PlayerGame playerGame, Rectangle drawingViewport, bool processMouse)
        {
            if (processMouse)
            {
                MouseState mouseState = Mouse.GetState();
                if (mouseState.X >= drawingViewport.Left && mouseState.X <= drawingViewport.Right &&
                    mouseState.Y >= drawingViewport.Top && mouseState.Y <= drawingViewport.Bottom)
                {
                    Vector2 centerTilePixelLoc = new Vector2(drawingViewport.X + (int)(drawingViewport.Width / 2),
                        drawingViewport.Y + (int)(drawingViewport.Height / 2));
                    List<Vector2> tiles = GetVisibleTilesInOrder(centerTilePixelLoc, drawingViewport);
                    Vector4 _tileRangeWithinViewportBounds = new Vector4(-1, -1, -1, -1);
                    Vector2 tileBoundaryPixelMin = new Vector2(-1, -1);
                    Vector2 tileBoundaryPixelMax = new Vector2(-1, -1);
                    Vector2 tileBoundaryTileMin = new Vector2(-1, -1);
                    Vector2 tileBoundaryTileMax = new Vector2(-1, -1);
                    foreach (Vector2 tile in tiles)
                    {
                        Vector2 tileTipLocation = centerTilePixelLoc;
                        Vector2 tilesFromCenter = new Vector2(_centerTile.X - tile.X, tile.Y - _centerTile.Y);
                        /* I will be honest: I don't really know why this works. But it does */
                        tileTipLocation = centerTilePixelLoc + new Vector2(tilesFromCenter.X *
                               0.5f * GlobalSettings.TileWidth,
                            ((tilesFromCenter.Y - tilesFromCenter.X * 0.5f) * GlobalSettings.TileHeight));
                        tileTipLocation += new Vector2(GlobalSettings.TileWidth * tilesFromCenter.Y * 0.5f,
                            GlobalSettings.TileHeight * -tilesFromCenter.Y * 0.5f);
                        if (tileTipLocation.X - GlobalSettings.TileWidth * 0.5f < mouseState.X &&
                            tileTipLocation.X + GlobalSettings.TileWidth * 0.5f > mouseState.X &&
                            tileTipLocation.Y < mouseState.Y &&
                            tileTipLocation.Y + GlobalSettings.TileHeight > mouseState.Y)
                        {
                            if (mouseState.X <= tileTipLocation.X &&
                                mouseState.Y > tileTipLocation.Y + (tileTipLocation.X - mouseState.X) * 0.5f &&
                                mouseState.Y < tileTipLocation.Y + GlobalSettings.TileHeight -
                                    (tileTipLocation.X - mouseState.X) * 0.5f)
                            {
                                _tiles[tile].HasHover = true;
                                if (_buildingPlacement != null)
                                    _buildingPlacement.TopTilePlacementLocation = _tiles[tile].Location;
                                if (mouseState.LeftButton == ButtonState.Pressed && _mouseInfo == null)
                                    _mouseInfo = new TileMouseInfo(tile, MouseButtons.LeftButton);
                                if (mouseState.LeftButton == ButtonState.Released && _mouseInfo != null)
                                {
                                    _tiles[tile].OnClick();
                                    if (TileClicked != null)
                                        TileClicked.Invoke(this, new TileClickedEventArgs(_tiles[tile]));
                                    if (_tiles[tile].Building != null)
                                    {
                                        currentlySelectedBuilding = _tiles[tile].Building;
                                        if (BuildingSelected != null)
                                            BuildingSelected.Invoke(_tiles[tile].Building, this);
                                    }
                                    _mouseInfo = null;
                                }
                            }
                            else if (mouseState.X >= tileTipLocation.X &&
                                mouseState.Y > tileTipLocation.Y + (mouseState.X - tileTipLocation.X) * 0.5f &&
                                mouseState.Y < tileTipLocation.Y + GlobalSettings.TileHeight -
                                    (mouseState.X - tileTipLocation.X) * 0.5f)
                            {
                                _tiles[tile].HasHover = true;
                                if (_buildingPlacement != null)
                                    _buildingPlacement.TopTilePlacementLocation = _tiles[tile].Location;
                                if (mouseState.LeftButton == ButtonState.Pressed && _mouseInfo == null)
                                    _mouseInfo = new TileMouseInfo(tile, MouseButtons.LeftButton);
                                if (mouseState.LeftButton == ButtonState.Released && _mouseInfo != null)
                                {
                                    _tiles[tile].OnClick();
                                    if (TileClicked != null)
                                        TileClicked.Invoke(this, new TileClickedEventArgs(_tiles[tile]));
                                    if (_tiles[tile].Building != null)
                                    {
                                        currentlySelectedBuilding = _tiles[tile].Building;
                                        if (BuildingSelected != null)
                                            BuildingSelected.Invoke(_tiles[tile].Building, this);
                                    }
                                    _mouseInfo = null;
                                }
                            }
                            else
                                _tiles[tile].HasHover = false;

                        }
                        else
                            _tiles[tile].HasHover = false;
                        if (tileTipLocation.X >= drawingViewport.X &&
                            (tileBoundaryPixelMin.X == -1 || tileTipLocation.X < tileBoundaryPixelMin.X))
                        {
                            tileBoundaryPixelMin.X = tileTipLocation.X;
                            tileBoundaryTileMin.X = tile.X;
                        }
                        if (tileTipLocation.Y >= drawingViewport.Y &&
                            (tileBoundaryPixelMin.Y == -1 || tileTipLocation.Y < tileBoundaryPixelMin.Y))
                        {
                            tileBoundaryPixelMin.Y = tileTipLocation.Y;
                            tileBoundaryTileMin.Y = tile.Y;
                        }
                        if (tileTipLocation.X <= drawingViewport.Right &&
                            (tileBoundaryPixelMax.X == -1 || tileTipLocation.X > tileBoundaryPixelMax.X))
                        {
                            tileBoundaryPixelMax.X = tileTipLocation.X;
                            tileBoundaryTileMax.X = tile.X;
                        }
                        if (tileTipLocation.Y <= drawingViewport.Bottom &&
                            (tileBoundaryPixelMax.Y == -1 || tileTipLocation.Y > tileBoundaryPixelMax.Y))
                        {
                            tileBoundaryPixelMax.Y = tileTipLocation.Y;
                            tileBoundaryPixelMax.Y = tile.Y;
                        }
                    }
                    /*_tileRangeWithinViewportBounds = new Vector4(tileBoundaryTileMin.X,
                        tileBoundaryTileMin.Y, tileBoundaryTileMax.X, tileBoundaryTileMax.Y);*/
                    tileViewportRange = new List<Vector2>();
                }

                if (_buildingPlacement != null)
                {
                    List<string> reasonsCannotPlace = new List<string>();
                    Vector2 placeStart = _buildingPlacement.TopTilePlacementLocation;
                    if (placeStart.Y + _buildingPlacement.Width > Width ||
                        placeStart.X + _buildingPlacement.Height > Height)
                    {
                        _buildingPlacement.CanPlace = false;
                        reasonsCannotPlace.Add("Building cannot be placed outside the bounds of the map.");
                    }
                    else
                    {
                        _buildingPlacement.CanPlace = true;
                        for (int iterationX = 0; iterationX < _buildingPlacement.Height; iterationX++)
                        {
                            for (int iterationY = 0; iterationY < _buildingPlacement.Width; iterationY++)
                            {
                                if (!_tiles[placeStart + new Vector2(iterationX, iterationY)].IsBuildable)
                                {
                                    _buildingPlacement.CanPlace = false;
                                    if (!reasonsCannotPlace.Contains("Location is deemed as unbuildable."))
                                        reasonsCannotPlace.Add("Location is deemed as unbuildable.");
                                    continue;
                                }
                                Terrain validBuildingTypes = _buildingPlacement.Building.ValidTerrainTypes;
                                Terrain tileTerrain = _tiles[placeStart + new Vector2(iterationX, iterationY)].TerrainType;
                                if ((validBuildingTypes & tileTerrain) != tileTerrain)
                                {
                                    _buildingPlacement.CanPlace = false;
                                    if (!reasonsCannotPlace.Contains("Cannot construct this structure on " +
                                        _tiles[placeStart + new Vector2(iterationX,
                                            iterationY)].TerrainType.ToString().ToLower() + "."))
                                        reasonsCannotPlace.Add("Cannot construct this structure on " +
                                            _tiles[placeStart + new Vector2(iterationX,
                                                iterationY)].TerrainType.ToString().ToLower() + ".");
                                    continue;
                                }
                                if (_tiles[placeStart + new Vector2(iterationX, iterationY)].Building != null)
                                {
                                    _buildingPlacement.CanPlace = false;
                                    if (!reasonsCannotPlace.Contains("Cannot intersect with the " +
                                        _tiles[placeStart + new Vector2(iterationX, iterationY)].Building.Name + "."))
                                        reasonsCannotPlace.Add("Cannot intersect with the " + _tiles[placeStart +
                                            new Vector2(iterationX, iterationY)].Building.Name + ".");
                                    continue;
                                }
                            }
                        }
                    }
                    _buildingPlacement.ReasonsCannotPlace = reasonsCannotPlace.ToArray();
                }
            }

            foreach (Tile tile in _tiles.Values)
                tile.Update(gameTime, playerGame, _centerTile, _buildingPlacement);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawingViewport, ApplicationSkin skin)
        {
            /* Determine drawing bounds */
            Vector2 centerTilePixelLoc = new Vector2(drawingViewport.X + (int)(drawingViewport.Width / 2),
                drawingViewport.Y + (int)(drawingViewport.Height / 2));

            List<Vector2> _order = GetVisibleTilesInOrder(centerTilePixelLoc, drawingViewport);
            Vector2 drawStartPoint = centerTilePixelLoc;
            foreach (Vector2 tile in _order)
            {
                Vector2 tileTipLocation = drawStartPoint;
                Vector2 tilesFromCenter = new Vector2(_centerTile.X - tile.X, tile.Y - _centerTile.Y);
                tileTipLocation = drawStartPoint + new Vector2(tilesFromCenter.X *
                       0.5f * GlobalSettings.TileWidth,
                    ((tilesFromCenter.Y - tilesFromCenter.X * 0.5f) * GlobalSettings.TileHeight));
                tileTipLocation += new Vector2(GlobalSettings.TileWidth * tilesFromCenter.Y * 0.5f,
                    GlobalSettings.TileHeight * -tilesFromCenter.Y * 0.5f);
                spriteBatch.DrawTileShadow(tileTipLocation +
                    new Vector2((int)(GlobalSettings.TileWidth / 32), (int)(GlobalSettings.TileHeight / 4)),
                    skin);
                spriteBatch.DrawTile(_tiles[tile], this, tileTipLocation, skin);
                //_tiles[tile].Draw(gameTime, spriteBatch, tileTipLocation, _centerTile);
            }
        }

        public int GetTotalEco()
        {
            int _eco = 0;
            foreach (ISimItem simItem in _buildings)
                _eco += simItem.EcoFactor;
            return _eco;
        }
        public int GetNumberBuildings() { return _buildings.Count; }
        public bool BeginBuildingPlacement(Building building)
        {
            if (_buildingPlacement != null)
                return false;
            _buildingPlacement = new BuildingPlacementInfo(building, _centerTile);
            if (BuildingPlacementBegan != null)
                BuildingPlacementBegan.Invoke(building, this);
            return true;
        }
        public void EndBuildingPlacement(Vector2 location)
        {
            if (_buildingPlacement == null)
                return;
            _buildingPlacement.Building.PlaceBuilding(location);
            for (int x = 0; x < _buildingPlacement.Building.Size.X; x++)
                for (int y = 0; y < _buildingPlacement.Building.Size.Y; y++)
                {
                    Vector2 tile = location + new Vector2(y, x);
                    _tiles[location + new Vector2(y, x)].Building = _buildingPlacement.Building;
                    _tiles[location + new Vector2(y, x)].BuildingPieceHere = new Vector2(x, y);
                    Color color = skin.Graphics[_tiles[tile].TerrainType.ToString()].MonoColor;
                    if (_buildingPlacement.Building.Textures[new Vector2(x, y)].MonoColor != Color.TransparentWhite)
                        color = _buildingPlacement.Building.Textures[new Vector2(x, y)].MonoColor;
                    SetMinimap(location + new Vector2(y, x), color);
                }
            if (BuildingPlacementFinished != null)
                BuildingPlacementFinished.Invoke(_buildingPlacement.Building, this);
            _buildingPlacement = null;
        }
        public delegate void TileClickedHandler(Map map, TileClickedEventArgs args);
        public event TileClickedHandler TileClicked;
        public event BuildingPlacementFinishedHandler BuildingPlacementFinished;
        public event BuildingSelectedHandler BuildingSelected;
    }
    public class TileClickedEventArgs : EventArgs
    {
        protected Tile tile;
        public TileClickedEventArgs(Tile clickedTile)
        {
            tile = clickedTile;
        }
        public Tile Tile { get { return tile; } }
    }
    public delegate void BuildingPlacementBeganHandler(Building building, Map map);
    public delegate void BuildingPlacementFinishedHandler(Building building, Map map);
    public delegate void BuildingSelectedHandler(Building building, Map map);
}
