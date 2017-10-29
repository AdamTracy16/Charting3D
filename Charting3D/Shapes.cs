using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charting3D
{
    public class Shapes
    {
        public abstract class Shape
        {
            public string Name { get; set; }
            public abstract double Area();
        }

        public class BoundingBox : Shape
        {
            public double Width { get; set; }
            public double Height { get; set; }
            public double Length { get; set; }

            public BoundingBox(string name, double width, double height, double length)
            {
                this.Name = name;
                this.Width = width;
                this.Height = height;
                this.Length = length;
            }

            public override double Area()
            {
                double area = this.Width * this.Height * this.Length;
                return area;
            }
        }

        public class Point : Shape
        {
            public Point(string name)
            {
                this.Name = name;
            }

            public override double Area()
            {
                return 0;
            }
        }

        public class Cylinder : Shape
        {
            public double Radius { get; set; }
            public double Facet_number { get; set; }
            public double Length { get; set; }

            public Cylinder(string name, double radius, double facet_number, double length)
            {
                this.Name = name;
                this.Radius = radius;
                this.Facet_number = facet_number;
                this.Length = length;
            }

            public override double Area()
            {
                double area = this.Radius * this.Radius * Math.PI * Length;
                return area;
            }
        }
    }
}
