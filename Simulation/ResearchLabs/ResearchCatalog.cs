using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Simulation.ResearchLabs
{
    public class ResearchCatalog : Dictionary<string, ResearchBase>
    {
        public static ResearchCatalog Load(string filepath, Game game)
        {
            ResearchCatalog researchCatalog = new ResearchCatalog();
            Stream stream = new FileStream(filepath, FileMode.Open);
            XElement xml = XDocument.Load(XmlReader.Create(stream), LoadOptions.None).Element("ResearchLabs");
            stream.Close();
            foreach (XElement researchElement in xml.Elements("Research"))
            {
                ResearchBase research = ResearchBase.Load(researchElement, game);
                researchCatalog.Add(research.Handle, research);
            }
            return researchCatalog;
        }
    }
}
