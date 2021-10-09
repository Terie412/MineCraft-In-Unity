using UnityEngine;

namespace VoronoiEngine
{
    public class Circle
    {
        public Point center;
        public float radius;

        private float radiusPow;

        public Circle(Point center, float radius)
        {
            this.center = center;
            this.radius = radius;

            radiusPow = Mathf.Pow(this.radius, 2);
        }

        public bool IsContainPoint(Point point)
        {
            return Point.DistancePow(point, center) >= radiusPow;
        }

        public override string ToString()
        {
            return $"center:{center}, radius:{radius}";
        }
    }
}