using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation.Buildings
{
    /// <summary>
    /// This interface is to make sure we can get the cost factor and
    /// eco factor of any item that is in our game. It also allows us 
    /// a generic way of looking at buildings and add-ons with no differentiation.
    /// </summary>
    interface ISimItem
    {
        /// <summary>
        /// Gets or sets the eco factor of this item.
        /// </summary>
        int EcoFactor { get; }

        /// <summary>
        /// Gets or sets the eco factor of this item.
        /// </summary>
        int CostFactor { get; }
    }
}
