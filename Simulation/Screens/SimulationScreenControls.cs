using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.GUI;
using Simulation.Buildings;
using Simulation.ResearchLabs;

namespace Simulation.Screens
{
    public class SimulationScreenControls
    {
        public SimulationScreenControls(Game game, ApplicationSkin skin)
        {
            Game = game;
            ScreenItemToggleButtonGroup selectionGroup = new ScreenItemToggleButtonGroup(game);
            UrbanAreaSelection = new ScreenItemToggleButton(game, skin, 4,
                   518, 41, 41, selectionGroup);
            UrbanAreaSelection.Tooltip = "Urban Map";
            UrbanAreaSelection.Value = true;
            CountryAreaSelection = new ScreenItemToggleButton(game, skin, 51,
                518, 41, 41, selectionGroup);
            CountryAreaSelection.Tooltip = "Countryside Map";
            OceanAreaSelection = new ScreenItemToggleButton(game, skin, 99,
                518, 41, 41, selectionGroup);
            OceanAreaSelection.Tooltip = "Oceanic Map";
            PrimaryEcoBar = new ScreenItemProgressBar(game, skin, 5, 29, 132);
            SecondaryEcoBar = new ScreenItemProgressBar(game, skin, 884, 372, 162);
            MoneyLabel = new ScreenItemLabel(game, skin, 36, 222, 13, 101, 24, "[Money]");
            MoneyLabel.PaddingLeft = 10;
            MoneyLabel.Tooltip = "Your current money.";
            BuildingListLeft = new ScreenItemButton(game, skin, 157, 605, 30, 71, "");
            BuildingListRight = new ScreenItemButton(game, skin, 1010, 605, 30, 72, "");
            BuildingListOne = new ScreenItemButton(game, skin, 198, 596, 100, 96, "");
            BuildingListTwo = new ScreenItemButton(game, skin, 337, 595, 100, 96, "");
            BuildingListThree = new ScreenItemButton(game, skin, 484, 595, 100, 96, "");
            BuildingListFour = new ScreenItemButton(game, skin, 622, 595, 100, 96, "");
            BuildingListFive = new ScreenItemButton(game, skin, 763, 595, 100, 96, "");
            BuildingListSix = new ScreenItemButton(game, skin, 900, 595, 100, 96, "");
            BuildingDropdown = new ScreenItemList(game, skin, 5, 143, 132, 24, "Buildings");
            TechnologyList = new ScreenItemList(game, skin, 5, 98, 132, 24, "Technologies");
            FoodLabel = new ScreenItemLabel(game, skin, 36, 316, 13, 101, 25, "[Food]");
            FoodLabel.Tooltip = "Bushels of food";
            FoodLabel.PaddingLeft = 10;
            MoneyIcon = new ScreenItemImage(game, skin, 0, 210, "MoneyIcon");
            MoneyIcon.Tooltip = MoneyLabel.Tooltip;
            FoodIcon = new ScreenItemImage(game, skin, 0, 305, "FoodIcon");
            FoodIcon.Tooltip = FoodLabel.Tooltip;
            MinimapNorth = new ScreenItemButton(game, skin, 985, 421, 100, 25, null);
            MinimapNorth.Rotation = MathHelper.PiOver4;
            MinimapNorth.TextRotation = MathHelper.PiOver4;
            MinimapWest = new ScreenItemButton(game, skin, 950, 420, 25, 100, null);
            MinimapWest.Rotation = MathHelper.PiOver4;
            MinimapEast = new ScreenItemButton(game, skin, 1038, 508, 25, 100, null);
            MinimapEast.Rotation = MathHelper.PiOver4;
            MinimapSouth = new ScreenItemButton(game, skin, 897, 507, 101, 26, null);
            MinimapSouth.Rotation = MathHelper.PiOver4;
            MinimapSouth.TextRotation = MathHelper.PiOver4;
            OilLabel = new ScreenItemLabel(game, skin, 36, 270, 13, 101, "[Oil]");
            OilLabel.PaddingLeft = 10;
            OilLabel.Tooltip = "Your accumulated oil.";
            OilIcon = new ScreenItemImage(game, skin, 0, 258, "OilIcon");
            OilIcon.Tooltip = OilLabel.Tooltip;
            NewTechnologyNotice = new ScreenItemNewNotice(game, skin, 130, 90, new Vector2(0, 1),
                new Vector2(0, 25));
            NewTechnologyNotice.Visible = false;
            NewTechnologyNotice.Stop();
            List = new List<ScreenItem>();
            List.Add(UrbanAreaSelection);
            List.Add(CountryAreaSelection);
            List.Add(OceanAreaSelection);
            List.Add(PrimaryEcoBar);
            List.Add(SecondaryEcoBar);
            List.Add(MoneyLabel);
            List.Add(BuildingListLeft);
            List.Add(BuildingListRight);
            List.Add(BuildingListOne);
            List.Add(BuildingListTwo);
            List.Add(BuildingListThree);
            List.Add(BuildingListFour);
            List.Add(BuildingListFive);
            List.Add(BuildingListSix);
            List.Add(BuildingDropdown);
            List.Add(TechnologyList);
            List.Add(FoodLabel);
            BuildingListButtons = new List<ScreenItemButton>();
            BuildingListButtons.Add(BuildingListOne);
            BuildingListButtons.Add(BuildingListTwo);
            BuildingListButtons.Add(BuildingListThree);
            BuildingListButtons.Add(BuildingListFour);
            BuildingListButtons.Add(BuildingListFive);
            BuildingListButtons.Add(BuildingListSix);
            SelectedBuilding = new ScreenItemBuildingInformation(game, skin, 23, 596, 100, 96, null);
            List.Add(SelectedBuilding);
            List.Add(MoneyIcon);
            List.Add(FoodIcon);
//            List.Add(MinimapNorth);
//            List.Add(MinimapWest);
//            List.Add(MinimapEast);
//            List.Add(MinimapSouth);
            ResearchOne = new ScreenItemResearchProgress(game, skin, 5, 380, 132, null);
            ResearchTwo = new ScreenItemResearchProgress(game, skin, 5, 410, 132, null);
            ResearchThree = new ScreenItemResearchProgress(game, skin, 5, 441, 132, null);
            List.Add(ResearchOne);
            List.Add(ResearchTwo);
            List.Add(ResearchThree);
            List.Add(OilLabel);
            List.Add(OilIcon);
            List.Add(NewTechnologyNotice);
        }
        public Game Game { get; set; }
        public ScreenItemToggleButton UrbanAreaSelection { get; set; }
        public ScreenItemToggleButton CountryAreaSelection { get; set; }
        public ScreenItemToggleButton OceanAreaSelection { get; set; }
        /// <summary>
        /// The primary eco bar is the eco bar along the left side of the screen
        /// </summary>
        public ScreenItemProgressBar PrimaryEcoBar { get; set; }
        /// <summary>
        /// The secondary eco bar is the eco bar directly above the minimap
        /// </summary>
        public ScreenItemProgressBar SecondaryEcoBar { get; set; }
        public ScreenItemLabel MoneyLabel { get; set; }
        public ScreenItemButton BuildingListLeft { get; set; }
        public ScreenItemButton BuildingListRight { get; set; }
        public ScreenItemButton BuildingListOne { get; set; }
        public ScreenItemButton BuildingListTwo { get; set; }
        public ScreenItemButton BuildingListThree { get; set; }
        public ScreenItemButton BuildingListFour { get; set; }
        public ScreenItemButton BuildingListFive { get; set; }
        public ScreenItemButton BuildingListSix { get; set; }
        public ScreenItemList BuildingDropdown { get; set; }
        public ScreenItemList TechnologyList { get; set; }
        public ScreenItemLabel FoodLabel { get; set; }
        protected ScreenItemDialog openDialog;
        public ScreenItemDialog OpenDialog
        {
            get { return openDialog; }
            set
            {
                if (value == null)
                    List.Remove(openDialog);
                else
                    List.Add(value);
                openDialog = value;
            }
        }
        public List<ScreenItem> List { get; set; }
        public List<ScreenItemButton> BuildingListButtons { get; set; }
        public ScreenItemBuildingInformation SelectedBuilding { get; set; }
        public ScreenItemImage MoneyIcon { get; set; }
        public ScreenItemImage FoodIcon { get; set; }
        public ScreenItemButton MinimapWest { get; set; }
        public ScreenItemButton MinimapEast { get; set; }
        public ScreenItemButton MinimapNorth { get; set; }
        public ScreenItemButton MinimapSouth { get; set; }
        public ScreenItemResearchProgress ResearchOne { get; set; }
        public ScreenItemResearchProgress ResearchTwo { get; set; }
        public ScreenItemResearchProgress ResearchThree { get; set; }
        public ScreenItemLabel OilLabel { get; set; }
        public ScreenItemImage OilIcon { get; set; }
        public ScreenItemNewNotice NewTechnologyNotice { get; set; }
    }
}
