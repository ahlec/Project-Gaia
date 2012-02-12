using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation.General
{
    public class Benefit<Cast>
    {
        public Benefit(Cast value, bool repeatable)
        {
            this.benefit = value;
            this.repeatable = repeatable;
        }
        private bool repeatable;
        private Cast benefit;
        public Cast Value { get { return benefit; } }
        public bool Repeatable { get { return repeatable; } }
    }
}
