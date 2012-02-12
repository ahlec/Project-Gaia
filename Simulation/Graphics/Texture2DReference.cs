using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.Graphics
{
    public class Texture2DReference
    {
        public Texture2DReference(Texture2D texture, Color color)
        {
            this.texture = texture;
            this.color = color;
        }
        private Texture2D texture;
        private Color color;
        public Texture2D Texture { get { return texture; } }
        public Color MonoColor { get { return color; } }
        public static implicit operator Texture2D(Texture2DReference reference)
        {
            return reference.texture;
        }
    }
}
