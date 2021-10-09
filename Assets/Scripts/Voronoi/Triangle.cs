using UnityEngine;

namespace VoronoiEngine
{
    public class Triangle
    {
        public Point[] points;

        private Circle _circumcircle;

        public Circle circumcircle => _circumcircle;

        public Triangle(Point[] points)
        {
            this.points = points;

            float a = Point.Distance(points[0], points[1]);
            float b = Point.Distance(points[1], points[2]);
            float c = Point.Distance(points[2], points[0]);
            float p = (a + b + c) / 3;
            float r = (a * b * c) / (4 * Mathf.Sqrt(p * (p - a) * (p - b) * (p - c)));
            float x1 = points[0].x;
            float x2 = points[1].x;
            float x3 = points[2].x;
            float y1 = points[0].y;
            float y2 = points[1].y;
            float y3 = points[2].y;
            float x1P = x1 * x1;
            float x2P = x2 * x2;
            float x3P = x3 * x3;
            float y1P = y1 * y1;
            float y2P = y2 * y2;
            float y3P = y3 * y3;
            float A1 = 2 * (x2 - x1);
            float B1 = 2 * (y2 - y1);
            float C1 = x2P + y2P - x1P - y1P;
            float A2 = 2 * (x3 - x2);
            float B2 = 2 * (y3 - y2);
            float C2 = x3P + y3P - x2P - y2P;
            float den = A1 * B2 - A2 * B1;
            float x = (C1 * B2 - C2 * B1) / den;
            float y = (A1 * C2 - A2 * C1) / den;

            _circumcircle = new Circle(new Point(x, y), r);
        }

        public override string ToString()
        {
            return $"point[{points[0]}, {points[1]}, {points[2]}], circumcircle={circumcircle}";
        }
    }
}