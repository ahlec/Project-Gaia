using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Simulation.GUI;
using Simulation.Buildings;
using Simulation.ResearchLabs;

namespace Simulation
{
    public class Simulation : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Screens.IScreen _currentScreen;
        PlayerGame _playerGame;
        ApplicationSkin skin;

        public Simulation()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GlobalSettings.ScreenWidth;
            graphics.PreferredBackBufferHeight = GlobalSettings.ScreenHeight;
            //if (!graphics.IsFullScreen)
            //    graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            _currentScreen = new Screens.SimulationScreen();
            IsMouseVisible = true;
        }

        public GraphicsDeviceManager GraphicsDeviceManager { get { return graphics; } }

        protected override void Initialize()
        {
            base.Initialize();
            skin = ApplicationSkin.FromXML(Content.RootDirectory + @"\defaultSkin.xml", this);

            GlobalSettings.BuildingCatalog = BuildingCatalog.Load(Content.RootDirectory +
                @"\buildings.xml", this);
            GlobalSettings.ResearchCatalog = ResearchCatalog.Load(Content.RootDirectory +
                @"\research_labs.xml", this);

            _playerGame = new PlayerGame(this, skin);
            foreach (Building building in GlobalSettings.BuildingCatalog.Values)
                if (building.InitiallyAvailable)
                    _playerGame.AvailableBuildings.Add(building);
            foreach (ResearchBase researchBase in GlobalSettings.ResearchCatalog.Values)
                if (researchBase.InitiallyAvailable)
                    _playerGame.AvailableResearches.Add(new Research(researchBase));

            _playerGame.UrbanMap.Initialize();
            _playerGame.CountryMap.Initialize();
            _playerGame.OceanMap.Initialize();

            _currentScreen.Initialize(this, skin, _playerGame);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing2D.font = Content.Load<SpriteFont>("Arial");
            Tiles.TileExtensions.isometricTile = Content.Load<Texture2D>("isometric_tile");
        }

        protected override void Update(GameTime gameTime)
        {
            _playerGame.Update(gameTime);
            _currentScreen.Update(gameTime, ref _playerGame);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            _currentScreen.Draw(gameTime, graphics.GraphicsDevice, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
