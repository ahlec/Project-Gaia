using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.General;

namespace Simulation.ResearchLabs
{
    public class ResearchBase
    {
        protected string name, handle, description;
        protected int monetaryCost;
        protected Benefits benefits;
        protected Texture2D icon;
        protected TimeSpan researchDuration;
        protected bool initiallyAvailable;

        public string Name { get { return name; } }
        public string Handle { get { return handle; } }
        public string Description { get { return description; } }
        public int MonetaryCost { get { return monetaryCost; } }
        public Texture2D Icon { get { return icon; } }
        public TimeSpan ResearchDuration { get { return researchDuration; } }
        public Benefits Benefits { get { return benefits; } }
        public bool InitiallyAvailable { get { return initiallyAvailable; } }

        public static ResearchBase Load(XElement xElement, Game game)
        {
            ResearchBase research = new ResearchBase();
            research.handle = xElement.Attribute("Handle").Value;
            research.name = xElement.Element("Name").Value;
            research.description = xElement.Element("Description").Value;
            research.monetaryCost = Int32.Parse(xElement.Element("MonetaryCost").Value);
            research.icon = game.Content.Load<Texture2D>(xElement.Element("Icon").Value);
            research.researchDuration = TimeSpan.FromSeconds(double.Parse(xElement.Element("Duration").Value));
            research.benefits = Benefits.Load(xElement.Element("Benefits"), research.handle);
            research.initiallyAvailable = (xElement.Element("InitiallyAvailable") != null);
            return research;
        }
    }
}
