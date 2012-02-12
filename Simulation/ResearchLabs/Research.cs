using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation.ResearchLabs
{
    public class Research : ResearchBase
    {
        public Research(ResearchBase researchBase)
        {
            this.benefits = researchBase.Benefits;
            this.description = researchBase.Description;
            this.handle = researchBase.Handle;
            this.icon = researchBase.Icon;
            this.initiallyAvailable = researchBase.InitiallyAvailable;
            this.monetaryCost = researchBase.MonetaryCost;
            this.name = researchBase.Name;
            this.researchDuration = researchBase.ResearchDuration;
        }
        public bool Completed { get; set; }
    }
}
