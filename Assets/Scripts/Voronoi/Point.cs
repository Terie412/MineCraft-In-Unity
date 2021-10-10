using UnityEngine;

namespace VoronoiEngine
{
    public class Point
    {
        public float x, y;
        public bool isCheck = false;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        /// <returns>return positive for left, negative for right</returns>
        public float GetSideFromEdge(Point edgeStart, Point edgeEnd)
        {
            float x1 = edgeStart.x;
            float y1 = edgeStart.y;
            float x2 = edgeEnd.x;
            float y2 = edgeEnd.y;
            return x1 * (y2 - y) + y1 * (x - x2) + x2 * y - x * y2;
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