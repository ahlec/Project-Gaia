using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Tiles;
using Simulation.Graphics;
using Simulation.General;

namespace Simulation.Buildings
{
    public class Building : DrawableGameComponent, IBuilding
    {
        private Dictionary<Vector2, Texture2DReference> buildingTextures = new Dictionary<Vector2, Texture2DReference>();
        protected Vector2 mapLocation;
        protected Vector2 size = new Vector2(1, 1);
        protected bool onMap = false, initiallyAvailable;
        protected Texture2D buildingIcon;
        protected string name, description, handle;
        private Terrain validTerrainTypes;
        private Benefits benefits;
        private Requirements requirements;

        public string Handle { get { return handle; } }
        public bool InitiallyAvailable { get { return initiallyAvailable; } }
        public Terrain ValidTerrainTypes { get { return validTerrainTypes; } }
        public Dictionary<Vector2, Texture2DReference> Textures { get { return buildingTextures; } }
        public Benefits Benefits { get { return benefits; } }
        public Requirements Requirements { get { return requirements; } }

        public Building(Game game) : base(game) { }

        public static Building Load(XElement xElement, Game game)
        {
            Building building = new Building(game);
            building.name = xElement.Element("Name").Value;
            building.handle = xElement.Attribute("Handle").Value;
            building.initiallyAvailable = (xElement.Element("InitiallyAvailable") != null);
            building.requirements = Requirements.Load(xElement.Element("Requirements"));
            building.buildingIcon = game.Content.Load<Texture2D>(xElement.Element("Icon").Value);
            building.size.X = Int32.Parse(xElement.Element("Size").Value.Substring(0,
                xElement.Element("Size").Value.IndexOf("x")));
            building.size.Y = Int32.Parse(xElement.Element("Size").Value.Substring(xElement.Element("Size").Value.IndexOf("x") + 1));
            building.description = xElement.Element("Description").Value;
            string[] validTerrainTypes = xElement.Element("Requirements").Element("Terrain").Value.Split(new char[] { ',' });
            foreach (string terrainType in validTerrainTypes)
                building.validTerrainTypes |= (Terrain)Enum.Parse(typeof(Terrain), terrainType);
            foreach (XElement textureElement in xElement.Element("Textures").Elements("Texture"))
            {
                Color color = Color.TransparentWhite;
                if (textureElement.Attribute("Color") != null)
                {
                    string[] pieces = textureElement.Attribute("Color").Value.Split(',');
                    color = new Color(int.Parse(pieces[0]), int.Parse(pieces[1]), int.Parse(pieces[2]));
                }
                building.buildingTextures.Add(new Vector2(int.Parse(textureElement.Attribute("X").Value),
                    int.Parse(textureElement.Attribute("Y").Value)),
                    new Texture2DReference(game.Content.Load<Texture2D>(textureElement.Value), color));
            }
            building.benefits = Benefits.Load(xElement.Element("Benefits"), building.handle);
            return building;
        }

        public int BaseEcoFactor { get; set; }
        public Vector2 Size { get { return size; } }
        public Vector2 MapLocation { get { return mapLocation; } }
        public bool OnMap { get { return onMap; } }
        public Texture2D BuildingIcon { get { return buildingIcon; } }
        public string Name { get { return name; } }
        public string Description { get { return description; } }

        public void PlaceBuilding(Vector2 location)
        {
            onMap = true;
            mapLocation = location;
        }

    }
}
