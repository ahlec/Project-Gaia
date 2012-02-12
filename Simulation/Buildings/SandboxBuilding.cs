using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simulation.Buildings
{
    public class SandboxBuilding : Building
    {
        public SandboxBuilding(Game game)
            : base(game)
        {
            size = new Vector2(2, 2);
        }
    }
}
