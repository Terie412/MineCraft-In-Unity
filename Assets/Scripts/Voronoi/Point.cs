using UnityEngine;

namespace VoronoiEngine
{
    public class Point
    {
        public float x, y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        public static float Distance(Point a, Point b)
        {
            return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
        }

        public static float DistancePow(Point a, Point b)
        {
            return Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}