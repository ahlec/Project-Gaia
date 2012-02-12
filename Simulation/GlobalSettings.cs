using System;
using Microsoft.Xna.Framework;
using Simulation.Buildings;
using Simulation.ResearchLabs;

namespace Simulation
{
    public static class GlobalSettings
    {
        public const int TileWidth = 64;
        public const int TileHeight = 32;
        public static bool GameplayHalted = false;
        public const int ScreenWidth = 1050;
        public const int ScreenHeight = 700;
        public static BuildingCatalog BuildingCatalog;
        public static ResearchCatalog ResearchCatalog;
        public const int MinimapWidth = 128;
        public const int MinimapHeight = 128;
        public const bool DevelopmentMode = true;
    }
}
