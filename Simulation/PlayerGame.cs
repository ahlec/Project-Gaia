using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Simulation.Buildings;
using Simulation.Tiles;
using Simulation.Maps;
using Simulation.GUI;
using Simulation.ResearchLabs;
using Simulation.General;

namespace Simulation
{
    public class PlayerGame
    {
        public PlayerGame(Game game, ApplicationSkin skin)
        {
            UrbanMap = new UrbanMap(game, skin, 16, 16);
            CountryMap = new CountrysideMap(game, skin, 20, 20);
            OceanMap = new OceanMap(game, skin, 20, 20);
            AvailableBuildings = new List<Building>();
            AvailableResearches = new List<Research>();
            CurrentResearch = new List<ResearchProgress>();
            Money = 10000;
            Food = 10000;
            Oil = 10000;
        }
        public Map UrbanMap { get; set; }
        public Map CountryMap { get; set; }
        public Map OceanMap { get; set; }
        public float Money { get; set; }
        public float Food { get; set; }
        public float Oil { get; set; }
        public List<Building> AvailableBuildings { get; set; }
        public List<Research> AvailableResearches { get; set; }
        public float Income { get; set; }
        public float FoodIncome { get; set; }
        public float OilIncome { get; set; }
        public float Electricity { get; set; }
        public float ConsumedElectricity { get; set; }

        public List<ResearchProgress> CurrentResearch { get; set; }
        public event ResearchCompletedHandler ResearchCompleted;

        public void Update(GameTime gameTime)
        {
            if (Money + (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 60000) * Income > float.MaxValue)
                Money = float.MaxValue;
            else
                Money += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 60000) * Income;
            if (Food + (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 60000) * FoodIncome > float.MaxValue)
                Food = float.MaxValue;
            else
                Food += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 60000) * FoodIncome;
            if (OilIncome > 0)
            {
                if (Oil + (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 60000) * OilIncome > float.MaxValue)
                    Oil = float.MaxValue;
                else
                    Oil += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 60000) * OilIncome;
            }
        }

        public void BeginResearch(Research research)
        {
            if (CurrentResearch.Count >= 3)
                return;
            ResearchProgress progress = new ResearchProgress(research);
            progress.ResearchCompleted += delegate(ResearchProgress researchProgress)
            {
                CurrentResearch.Remove(researchProgress);
                AvailableResearches.Find(available => available.Handle.Equals(research.Handle)).Completed = true;
                if (ResearchCompleted != null)
                    ResearchCompleted.Invoke(researchProgress.Research);
            };
            CurrentResearch.Add(progress);
        }
        public void Detract(Requirements requirements)
        {
            Money -= requirements.Money;
            Food -= requirements.Food;
            ConsumedElectricity += requirements.Electricity;
            Oil -= requirements.Oil;
        }

        private List<string> appliedBenefits = new List<string>();
        public void ApplyBenefits(Benefits benefits)
        {
            Income += benefits.Income.Value;
            Electricity += benefits.Electricity.Value;
            FoodIncome += benefits.Food.Value;
            OilIncome += benefits.Oil.Value;
            if (benefits.Buildings.Repeatable || !appliedBenefits.Contains(benefits.Handle))
                if (benefits.Buildings.Value.Length > 0)
                {
                    foreach (string buildingHandle in benefits.Buildings.Value)
                        AvailableBuildings.Add(GlobalSettings.BuildingCatalog[buildingHandle]);
                    if (BuildingsMadeAvailable != null)
                        BuildingsMadeAvailable.Invoke();
                }
            if (benefits.Research.Repeatable || !appliedBenefits.Contains(benefits.Handle))
                if (benefits.Research.Value.Length > 0)
                {
                    foreach (string researchHandle in benefits.Research.Value)
                        if (!AvailableResearches.Exists(research => research.Handle.Equals(researchHandle)))
                            AvailableResearches.Add(new Research(GlobalSettings.ResearchCatalog[researchHandle]));
                    if (ResearchMadeAvailable != null)
                        ResearchMadeAvailable.Invoke();
                }
            if (!appliedBenefits.Contains(benefits.Handle))
                appliedBenefits.Add(benefits.Handle);
        }
        public event BuildingsMadeAvailableHandler BuildingsMadeAvailable;
        public event ResearchMadeAvailableHandler ResearchMadeAvailable;
    }
    public delegate void BuildingsMadeAvailableHandler();
    public delegate void ResearchMadeAvailableHandler();
}
