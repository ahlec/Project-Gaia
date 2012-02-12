using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Simulation.Buildings
{
    public class BuildingCatalog : Dictionary<string, Building>
    {
        public static BuildingCatalog Load(string filepath, Game game)
        {
            BuildingCatalog catalog = new BuildingCatalog();
            Stream stream = new FileStream(filepath, FileMode.Open);
            XElement xml = XDocument.Load(XmlReader.Create(stream), LoadOptions.None).Element("Buildings");
            stream.Close();
            foreach (XElement buildingXML in xml.Elements("Building"))
            {
                Building building = Building.Load(buildingXML, game);
                catalog.Add(building.Handle, building);
            }
            return catalog;
        }
    }
}
