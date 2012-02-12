using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Buildings;

namespace Simulation.GUI
{
    public class SimulationScreen : IScreen
    {
        private PlayerGame _playerGame;
        private SimulationScreenMap _currentMap = SimulationScreenMap.Urban;
        private Dictionary<string, ScreenItem> _screenItems = new Dictionary<string, ScreenItem>();     
        private Rectangle mapWindow, minimapWindow;
        private string outputText;
        private Texture2D interfaceBackground;
        private int availableBuildingListOffset = 0;

        public void Initialize(Game game)
        {
            interfaceBackground = game.Content.Load<Texture2D>(@"GUI\interface");
            mapWindow = new Rectangle(151, 0, game.GraphicsDevice.Viewport.Width - 151,
                game.GraphicsDevice.Viewport.Height - 133);
            minimapWindow = new Rectangle(917, 458, 102, 76);
            ScreenItemToggleButton _areaUrban = new ScreenItemToggleButton(game, 4, 
                518, 41, 41, "areaSelection");
            _areaUrban.Toggled += new EventHandler(areaSelectionToggled);
            _areaUrban.Value = true;
            ScreenItemToggleButton _areaCountry = new ScreenItemToggleButton(game, 51,
                518, 41, 41, "areaSelection");
            _areaCountry.Toggled += new EventHandler(areaSelectionToggled);
            ScreenItemToggleButton _areaOcean = new ScreenItemToggleButton(game, 99,
                518, 41, 41, "areaSelection");
            _areaOcean.Toggled += new EventHandler(areaSelectionToggled);
            _screenItems.Add("areaSelection-Urban", _areaUrban);
            _screenItems.Add("areaSelection-Country", _areaCountry);
            _screenItems.Add("areaSelection-Ocean", _areaOcean);

            ScreenItemWindowButtons closeButton = new ScreenItemWindowButtons(game, GlobalSettings.ScreenWidth - 30, 10);
            closeButton.OnClick += new EventHandler(openCloseDialog);
            _screenItems.Add("windowButtons-Close", closeButton);

            ScreenItemProgressBar _ecoHealthBar = new ScreenItemProgressBar(game, 5, 29, 132, 23);
            _ecoHealthBar.MaxValue = 100;
            _ecoHealthBar.MinValue = 0;
            _ecoHealthBar.CurrentValue = 37;
            _ecoHealthBar.Text = "Eco-health";
            _screenItems.Add("ecoHealth-Bar", _ecoHealthBar);

            ScreenItemProgressBar _ecoHealthBarTwo = new ScreenItemProgressBar(game, 884, 402, 162, 24);
            _ecoHealthBarTwo.MaxValue = 100;
            _ecoHealthBarTwo.MinValue = 0;
            _ecoHealthBarTwo.CurrentValue = 37;
            _ecoHealthBarTwo.Text = "Eco-health";
            _screenItems.Add("ecoHealth2-Bar", _ecoHealthBarTwo);

            ScreenItemList _technologiesList = new ScreenItemList(game, 5, 98, 132, 24, "Technologies");
            _technologiesList.Items.Add(new ScreenItemListItem("Item 1"));
            _technologiesList.Items.Add(new ScreenItemListItem("Item 2"));
            _technologiesList.Items.Add(new ScreenItemListItem("Item 3"));
            ScreenItemList _buildingsList = new ScreenItemList(game, 5, 143, 132, 24, "Buildings");
            _buildingsList.Items.Add(new ScreenItemListItem("Building 1"));
            _buildingsList.Items.Add(new ScreenItemListItem("Building 2"));
            _buildingsList.Items.Add(new ScreenItemListItem("Building 3"));
            _buildingsList.ListItemSelected +=new EventHandler<ListItemSelectedEventHandler>(ListItemSelected);
            _technologiesList.ListItemSelected += new EventHandler<ListItemSelectedEventHandler>(ListItemSelected);
            _screenItems.Add("technologiesList", _technologiesList);
            _screenItems.Add("buildingsList", _buildingsList);

            ScreenItemButton _placeBuildingButton = new ScreenItemButton(game, 5, 200, 60, 60, "Place");
            _placeBuildingButton.OnClick += delegate(object snd, EventArgs args)
            {
                Map loadedMap = null;
                switch (_currentMap)
                {
                    case SimulationScreenMap.Urban: loadedMap = _playerGame.UrbanMap; break;
                    case SimulationScreenMap.Country: loadedMap = _playerGame.CountryMap; break;
                    case SimulationScreenMap.Ocean: loadedMap = _playerGame.OceanMap; break;
                }
                loadedMap.BeginBuildingPlacement(new SandboxBuilding(game));
            };
            _screenItems.Add("testBuilding", _placeBuildingButton);

            ScreenItemProgressBar resourceMoney = new ScreenItemProgressBar(game, 36, 222, 101, 24);
            resourceMoney.Text = "Money";
            _screenItems.Add("resource-Money", resourceMoney);
        }

        void openCloseDialog(object sender, EventArgs e)
        {
            if (GlobalSettings.GameplayHalted)
                return;
            GlobalSettings.GameplayHalted = true;
            ScreenItemDialog closeDialog = new ScreenItemDialog(((ScreenItem)sender).Game,
                (GlobalSettings.ScreenWidth - 300) / 2, (GlobalSettings.ScreenHeight - 150) / 2,
               300, 150, "Exit game?", true);
            ScreenItemButton closeDialogConfirm = new ScreenItemButton(((ScreenItem)sender).Game,
                closeDialog.X + 5, closeDialog.Y + 20, 65, 40, "Exit");
            closeDialogConfirm.OnClick += delegate(object confSender, EventArgs confE)
                {
                    ((ScreenItem)sender).Game.Exit();
                };
            ScreenItemButton closeDialogCancel = new ScreenItemButton(((ScreenItem)sender).Game,
                closeDialog.X + 75, closeDialog.Y + 20, 65, 40, "Resume");
            closeDialogCancel.OnClick += delegate(object cancSender, EventArgs cancE)
                {
                    if (_screenItems.ContainsKey("closeDialog"))
                        _screenItems.Remove("closeDialog");
                    GlobalSettings.GameplayHalted = false;
                };
            closeDialog.Items.Add(closeDialogConfirm);
            closeDialog.Items.Add(closeDialogCancel);
            _screenItems.Add("closeDialog", closeDialog);
        }

        void ListItemSelected(object sender, ListItemSelectedEventHandler e)
        {
            outputText = e.Item.Label;
        }

        void areaSelectionToggled(object sender, EventArgs e)
        {
            ScreenItemToggleButton sndr = (ScreenItemToggleButton)sender;
            if (sndr == _screenItems["areaSelection-Urban"])
            {
                _currentMap = SimulationScreenMap.Urban;
                ((ScreenItemToggleButton)_screenItems["areaSelection-Country"]).Value = false;
                ((ScreenItemToggleButton)_screenItems["areaSelection-Ocean"]).Value = false;
            }
            else if (sndr == _screenItems["areaSelection-Country"])
            {
                _currentMap = SimulationScreenMap.Country;
                ((ScreenItemToggleButton)_screenItems["areaSelection-Urban"]).Value = false;
                ((ScreenItemToggleButton)_screenItems["areaSelection-Ocean"]).Value = false;
            }
            else if (sndr == _screenItems["areaSelection-Ocean"])
            {
                _currentMap = SimulationScreenMap.Ocean;
                ((ScreenItemToggleButton)_screenItems["areaSelection-Urban"]).Value = false;
                ((ScreenItemToggleButton)_screenItems["areaSelection-Country"]).Value = false;
            }
        }

        public void Update(GameTime gameTime, ref PlayerGame playerGame)
        {
            _playerGame = playerGame;
            List<ScreenItem> orderedScreenItems = _screenItems.Values.OrderByDescending<ScreenItem, int>(item => item.Layer).ToList<ScreenItem>();
            bool stillProcessingMouse = true;
            outputText = "";
            foreach (ScreenItem screenItem in orderedScreenItems)
            {
                if (screenItem.Enabled && stillProcessingMouse)// && screenItem.GetMouseOver())
                {
                    stillProcessingMouse = !screenItem.ProcessUpdateMouse();
                    if (!stillProcessingMouse)
                        outputText = screenItem.ToString();
                }
                screenItem.Update(gameTime);
                outputText += screenItem.ToString() + "\n";
            }

            Map loadedMap = null;
            switch (_currentMap)
            {
                case SimulationScreenMap.Urban: loadedMap = _playerGame.UrbanMap; break;
                case SimulationScreenMap.Country: loadedMap = _playerGame.CountryMap; break;
                case SimulationScreenMap.Ocean: loadedMap = _playerGame.OceanMap; break;
            }
            Microsoft.Xna.Framework.Input.KeyboardState _state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            if (_state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) && !_screenItems.ContainsKey("closeDialog"))
                openCloseDialog(_screenItems["windowButtons-Close"], new EventArgs());
            Vector2 centerTile = loadedMap.CenterTile;
            if (_state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                centerTile.X++;
                centerTile.Y--;
            }
            if (_state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                centerTile.X--;
                centerTile.Y++;
            }
            if (_state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                centerTile.X--;
                centerTile.Y--;
            }
            if (_state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                centerTile.X++;
                centerTile.Y++;
            }
            if (centerTile.X < 0)
                centerTile.X = 0;
            if (centerTile.X >= loadedMap.Width)
                centerTile.X = loadedMap.Width - 1;
            if (centerTile.Y < 0)
                centerTile.Y = 0;
            if (centerTile.Y >= loadedMap.Height)
                centerTile.Y = loadedMap.Height - 1;
            loadedMap.CenterTile = centerTile;
            loadedMap.Update(gameTime, playerGame, mapWindow);
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Map currentMap = null;
            switch (_currentMap)
            {
                case SimulationScreenMap.Urban:
                    currentMap = _playerGame.UrbanMap;
                    break;
                case SimulationScreenMap.Country:
                    currentMap = _playerGame.CountryMap;
                    break;
                case SimulationScreenMap.Ocean:
                    currentMap = _playerGame.OceanMap;
                    break;
            }
            graphicsDevice.Clear(currentMap.BackgroundColor);
            currentMap.Draw(gameTime, spriteBatch, mapWindow);

            /* Interface/Backdrops */
            spriteBatch.Draw(interfaceBackground, new Rectangle(0, 0, GlobalSettings.ScreenWidth,
                GlobalSettings.ScreenHeight), Color.White);

            /* Screen Items */
            //spriteBatch.DrawString(outputText, new Vector2(mapWindow.X + 10, 
            //    mapWindow.Y + 10), Color.Black);

            List<ScreenItem> orderedScreenItems = _screenItems.Values.OrderBy<ScreenItem, int>(item => item.Layer).ToList<ScreenItem>();
            foreach (ScreenItem screenItem in orderedScreenItems)
                screenItem.Draw(gameTime, spriteBatch);

            int availableBuildingStripLimit = (availableBuildingListOffset + 5 <=
                _playerGame.AvailableBuildings.Count ? 5 :
                _playerGame.AvailableBuildings.Count - availableBuildingListOffset);
            for (int buildingIndex = availableBuildingListOffset; buildingIndex < availableBuildingStripLimit; buildingIndex++)
            {
                spriteBatch.Draw(_playerGame.AvailableBuildings[buildingIndex].BuildingIcon,
                    new Rectangle(199 + 140 * (availableBuildingListOffset - buildingIndex), 597, 100, 96), Color.White);
            }

            /* Minimap */
            currentMap.DrawMinimap(gameTime, spriteBatch, minimapWindow);

            /* Gameplay halted? Process */
            /*if (GlobalSettings.GameplayHalted)
            {
                spriteBatch.FillRectangle(0, 0, GlobalSettings.ScreenWidth, GlobalSettings.ScreenHeight,
                    new Color(Color.WhiteSmoke, 175));
            }*/
        }
    }

    public enum SimulationScreenMap
    {
        Urban,
        Country,
        Ocean
    }
}
