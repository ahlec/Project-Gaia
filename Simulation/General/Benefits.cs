using System;
using System.Xml.Linq;

namespace Simulation.General
{
    public class Benefits
    {
        private string handle;
        private Benefit<float> income, electricity, food, oil;
        private Benefit<string[]> buildings;
        private Benefit<string[]> research;

        public string Handle { get { return handle; } }
        public Benefit<float> Income { get { return income; } }
        public Benefit<string[]> Buildings { get { return buildings; } }
        public Benefit<string[]> Research { get { return research; } }
        public Benefit<float> Electricity { get { return electricity; } }
        public Benefit<float> Food { get { return food; } }
        public Benefit<float> Oil { get { return oil; } }

        public static Benefits Load(XElement xElement, string handle)
        {
            Benefits benefits = new Benefits();
            benefits.handle = handle;
            if (xElement.Element("Income") != null)
                benefits.income = new Benefit<float>(float.Parse(xElement.Element("Income").Value),
                    (xElement.Element("Income").Attribute("Repeatable") != null &&
                    xElement.Element("Income").Attribute("Repeatable").Value.Equals("True")));
            else
                benefits.income = new Benefit<float>(0, false);
            if (xElement.Element("Electricity") != null)
                benefits.electricity = new Benefit<float>(float.Parse(xElement.Element("Electricity").Value),
                    (xElement.Element("Electricity").Attribute("Repeatable") != null &&
                    xElement.Element("Electricity").Attribute("Repeatable").Value.ToLower().Equals("true")));
            else
                benefits.electricity = new Benefit<float>(0, false);
            if (xElement.Element("Food") != null)
                benefits.food = new Benefit<float>(float.Parse(xElement.Element("Food").Value), true);
            else
                benefits.food = new Benefit<float>(0, false);
            benefits.oil = new Benefit<float>((xElement.Element("Oil") != null ?
                float.Parse(xElement.Element("Oil").Value) : 0), true);
            if (xElement.Element("Buildings") != null)
                benefits.buildings = new Benefit<string[]>(xElement.Element("Buildings").Value.Split(','),
                    (xElement.Element("Buildings").Attribute("Repeatable") != null &&
                    xElement.Element("Buildings").Attribute("Repeatable").Value.Equals("True")));
            else
                benefits.buildings = new Benefit<string[]>(new string[] { }, false);
            if (xElement.Element("Research") != null)
                benefits.research = new Benefit<string[]>(xElement.Element("Research").Value.Split(','),
                    (xElement.Attribute("Repeatable") != null &&
                    xElement.Element("Research").Attribute("Repeatable").Value.Equals("True")));
            else
                benefits.research = new Benefit<string[]>(new string[] { }, false);
            return benefits;
        }
    }
}
