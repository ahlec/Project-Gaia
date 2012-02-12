using System;
using Microsoft.Xna.Framework;

namespace Simulation.Graphics
{
    public static class PolygonExtensions
    {
        public static bool PointInside(this Vector2[] vertices, Vector2 point)
        {
            int bordersCrossed = 0;
            int previousIndex = vertices.Length - 1;
            for (int index = 0; index < vertices.Length; index++)
            {
                if (vertices[index].Y < point.Y && vertices[previousIndex].Y >= point.Y ||
                    vertices[previousIndex].Y < point.Y && vertices[index].Y >= point.Y)
                {
                    if (vertices[index].X + (point.Y - vertices[index].Y) / (vertices[previousIndex].Y -
                        vertices[index].Y) * (vertices[previousIndex].X - vertices[index].X) < point.X)
                        bordersCrossed++;
                }
                previousIndex = index;
            }
            return (bordersCrossed % 2 == 1);
        }
        public static bool PointInside(this Vector2[] vertices, float x, float y)
        {
            return vertices.PointInside(new Vector2(x, y));
        }
    }
}
