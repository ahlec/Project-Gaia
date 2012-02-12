using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Simulation.Buildings
{
    /// <summary>
    /// Addons are put on buildings which will cause a change in ecofactor
    /// and cost of a building. You can not use AddOn alone. It must be the base
    /// class of another item such as SunPanels.
    /// </summary>
    abstract class AddOn : GameComponent, ISimItem
    {
        #region Members

        // This is the magic to the whole game. It will be a value between 0 and 100
        int ecoFactor;

        // This is the magic to the whole game. It will be a value between 0 and 100
        int costFactor;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the eco factor of the building. Inclusive value between 0-100
        /// </summary>
        public int EcoFactor
        {
            get { return ecoFactor; }
            protected set
            {
                // Let's make sure we are allowed to set this value
                if (value > 100 || value < 0)
                    throw new NotSupportedException("Eco factor is only allowed to be between 0-100 inclusively.");

                ecoFactor = value;
            }
        }

        /// <summary>
        /// Get or set the eco factor of the building. Inclusive value between 0-100
        /// </summary>
        public int CostFactor
        {
            get { return costFactor; }
            protected set
            {
                // Let's make sure we are allowed to set this value
                if (value > 100 || value < 0)
                    throw new NotSupportedException("Eco factor is only allowed to be between 0-100 inclusively.");

                costFactor = value;
            }
        }

        #endregion

        // This may not be needed depending on how AddOns will work in the end
        #region Main Methods

        public AddOn(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion
    }
}
