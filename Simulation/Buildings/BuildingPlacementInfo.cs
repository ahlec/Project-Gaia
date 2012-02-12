using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simulation.Buildings
{
    public class BuildingPlacementInfo
    {
        public BuildingPlacementInfo(Building building, Vector2 initialPlacementLocation)
        {
            Building = building;
            TopTilePlacementLocation = initialPlacementLocation;
            ReasonsCannotPlace = new string[0];
        }
        public Building Building { get; set; }
        public int Width { get { return (int)Building.Size.X; } }
        public int Height { get { return (int)Building.Size.Y; } }
        public bool CanPlace { get; set; }
        public Vector2 TopTilePlacementLocation { get; set; }
        public string[] ReasonsCannotPlace { get; set; }
    }
}
