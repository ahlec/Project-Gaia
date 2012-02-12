using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Simulation
{
    public class Soundboard
    {
        public Soundboard()
        {
        }
        protected Song[] songs;
        public Song[] Songs { get { return songs; } }
    }
}
