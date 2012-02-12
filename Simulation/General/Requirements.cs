using System;
using System.Xml.Linq;

namespace Simulation.General
{
    public class Requirements
    {
        private float money, food, electricity, oil;
        public float Money { get { return money; } }
        public float Food { get { return food; } }
        public float Electricity { get { return electricity; } }
        public float Oil { get { return oil; } }
        public static Requirements Load(XElement xElement)
        {
            Requirements requirements = new Requirements();
            requirements.money = (xElement.Element("Money") != null ?
                float.Parse(xElement.Element("Money").Value) : 0);
            requirements.food = (xElement.Element("Food") != null ?
                float.Parse(xElement.Element("Food").Value) : 0);
            requirements.electricity = (xElement.Element("Electricity") != null ?
                float.Parse(xElement.Element("Electricity").Value) : 0);
            requirements.oil = (xElement.Element("Oil") != null ?
                float.Parse(xElement.Element("Oil").Value) : 0);
            return requirements;
        }
        public bool AreMet(PlayerGame playerGame)
        {
            return (playerGame.Money >= money && playerGame.Food >= food &&
                playerGame.Electricity - playerGame.ConsumedElectricity >= electricity &&
                playerGame.Oil >= oil);
        }
    }
}
