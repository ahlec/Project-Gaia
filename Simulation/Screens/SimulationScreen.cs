using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Buildings;
using Simulation.GUI;
using Simulation.ResearchLabs;
using Simulation.Maps;
using Microsoft.Xna.Framework.Input;
using Simulation.Graphics;
using Simulation.General;

namespace Simulation.Screens
{
    public class SimulationScreen : IScreen
    {
        private PlayerGame _playerGame;
        private SimulationScreenMap _currentMap = SimulationScreenMap.Urban;   
        private Rectangle mapWindow, minimapWindow;
        private string outputText = null;
        private int availableBuildingListOffset = 0;
        protected ApplicationSkin skin;
        protected SimulationScreenControls controls;
        private Rectangle[] interfaceBlocks;
        private Vector2[] minimapVertices;

        public void Initialize(Game game, ApplicationSkin applicationSkin, PlayerGame playerGame)
        {
            interfaceBlocks = new Rectangle[3];
            interfaceBlocks[0] = new Rectangle(0, 0, 144, 568);
            interfaceBlocks[1] = new Rectangle(0, 568, 1050, 133);
            interfaceBlocks[2] = new Rectangle(877, 376, 173, 192);

            _playerGame = playerGame;
            _playerGame.UrbanMap.TileClicked += new Map.TileClickedHandler(TileClicked);
            _playerGame.UrbanMap.BuildingPlacementBegan += new BuildingPlacementBeganHandler(BuildingPlacementBegins);
            _playerGame.UrbanMap.BuildingPlacementFinished += new BuildingPlacementFinishedHandler(BuildingPlacementEnds);
            _playerGame.UrbanMap.BuildingSelected += new BuildingSelectedHandler(BuildingSelected);
            _playerGame.CountryMap.TileClicked += new Map.TileClickedHandler(TileClicked);
            _playerGame.CountryMap.BuildingPlacementBegan += new BuildingPlacementBeganHandler(BuildingPlacementBegins);
            _playerGame.CountryMap.BuildingPlacementFinished += new BuildingPlacementFinishedHandler(BuildingPlacementEnds);
            _playerGame.CountryMap.BuildingSelected += new BuildingSelectedHandler(BuildingSelected);
            _playerGame.OceanMap.TileClicked += new Map.TileClickedHandler(TileClicked);
            _playerGame.OceanMap.BuildingPlacementBegan += new BuildingPlacementBeganHandler(BuildingPlacementBegins);
            _playerGame.OceanMap.BuildingPlacementFinished += new BuildingPlacementFinishedHandler(BuildingPlacementEnds);
            _playerGame.OceanMap.BuildingSelected += new BuildingSelectedHandler(BuildingSelected);

            _playerGame.ResearchCompleted += delegate(Research research) { _playerGame.ApplyBenefits(research.Benefits); };
            _playerGame.BuildingsMadeAvailable += delegate() { RefreshBuildingOpportunities(); };
            _playerGame.ResearchMadeAvailable += delegate() { RefreshResearchOpportunities(); };

            skin = applicationSkin;
            controls = new SimulationScreenControls(game, applicationSkin);
            controls.UrbanAreaSelection.Toggled += new EventHandler(areaSelectionToggled);
            controls.CountryAreaSelection.Toggled += new EventHandler(areaSelectionToggled);
            controls.OceanAreaSelection.Toggled += new EventHandler(areaSelectionToggled);

            controls.NewTechnologyNotice.MovingFinished += delegate() { controls.NewTechnologyNotice.Visible = false; };

            controls.BuildingListLeft.OnClick += delegate(ScreenItem item, MouseEventArgs args) { ShiftBuildingList(-1); };
            controls.BuildingListRight.OnClick += delegate(ScreenItem item, MouseEventArgs args) { ShiftBuildingList(1); };

            controls.BuildingListLeft.Enabled = false;
            controls.MoneyLabel.Text = playerGame.Money.ToString();
            controls.FoodLabel.Text = playerGame.Food.ToString();
            controls.OilLabel.Text = playerGame.Oil.ToString();

            controls.MinimapEast.OnClick += delegate(ScreenItem item, MouseEventArgs args) { ShiftViewport(new Vector2(0, 1)); };
            controls.MinimapNorth.OnClick += delegate(ScreenItem item, MouseEventArgs args) { ShiftViewport(new Vector2(-1, 0)); };
            controls.MinimapSouth.OnClick += delegate(ScreenItem item, MouseEventArgs args) { ShiftViewport(new Vector2(1, 0)); };
            controls.MinimapWest.OnClick += delegate(ScreenItem item, MouseEventArgs args) { ShiftViewport(new Vector2(0, -1)); };

            foreach (Research research in playerGame.AvailableResearches)
                controls.TechnologyList.Items.Add(new ScreenItemListItem(research.Name, research));
            controls.TechnologyList.ListItemSelected += new ScreenItemList.ListItemSelectedHandler(TechnologyListItemSelected);
            foreach (Building building in playerGame.AvailableBuildings)
                controls.BuildingDropdown.Items.Add(new ScreenItemListItem(building.Name, building));
            controls.BuildingDropdown.ListItemSelected += delegate(ScreenItemList list, ScreenItemListItem item)
            {
                OpenBuildingDialog((Building)item.AttachedInformation);
            };
            int buttonIdentity = 0;
            foreach (ScreenItemButton button in controls.BuildingListButtons)
            {
                button.AttachedInformation = buttonIdentity;
                buttonIdentity++;
                button.OnClick += delegate(ScreenItem item, MouseEventArgs args)
                {
                    OpenBuildingDialog(_playerGame.AvailableBuildings[availableBuildingListOffset +
                           (int)item.AttachedInformation]);
                };
            }

            int upperBounds = (playerGame.AvailableBuildings.Count > 6 ? 6 : playerGame.AvailableBuildings.Count);
            for (int index = 0; index < upperBounds; index++)
                controls.BuildingListButtons[index].Image = playerGame.AvailableBuildings[index].BuildingIcon;

            if (playerGame.AvailableBuildings.Count <= 6)
            {
                for (int index = playerGame.AvailableBuildings.Count; index < controls.BuildingListButtons.Count; index++)
                    controls.BuildingListButtons[index].Enabled = false;
                controls.BuildingListRight.Enabled = false;
            }

            mapWindow = new Rectangle(151, 0, game.GraphicsDevice.Viewport.Width - 151,
                game.GraphicsDevice.Viewport.Height - 133);
            minimapWindow = new Rectangle(955, 405, GlobalSettings.MinimapWidth, GlobalSettings.MinimapHeight);
            minimapVertices = new Vector2[4];
            minimapVertices[0] = Vector2.Zero;
            minimapVertices[1] = new Vector2(GlobalSettings.MinimapWidth, 0);
            minimapVertices[2] = new Vector2(GlobalSettings.MinimapWidth, GlobalSettings.MinimapHeight);
            minimapVertices[3] = new Vector2(0, GlobalSettings.MinimapHeight);
            for (int vertex = 0; vertex < 4; vertex++)
            {
                Vector2 originalVertice = minimapVertices[vertex];
                minimapVertices[vertex].X = (int)(originalVertice.X * Math.Cos(MathHelper.PiOver4) -
                    originalVertice.Y * Math.Sin(MathHelper.PiOver4)) + minimapWindow.X;
                minimapVertices[vertex].Y = (int)(originalVertice.X * Math.Sin(MathHelper.PiOver4) +
                    originalVertice.Y * Math.Cos(MathHelper.PiOver4)) + minimapWindow.Y;
            }

            controls.SelectedBuilding.OnClick += delegate(ScreenItem item, MouseEventArgs args)
            {
                if (((ScreenItemBuildingInformation)item).Building == null)
                    return;
                switch (_currentMap)
                {
                    case SimulationScreenMap.Urban: _playerGame.UrbanMap.CenterTile = ((ScreenItemBuildingInformation)item).Building.MapLocation; break;
                    case SimulationScreenMap.Country: _playerGame.CountryMap.CenterTile = ((ScreenItemBuildingInformation)item).Building.MapLocation; break;
                    case SimulationScreenMap.Ocean: _playerGame.OceanMap.CenterTile = ((ScreenItemBuildingInformation)item).Building.MapLocation; break;
                }
            };
         }

        public void ShiftViewport(Vector2 direction)
        {
            Map map = null;
            switch (_currentMap)
            {
                case SimulationScreenMap.Urban: map = _playerGame.UrbanMap; break;
                case SimulationScreenMap.Country: map = _playerGame.CountryMap; break;
                case SimulationScreenMap.Ocean: map = _playerGame.OceanMap; break;
            }
            Vector2 targetTile = map.CenterTile + direction;
            if (targetTile.X < 0)
                targetTile.X = 0;
            if (targetTile.Y < 0)
                targetTile.Y = 0;
            if (targetTile.X >= map.Width)
                targetTile.X = map.Width - 1;
            if (targetTile.Y >= map.Height)
                targetTile.Y = map.Height - 1;
            map.CenterTile = targetTile;
        }

        private void BuildingPlacementBegins(Building building, Map map)
        {
            foreach (ScreenItemButton button in controls.BuildingListButtons)
                button.Enabled = false;
            controls.BuildingDropdown.Enabled = false;
            controls.TechnologyList.Enabled = false;
            controls.BuildingListRight.Enabled = false;
            controls.BuildingListLeft.Enabled = false;
        }
        private void BuildingPlacementEnds(Building building, Map map)
        {
            foreach (ScreenItemButton button in controls.BuildingListButtons)
                button.Enabled = true;
            controls.BuildingDropdown.Enabled = true;
            controls.TechnologyList.Enabled = true;
            ShiftBuildingList(0);
            _playerGame.Detract(building.Requirements);
            _playerGame.ApplyBenefits(building.Benefits);
        }
        private void BuildingSelected(Building building, Map map)
        {
            controls.SelectedBuilding.Building = building;
        }
        private void RefreshResearchOpportunities()
        {
            if (controls.TechnologyList.Open)
                controls.TechnologyList.ToggleListOpen();
            controls.TechnologyList.Enabled = false;
            controls.TechnologyList.Items.Clear();
            foreach (Research research in _playerGame.AvailableResearches)
            {
                ScreenItemListItem listItem = new ScreenItemListItem(research.Name, research);
                listItem.Enabled = !research.Completed;
                controls.TechnologyList.Items.Add(listItem);
            }
            controls.TechnologyList.Enabled = true;
            controls.NewTechnologyNotice.Reset();
            controls.NewTechnologyNotice.Visible = true;
        }
        private void RefreshBuildingOpportunities()
        {
            if (controls.BuildingDropdown.Open)
                controls.BuildingDropdown.ToggleListOpen();
            controls.BuildingDropdown.Enabled = false;
            controls.BuildingDropdown.Items.Clear();
            foreach (Building building in _playerGame.AvailableBuildings)
                controls.BuildingDropdown.Items.Add(new ScreenItemListItem(building.Name, building));
            controls.BuildingDropdown.Enabled = true;
            int upperBounds = (_playerGame.AvailableBuildings.Count > 6 ? 6 : _playerGame.AvailableBuildings.Count);
            for (int index = 0; index < upperBounds; index++)
            {
                controls.BuildingListButtons[index].Enabled = true;
                controls.BuildingListButtons[index].Image = _playerGame.AvailableBuildings[index].BuildingIcon;
            }
            ShiftBuildingList(0);
        }

        protected virtual void TileClicked(Map map, TileClickedEventArgs args)
        {
            if (map.PlacingBuilding)
            {
                if (map.BuildingPlacementInfo.CanPlace)
                    map.EndBuildingPlacement(args.Tile.Location);
            } else
                map.CenterTile = args.Tile.Location;
        }
        private void OpenBuildingDialog(Building building)
        {
            ScreenItemDialog buildingDialog = new ScreenItemDialog(controls.Game, skin,
                 building.Name, true);
            buildingDialog.AttachedInformation = building;
            ScreenItemImage buildingIcon = new ScreenItemImage(controls.Game, skin, 5, 5,
                building.BuildingIcon);
            buildingDialog.Items.Add(buildingIcon);
            ScreenItemLabel buildingDescription = new ScreenItemLabel(controls.Game, skin,
                10 + (int)buildingIcon.Width, 5, 10, 240, building.Description);
            buildingDialog.Items.Add(buildingDescription);
            ScreenItemButton placeBuilding = new ScreenItemButton(controls.Game, skin,
                5 + (buildingIcon.Width - 75) / 2, buildingIcon.Height + 5,
                75, 20, "Build");
            placeBuilding.Enabled = building.Requirements.AreMet(_playerGame);
            placeBuilding.OnClick += delegate(ScreenItem item, MouseEventArgs args)
            {
                Map currentMap = null;
                switch (_currentMap)
                {
                    case SimulationScreenMap.Urban: currentMap = _playerGame.UrbanMap; break;
                    case SimulationScreenMap.Country: currentMap = _playerGame.CountryMap; break;
                    case SimulationScreenMap.Ocean: currentMap = _playerGame.OceanMap; break;
                }
                buildingDialog.Close();
                currentMap.BeginBuildingPlacement(building);
            };
            buildingDialog.Items.Add(placeBuilding);

            // Requirements
            float requirementsHeight = (int)buildingDescription.Height + 10;
            ScreenItemLabel requirementsLabel = new ScreenItemLabel(controls.Game, skin,
                20 + (int)buildingIcon.Width, (int)requirementsHeight, 13, "Requirements");
            requirementsHeight += requirementsLabel.Height;
            buildingDialog.Items.Add(requirementsLabel);
            if (building.Requirements.Money > 0)
            {
                ScreenItemLabel requirementsMoneyLabel = new ScreenItemLabel(controls.Game, skin,
                    36 + (int)buildingIcon.Width, (int)requirementsHeight,
                    10, "$" + building.Requirements.Money.ToString());
                requirementsMoneyLabel.FontColor = (building.Requirements.Money <= _playerGame.Money ?
                    Color.Black : Color.Red);
                ScreenItemImage requirementsMoneyIcon = new ScreenItemImage(controls.Game, skin,
                    20 + (int)buildingIcon.Width, (int)requirementsHeight, 16, 16, "MoneyIcon");
                requirementsHeight += requirementsMoneyLabel.Height;
                requirementsMoneyIcon.Tooltip = "Money";
                requirementsMoneyLabel.Tooltip = "Money";
                buildingDialog.Items.Add(requirementsMoneyLabel);
                buildingDialog.Items.Add(requirementsMoneyIcon);
            }
            if (building.Requirements.Food > 0)
            {
                ScreenItemLabel requirementsFoodLabel = new ScreenItemLabel(controls.Game, skin,
                    36 + (int)buildingIcon.Width, (int)requirementsHeight,
                    10, building.Requirements.Food.ToString() + " bushels");
                requirementsFoodLabel.FontColor = (building.Requirements.Food <= _playerGame.Food ?
                    Color.Black : Color.Red);
                ScreenItemImage requirementsFoodIcon = new ScreenItemImage(controls.Game, skin,
                    20 + (int)buildingIcon.Width, (int)requirementsHeight, 16, 16, "FoodIcon");
                requirementsHeight += requirementsFoodLabel.Height;
                requirementsFoodIcon.Tooltip = "Food";
                requirementsFoodLabel.Tooltip = "Food";
                buildingDialog.Items.Add(requirementsFoodLabel);
                buildingDialog.Items.Add(requirementsFoodIcon);
            }
            if (building.Requirements.Oil > 0)
            {
                ScreenItemLabel requirementsOilLabel = new ScreenItemLabel(controls.Game, skin,
                    36 + (int)buildingIcon.Width, (int)requirementsHeight,
                    10, building.Requirements.Oil.ToString() + " barrels");
                requirementsOilLabel.FontColor = (building.Requirements.Oil <= _playerGame.Oil ?
                    Color.Black : Color.Red);
                ScreenItemImage requirementsOilIcon = new ScreenItemImage(controls.Game, skin,
                    20 + (int)buildingIcon.Width, (int)requirementsHeight, 16, 16, "OilIcon");
                requirementsHeight += requirementsOilLabel.Height;
                requirementsOilIcon.Tooltip = "Oil";
                requirementsOilLabel.Tooltip = "Oil";
                buildingDialog.Items.Add(requirementsOilLabel);
                buildingDialog.Items.Add(requirementsOilIcon);
            }
            if (building.Requirements.Electricity > 0)
            {
                ScreenItemLabel requirementsElectricityLabel = new ScreenItemLabel(controls.Game, skin,
                    36 + (int)buildingIcon.Width, (int)requirementsHeight,
                    10, building.Requirements.Electricity.ToString() + " kwh");
                requirementsElectricityLabel.FontColor = (building.Requirements.Electricity <=
                    _playerGame.Electricity - _playerGame.ConsumedElectricity ? Color.Black : Color.Red);
                ScreenItemImage requirementsElectricityIcon = new ScreenItemImage(controls.Game, skin,
                    20 + (int)buildingIcon.Width, (int)requirementsHeight, 16, 16, "ElectricityIcon");
                requirementsHeight += requirementsElectricityLabel.Height;
                requirementsElectricityIcon.Tooltip = "Electricity";
                requirementsElectricityLabel.Tooltip = "Electricity";
                buildingDialog.Items.Add(requirementsElectricityLabel);
                buildingDialog.Items.Add(requirementsElectricityIcon);
            }

            // Benefits
            float benefitsHeight = (int)buildingDescription.Height + 10;
            ScreenItemLabel benefitsLabel = new ScreenItemLabel(controls.Game, skin,
                160 + (int)buildingIcon.Width, (int)benefitsHeight, 13, "Outputs");
            benefitsHeight += benefitsLabel.Height;
            buildingDialog.Items.Add(benefitsLabel);
            if (building.Benefits.Income.Value > 0)
            {
                ScreenItemLabel benefitsIncomeLabel = new ScreenItemLabel(controls.Game, skin,
                    176 + (int)buildingIcon.Width, (int)benefitsHeight,
                    10, "$" + building.Benefits.Income.Value.ToString() + " / minute");
                ScreenItemImage benefitsIncomeIcon = new ScreenItemImage(controls.Game, skin,
                    160 + (int)buildingIcon.Width, (int)benefitsHeight, 16, 16, "MoneyIcon");
                benefitsHeight += benefitsIncomeLabel.Height;
                benefitsIncomeIcon.Tooltip = "Income";
                benefitsIncomeLabel.Tooltip = "Income";
                buildingDialog.Items.Add(benefitsIncomeLabel);
                buildingDialog.Items.Add(benefitsIncomeIcon);
            }
            if (building.Benefits.Food.Value > 0)
            {
                ScreenItemLabel benefitsFoodLabel = new ScreenItemLabel(controls.Game, skin,
                    176 + (int)buildingIcon.Width, (int)benefitsHeight,
                    10, building.Benefits.Food.Value.ToString() + " bushels / minute");
                ScreenItemImage benefitsFoodIcon = new ScreenItemImage(controls.Game, skin,
                    160 + (int)buildingIcon.Width, (int)benefitsHeight, 16, 16, "FoodIcon");
                benefitsHeight += benefitsFoodLabel.Height;
                benefitsFoodIcon.Tooltip = "Food";
                benefitsFoodLabel.Tooltip = "Food";
                buildingDialog.Items.Add(benefitsFoodLabel);
                buildingDialog.Items.Add(benefitsFoodIcon);
            }
            if (building.Benefits.Oil.Value > 0)
            {
                ScreenItemLabel benefitsOilLabel = new ScreenItemLabel(controls.Game, skin,
                    176 + (int)buildingIcon.Width, (int)benefitsHeight,
                    10, building.Benefits.Oil.Value.ToString() + " barrels / minute");
                ScreenItemImage benefitsOilIcon = new ScreenItemImage(controls.Game, skin,
                    160 + (int)buildingIcon.Width, (int)benefitsHeight, 16, 16, "OilIcon");
                benefitsHeight += benefitsOilLabel.Height;
                benefitsOilIcon.Tooltip = "Oil";
                benefitsOilLabel.Tooltip = "Oil";
                buildingDialog.Items.Add(benefitsOilLabel);
                buildingDialog.Items.Add(benefitsOilIcon);
            }
            if (building.Benefits.Electricity.Value > 0)
            {
                ScreenItemLabel benefitsElectricityLabel = new ScreenItemLabel(controls.Game, skin,
                    176 + (int)buildingIcon.Width, (int)benefitsHeight,
                    10, building.Benefits.Electricity.Value.ToString() + " kwh");
                ScreenItemImage benefitsElectricityIcon = new ScreenItemImage(controls.Game, skin,
                    160 + (int)buildingIcon.Width, (int)benefitsHeight, 16, 16, "ElectricityIcon");
                benefitsHeight += benefitsElectricityLabel.Height;
                benefitsElectricityIcon.Tooltip = "Electricity";
                benefitsElectricityLabel.Tooltip = "Electricity";
                buildingDialog.Items.Add(benefitsElectricityLabel);
                buildingDialog.Items.Add(benefitsElectricityIcon);
            }

            controls.OpenDialog = buildingDialog;
            buildingDialog.Closed += delegate()
            {
                controls.OpenDialog = null;
            };
        }
        private void TechnologyListItemSelected(ScreenItemList list, ScreenItemListItem item)
        {
            if (_playerGame.CurrentResearch.Count >= 3)
            {
                return;
            }
            Research research = (Research)item.AttachedInformation;

            ScreenItemDialog researchDialog = new ScreenItemDialog(controls.Game, skin,
                 research.Name, true);
            researchDialog.AttachedInformation = research;
            researchDialog.Items.Add(new ScreenItemImage(controls.Game, skin, 250, 5, research.Icon));
            ScreenItemButton researchButton = new ScreenItemButton(controls.Game, skin, 250 +
                (float)((research.Icon.Width - 125) / 2), research.Icon.Height + 10, 125, 20, "Begin Research");
            researchButton.OnClick += delegate(ScreenItem screenItem, MouseEventArgs args)
            {
                item.Enabled = false;
                _playerGame.BeginResearch(research);
                ReassignResearchProgressBars();
                researchDialog.Close();
            };
            researchDialog.Items.Add(researchButton);
            ScreenItemLabel description = new ScreenItemLabel(controls.Game, skin, 5, 5, 10, 240, research.Description);
            description.TextAlignment = ScreenItemTextAlignment.Center;
            researchDialog.Items.Add(description);
            controls.OpenDialog = researchDialog;
            researchDialog.Closed += delegate() { controls.OpenDialog = null; };
        }
        private void ReassignResearchProgressBars()
        {
            if (_playerGame.CurrentResearch.Count > 0)
                controls.ResearchOne.ResearchProgress = _playerGame.CurrentResearch[0];
            else
                controls.ResearchOne.ResearchProgress = null;
            
            if (_playerGame.CurrentResearch.Count > 1)
                controls.ResearchTwo.ResearchProgress = _playerGame.CurrentResearch[1];
            else
                controls.ResearchTwo.ResearchProgress = null;

            if (_playerGame.CurrentResearch.Count > 2)
                controls.ResearchThree.ResearchProgress = _playerGame.CurrentResearch[2];
            else
                controls.ResearchThree.ResearchProgress = null;
        }

        private void ShiftBuildingList(int direction)
        {
            int newOffset = availableBuildingListOffset + direction;
            if (newOffset < 0 || newOffset + 6 > _playerGame.AvailableBuildings.Count)
                return;
            controls.BuildingListLeft.Enabled = (newOffset != 0);
            controls.BuildingListRight.Enabled = (newOffset + 6 < _playerGame.AvailableBuildings.Count);
            int upperBounds = (_playerGame.AvailableBuildings.Count > 6 ? 6 : _playerGame.AvailableBuildings.Count);
            for (int button = 0; button < upperBounds; button++)
                controls.BuildingListButtons[button].Image = _playerGame.AvailableBuildings[newOffset + button].BuildingIcon;
            availableBuildingListOffset = newOffset;
        }

        void areaSelectionToggled(object sender, EventArgs e)
        {
            ScreenItemToggleButton sndr = (ScreenItemToggleButton)sender;
            if (sndr == controls.UrbanAreaSelection)
            {
                _currentMap = SimulationScreenMap.Urban;
                controls.SelectedBuilding.Building = _playerGame.UrbanMap.CurrentlySelectedBuilding;
            }
            else if (sndr == controls.CountryAreaSelection)
            {
                _currentMap = SimulationScreenMap.Country;
                controls.SelectedBuilding.Building = _playerGame.CountryMap.CurrentlySelectedBuilding;
            }
            else if (sndr == controls.OceanAreaSelection)
            {
                _currentMap = SimulationScreenMap.Ocean;
                controls.SelectedBuilding.Building = _playerGame.OceanMap.CurrentlySelectedBuilding;
            }
        }

        public void Update(GameTime gameTime, ref PlayerGame playerGame)
        {
            _playerGame = playerGame;
            List<ScreenItem> orderedScreenItems = controls.List.OrderByDescending<ScreenItem,
                int>(item => item.Layer).ToList<ScreenItem>();
            bool stillProcessingMouse = true;
            foreach (ScreenItem screenItem in orderedScreenItems)
            {
                if (screenItem == null)
                    continue;
                if (screenItem.Enabled && stillProcessingMouse)
                {
                    stillProcessingMouse = !screenItem.ProcessUpdateMouse();
                }
                screenItem.Update(gameTime);
            }
            MouseState mouseState = Mouse.GetState();
            controls.MoneyLabel.Text = ((int)Math.Floor(playerGame.Money)).ToString();
            controls.FoodLabel.Text = ((int)Math.Floor(playerGame.Food)).ToString();
            controls.OilLabel.Text = ((int)Math.Floor(playerGame.Oil)).ToString();

            List<ResearchProgress> currentResearchToBeUpdated = new List<ResearchProgress>();
            currentResearchToBeUpdated.AddRange(playerGame.CurrentResearch);
            foreach (ResearchProgress researchProgress in currentResearchToBeUpdated)
                researchProgress.Update(gameTime);

            Map loadedMap = null;
            switch (_currentMap)
            {
                case SimulationScreenMap.Urban: loadedMap = _playerGame.UrbanMap; break;
                case SimulationScreenMap.Country: loadedMap = _playerGame.CountryMap; break;
                case SimulationScreenMap.Ocean: loadedMap = _playerGame.OceanMap; break;
            }
            if (stillProcessingMouse && minimapVertices.PointInside(new Vector2(mouseState.X, mouseState.Y)))
            {
                loadedMap.ProcessMinimapMouse(new Vector2(minimapWindow.X - minimapWindow.Width / 2,
                    minimapWindow.Y - minimapWindow.Height / 2));
                stillProcessingMouse = false;
            }
            
            if (stillProcessingMouse)
                foreach (Rectangle interfaceRectangle in interfaceBlocks)
                {
                    if (mouseState.X >= interfaceRectangle.X && mouseState.Y >= interfaceRectangle.Y &&
                        mouseState.X <= interfaceRectangle.Right && mouseState.Y <= interfaceRectangle.Bottom)
                        stillProcessingMouse = false;
                }

            loadedMap.Update(gameTime, playerGame, mapWindow, stillProcessingMouse);
            outputText = loadedMap.output;
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
            currentMap.Draw(gameTime, spriteBatch, mapWindow, skin);
            if (currentMap.BuildingPlacementInfo != null)
            {
                Vector2 currentReasonLocation = new Vector2(GlobalSettings.ScreenWidth - 250, 100);
                foreach (string cannotPlaceReason in currentMap.BuildingPlacementInfo.ReasonsCannotPlace)
                {
                    string reason = "• " + cannotPlaceReason;
                    string[] lines = skin.Fonts[13].WordWrap(reason, 240);
                    foreach (string line in lines)
                    {
                        spriteBatch.DrawString(skin.Fonts[13], line, currentReasonLocation, Color.Red);
                        currentReasonLocation.Y += skin.Fonts[13].MeasureString(line).Y;
                    }
                }
            }

            /* Interface/Backdrops */
            spriteBatch.Draw(skin.SimulationScreenInterface, new Rectangle(0, 0, GlobalSettings.ScreenWidth,
                GlobalSettings.ScreenHeight), Color.White);

            /* Minimap */
            spriteBatch.Draw(currentMap.Minimap, minimapWindow, null, Color.White, MathHelper.PiOver4,
                Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.DrawLine(minimapVertices[0], minimapVertices[1], Color.Black);
            spriteBatch.DrawLine(minimapVertices[1], minimapVertices[2], Color.Black);
            spriteBatch.DrawLine(minimapVertices[2], minimapVertices[3], Color.Black);
            spriteBatch.DrawLine(minimapVertices[3], minimapVertices[0], Color.Black);

            /* Screen Items */
            if (outputText != null)
                spriteBatch.DrawString(outputText, new Vector2(mapWindow.X + 10, 
                    mapWindow.Y + 10), Color.Black);

            List<ScreenItem> orderedScreenItems = controls.List.OrderBy<ScreenItem, int>(item => item.Layer).ToList<ScreenItem>();
            List<ScreenItem> itemsUnderMouse = new List<ScreenItem>();
            foreach (ScreenItem screenItem in orderedScreenItems)
            {
                if (screenItem == null || !screenItem.Visible)
                    continue;
                screenItem.Draw(gameTime, spriteBatch);
                if (screenItem.GetMouseOver())
                    itemsUnderMouse.Add(screenItem);
            }
            if (itemsUnderMouse.Count > 0 && itemsUnderMouse[itemsUnderMouse.Count - 1].Visible &&
                itemsUnderMouse[itemsUnderMouse.Count - 1].HasTooltip)
                itemsUnderMouse[itemsUnderMouse.Count - 1].DrawTooltip(spriteBatch, graphicsDevice);
        }
    }

    public enum SimulationScreenMap
    {
        Urban,
        Country,
        Ocean
    }
}
