/* Did have code in here before but didn't like design.
 * Removed for now with possibility of implementation later.
 * Likely to be deleted
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simulation.Buildings
{
    public interface IBuilding
    {
        Vector2 MapLocation { get; }
        Vector2 Size { get; }
        bool OnMap { get; }
        void PlaceBuilding(Vector2 location);
    }
}
