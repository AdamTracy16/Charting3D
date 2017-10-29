using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using p3D = System.Windows.Media.Media3D;

namespace Charting3D
{
    public partial class ucCharting3D : UserControl
    {
        public ucCharting3D()
        {
            InitializeComponent();
        }

        double x = 0.0;
        double y = 0.0;
        double z = 0.0;

        //Bounding Box plotting methods

        public void plotSetup(Shapes.BoundingBox box)
        {
            string[] seriesNames = boxSeriesNames(box);
            foreach (string name in seriesNames)
            {
                chart.Series.Add(name);
                chart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart.Series[name].Color = System.Drawing.Color.Blue;
            }
        }

        public void plotSetup(Shapes.BoundingBox box, System.Drawing.Color color)
        {
            string[] seriesNames = boxSeriesNames(box);
            foreach (string name in seriesNames)
            {
                chart.Series.Add(name);
                chart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart.Series[name].Color = color;
            }
        }

        public void Plot(Shapes.BoundingBox box, p3D.Point3D center, double rot_x, double rot_y, double rot_z, bool rendered)
        {
            string[] seriesNames = boxSeriesNames(box);
            foreach (string name in seriesNames)
                chart.Series[name].Points.Clear();
            if (rendered)
                boxPlotterRendered(box, rot_x, rot_y, rot_z, box.Width, box.Height, box.Length, center);
            else
                boxPlotterWire(box, rot_x, rot_y, rot_z, box.Width, box.Height, box.Length, center);
            chartSizer(chart);
        }

        private void boxPlotterWire(Shapes.BoundingBox box, double rot_x, double rot_y, double rot_z, double width, double height, double length, p3D.Point3D center)
        {
            string[] seriesNames = boxSeriesNames(box);
            Point3D[] polarity = boxCornerPolarities();
            List<DataPoint> points = new List<DataPoint>();
            for (int i = 0; i < 8; i++)
            {
                Convert3DTo2D(rot_x, rot_y, rot_z, width / 2 * polarity[i].X + center.X, height / 2 * polarity[i].Y + center.Y, length / 2 * polarity[i].Z + center.Z, ref x, ref y, ref z);
                DataPoint point = new DataPoint(x, y);
                points.Add(point);
            }
            for (int i = 0; i < 6; i++)
            {
                int[] point = boxSidePoints(i);
                for (int j = 0; j < 5; j++)
                    chart.Series[seriesNames[i]].Points.Add(points[point[j]]);
            }
        }

        private void boxPlotterRendered(Shapes.BoundingBox box, double rot_x, double rot_y, double rot_z, double width, double height, double length, p3D.Point3D center)
        {
            string[] seriesNames = boxSeriesNames(box);
            Point3D[] polarity = boxCornerPolarities();
            double inc = 300;
            double[] dimensions = { width, height, length };
            int[] sides = { 5, 0, 4 };
            Point3D dir0 = new Point3D(1, 0, 0);
            Point3D dir1 = new Point3D(0, 1, 0);
            Point3D dir2 = new Point3D(0, 0, 1);
            Point3D[] incDirection = { dir0, dir1, dir2 };
            for (int i = 0; i < 3; i++)
            {
                int[] sidePoints = boxSidePoints(sides[i]);
                for (double j = 0; j < dimensions[i]; j = j + dimensions[i] / inc)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        Convert3DTo2D(rot_x, rot_y, rot_z, width / 2 * polarity[sidePoints[k]].X + center.X + j * incDirection[i].X, height / 2 * polarity[sidePoints[k]].Y + center.Y + j * incDirection[i].Y,
                            length / 2 * polarity[sidePoints[k]].Z + center.Z + j * incDirection[i].Z, ref x, ref y, ref z);
                        chart.Series[seriesNames[sides[i]]].Points.AddXY(x, y);
                    }
                }
            }
        }

        private string[] boxSeriesNames(Shapes.BoundingBox box)
        {
            string Face0 = String.Format("boxFace0_{0}", box.Name);
            string Face1 = String.Format("boxFace1_{0}", box.Name);
            string Face2 = String.Format("boxFace2_{0}", box.Name);
            string Face3 = String.Format("boxFace3_{0}", box.Name);
            string Face4 = String.Format("boxFace4_{0}", box.Name);
            string Face5 = String.Format("boxFace5_{0}", box.Name);
            string[] series = { Face0, Face1, Face2, Face3, Face4, Face5 };
            return series;
        }

        public void deleteShape(Shapes.BoundingBox box)
        {
            string[] seriesNames = boxSeriesNames(box);
            foreach (string name in seriesNames)
            {
                Series series = chart.Series[name];
                chart.Series.Remove(series);
            }
        }

        private Point3D[] boxCornerPolarities()
        {
            Point3D p0 = new Point3D(-1, -1, -1);
            Point3D p1 = new Point3D(-1, -1, 1);
            Point3D p2 = new Point3D(1, -1, 1);
            Point3D p3 = new Point3D(1, -1, -1);
            Point3D p4 = new Point3D(-1, 1, -1);
            Point3D p5 = new Point3D(-1, 1, 1);
            Point3D p6 = new Point3D(1, 1, 1);
            Point3D p7 = new Point3D(1, 1, -1);
            Point3D[] polarities = { p0, p1, p2, p3, p4, p5, p6, p7 };
            return polarities;
        }

        private int[] boxSidePoints(int n)
        {
            List<int[]> sides = new List<int[]>();
            int[] side0 = { 0, 1, 2, 3, 0 };
            sides.Add(side0);
            int[] side1 = { 1, 5, 6, 2, 1 };
            sides.Add(side1);
            int[] side2 = { 2, 6, 7, 3, 2 };
            sides.Add(side2);
            int[] side3 = { 4, 5, 6, 7, 4 };
            sides.Add(side3);
            int[] side4 = { 0, 3, 7, 4, 0 };
            sides.Add(side4);
            int[] side5 = { 0, 1, 5, 4, 0 };
            sides.Add(side5);
            return sides[n];
        }

        //Cylinder plotting methods

        public void plotSetup(Shapes.Cylinder cylinder)
        {
            string[] series = cylinderSeriesNames(cylinder);
            foreach (string name in series)
            {
                chart.Series.Add(name);
                chart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart.Series[name].Color = System.Drawing.Color.Blue;
            }
        }

        public void plotSetup(Shapes.Cylinder cylinder, System.Drawing.Color color)
        {
            string[] series = cylinderSeriesNames(cylinder);
            foreach (string name in series)
            {
                chart.Series.Add(name);
                chart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart.Series[name].Color = color;
            }
        }

        public void Plot(Shapes.Cylinder cylinder, p3D.Point3D p1, p3D.Point3D p2, double rot_x, double rot_y, double rot_z, bool rendered)
        {
            string[] series = cylinderSeriesNames(cylinder);
            foreach (var name in series)
                chart.Series[name].Points.Clear();
            if (rendered)
                cylinderPlotterRendered(cylinder, p1, p2, rot_x, rot_y, rot_z);
            else
                cylinderPlotterWire(cylinder, p1, p2, rot_x, rot_y, rot_z);
            chartSizer(chart);
        }

        private void cylinderPlotterWire(Shapes.Cylinder cylinder, p3D.Point3D p1, p3D.Point3D p2, double rot_x, double rot_y, double rot_z)
        {
            string[] seriesNames = cylinderSeriesNames(cylinder);
            p3D.Vector3D p1p2 = new p3D.Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            Random rand = new Random();
            p3D.Point3D p_rand = new p3D.Point3D(rand.NextDouble() * 100.0, rand.NextDouble() * 100.0, rand.NextDouble() * 100.0);
            p3D.Vector3D pv0 = new p3D.Vector3D(p_rand.X - p1.X, p_rand.Y - p1.Y, p_rand.Z - p1.Z);
            p3D.Vector3D r0 = p3D.Vector3D.CrossProduct(p1p2, pv0);
            p3D.Vector3D s0 = p3D.Vector3D.CrossProduct(p1p2, r0);
            r0.Normalize();
            s0.Normalize();
            double theta_inc = Math.PI * 2.0 / 36;
            for (double theta = 0; theta < Math.PI * 2.0; theta += theta_inc)
            {
                p3D.Point3D n = new p3D.Point3D(0.0, 0.0, 0.0);
                n.X = p1.X + cylinder.Radius * Math.Cos(theta) * r0.X + cylinder.Radius * Math.Sin(theta) * s0.X;
                n.Y = p1.Y + cylinder.Radius * Math.Cos(theta) * r0.Y + cylinder.Radius * Math.Sin(theta) * s0.Y;
                n.Z = p1.Z + cylinder.Radius * Math.Cos(theta) * r0.Z + cylinder.Radius * Math.Sin(theta) * s0.Z;
                Convert3DTo2D(rot_x, rot_y, rot_z, n.X, n.Y, n.Z, ref x, ref y, ref z);
                chart.Series[seriesNames[0]].Points.AddXY(x, y);
            }

            p3D.Vector3D p2p1 = new p3D.Vector3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
            p3D.Vector3D pv1 = new p3D.Vector3D(p_rand.X - p2.X, p_rand.Y - p2.Y, p_rand.Z - p2.Z);
            p3D.Vector3D r1 = p3D.Vector3D.CrossProduct(p2p1, pv1);
            p3D.Vector3D s1 = p3D.Vector3D.CrossProduct(p2p1, r1);
            r1.Normalize();
            s1.Normalize();
            for (double theta = 0; theta < Math.PI * 2.0; theta += theta_inc)
            {
                p3D.Point3D n = new p3D.Point3D(0.0, 0.0, 0.0);
                n.X = p2.X + cylinder.Radius * Math.Cos(theta) * r1.X + cylinder.Radius * Math.Sin(theta) * s1.X;
                n.Y = p2.Y + cylinder.Radius * Math.Cos(theta) * r1.Y + cylinder.Radius * Math.Sin(theta) * s1.Y;
                n.Z = p2.Z + cylinder.Radius * Math.Cos(theta) * r1.Z + cylinder.Radius * Math.Sin(theta) * s1.Z;
                Convert3DTo2D(rot_x, rot_y, rot_z, n.X, n.Y, n.Z, ref x, ref y, ref z);
                chart.Series[seriesNames[1]].Points.AddXY(x, y);
            }

            p3D.Point3D[] q = new p3D.Point3D[4];
            double nfacets = cylinder.Facet_number;
            for (double i = 0; i < nfacets; i++)
            {
                int n = 0;
                double theta1 = i * 2 * Math.PI / nfacets;
                double theta2 = (i + 1) * 2 * Math.PI / nfacets;
                q[n].X = p1.X + cylinder.Radius * Math.Cos(theta1) * r0.X + cylinder.Radius * Math.Sin(theta1) * s0.X;
                q[n].Y = p1.Y + cylinder.Radius * Math.Cos(theta1) * r0.Y + cylinder.Radius * Math.Sin(theta1) * s0.Y;
                q[n].Z = p1.Z + cylinder.Radius * Math.Cos(theta1) * r0.Z + cylinder.Radius * Math.Sin(theta1) * s0.Z;
                n++;
                q[n].X = p2.X + cylinder.Radius * Math.Cos(theta1) * r0.X + cylinder.Radius * Math.Sin(theta1) * s0.X;
                q[n].Y = p2.Y + cylinder.Radius * Math.Cos(theta1) * r0.Y + cylinder.Radius * Math.Sin(theta1) * s0.Y;
                q[n].Z = p2.Z + cylinder.Radius * Math.Cos(theta1) * r0.Z + cylinder.Radius * Math.Sin(theta1) * s0.Z;
                n++;
                q[n].X = p1.X + cylinder.Radius * Math.Cos(theta2) * r0.X + cylinder.Radius * Math.Sin(theta2) * s0.X;
                q[n].Y = p1.Y + cylinder.Radius * Math.Cos(theta2) * r0.Y + cylinder.Radius * Math.Sin(theta2) * s0.Y;
                q[n].Z = p1.Z + cylinder.Radius * Math.Cos(theta2) * r0.Z + cylinder.Radius * Math.Sin(theta2) * s0.Z;
                n++;
                q[n].X = p2.X + cylinder.Radius * Math.Cos(theta2) * r0.X + cylinder.Radius * Math.Sin(theta2) * s0.X;
                q[n].Y = p2.Y + cylinder.Radius * Math.Cos(theta2) * r0.Y + cylinder.Radius * Math.Sin(theta2) * s0.Y;
                q[n].Z = p2.Z + cylinder.Radius * Math.Cos(theta2) * r0.Z + cylinder.Radius * Math.Sin(theta2) * s0.Z;
                int[] points = { 0, 1, 3, 2 };
                for (int j = 0; j < 4; j++)
                {
                    Convert3DTo2D(rot_x, rot_y, rot_z, q[points[j]].X, q[points[j]].Y, q[points[j]].Z, ref x, ref y, ref z);
                    chart.Series[seriesNames[2]].Points.AddXY(x, y);
                }
            }
        }

        private void cylinderPlotterRendered(Shapes.Cylinder cylinder, p3D.Point3D p1, p3D.Point3D p2, double rot_x, double rot_y, double rot_z)
        {
            string[] seriesNames = cylinderSeriesNames(cylinder);
            p3D.Vector3D p1p2 = new p3D.Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            Random rand = new Random();
            p3D.Point3D p_rand = new p3D.Point3D(rand.NextDouble() * 100.0, rand.NextDouble() * 100.0, rand.NextDouble() * 100.0);
            p3D.Vector3D pv0 = new p3D.Vector3D(p_rand.X - p1.X, p_rand.Y - p1.Y, p_rand.Z - p1.Z);
            p3D.Vector3D r0 = p3D.Vector3D.CrossProduct(p1p2, pv0);
            p3D.Vector3D s0 = p3D.Vector3D.CrossProduct(p1p2, r0);
            r0.Normalize();
            s0.Normalize();
            p3D.Point3D[] q = new p3D.Point3D[4];
            double nfacets = cylinder.Facet_number;
            double theta_inc = Math.PI * 2.0 / 360;
            for (double theta = 0; theta < Math.PI * 2.0; theta += theta_inc)
            {
                int n = 0;
                q[n].X = p1.X + cylinder.Radius * Math.Cos(theta) * r0.X + cylinder.Radius * Math.Sin(theta) * s0.X;
                q[n].Y = p1.Y + cylinder.Radius * Math.Cos(theta) * r0.Y + cylinder.Radius * Math.Sin(theta) * s0.Y;
                q[n].Z = p1.Z + cylinder.Radius * Math.Cos(theta) * r0.Z + cylinder.Radius * Math.Sin(theta) * s0.Z;
                n++;
                q[n].X = p2.X + cylinder.Radius * Math.Cos(theta) * r0.X + cylinder.Radius * Math.Sin(theta) * s0.X;
                q[n].Y = p2.Y + cylinder.Radius * Math.Cos(theta) * r0.Y + cylinder.Radius * Math.Sin(theta) * s0.Y;
                q[n].Z = p2.Z + cylinder.Radius * Math.Cos(theta) * r0.Z + cylinder.Radius * Math.Sin(theta) * s0.Z;
                Convert3DTo2D(rot_x, rot_y, rot_z, p2.X, p2.Y, p2.Z, ref x, ref y, ref z);
                chart.Series[seriesNames[2]].Points.AddXY(x, y);
                Convert3DTo2D(rot_x, rot_y, rot_z, q[1].X, q[1].Y, q[1].Z, ref x, ref y, ref z);
                chart.Series[seriesNames[2]].Points.AddXY(x, y);
                Convert3DTo2D(rot_x, rot_y, rot_z, q[0].X, q[0].Y, q[0].Z, ref x, ref y, ref z);
                chart.Series[seriesNames[2]].Points.AddXY(x, y);
                Convert3DTo2D(rot_x, rot_y, rot_z, p1.X, p1.Y, p1.Z, ref x, ref y, ref z);
                chart.Series[seriesNames[2]].Points.AddXY(x, y);
            }
        }

        private string[] cylinderSeriesNames(Shapes.Cylinder cylinder)
        {
            string face0 = String.Format("cylinderFace0_{0}", cylinder.Name);
            string face1 = String.Format("cylinderFace1_{0}", cylinder.Name);
            string sides = String.Format("cylinderSides_{0}", cylinder.Name);
            string[] series = { face0, face1, sides };
            return series;
        }

        public void deleteShape(Shapes.Cylinder cylinder)
        {
            string[] seriesNames = cylinderSeriesNames(cylinder);
            foreach (string name in seriesNames)
            {
                Series series = chart.Series[name];
                chart.Series.Remove(series);
            }
        }

        //Point plotting methods

        public void plotSetup(Shapes.Point point)
        {
            string series = pointSeriesName(point);
            chart.Series.Add(series);
            chart.Series[series].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart.Series[series].Color = System.Drawing.Color.Blue;
        }

        public void plotSetup(Shapes.Point point, System.Drawing.Color color)
        {
            string series = pointSeriesName(point);
            chart.Series.Add(series);
            chart.Series[series].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart.Series[series].Color = color;
        }

        public void Plot(Shapes.Point point, p3D.Point3D center, double rot_x, double rot_y, double rot_z)
        {
            string series = pointSeriesName(point);
            chart.Series[series].Points.Clear();
            pointPlotter(point, rot_x, rot_y, rot_z, center);
            chartSizer(chart);
        }

        private void pointPlotter(Shapes.Point point, double rot_x, double rot_y, double rot_z, p3D.Point3D center)
        {
            string series = pointSeriesName(point);
            Convert3DTo2D(rot_x, rot_y, rot_z, center.X, center.Y, center.Z, ref x, ref y, ref z);
            chart.Series[series].Points.AddXY(x, y);
        }

        private string pointSeriesName(Shapes.Point point)
        {
            string point_series = String.Format("point_{0}", point.Name);
            return point_series;
        }

        public void deleteShape(Shapes.Point point)
        {
            string seriesName = pointSeriesName(point);
            Series series = chart.Series[seriesName];
            chart.Series.Remove(series);
        }

        //Label plotting methods

        public void plotSetup(TextAnnotation label)
        {
            this.chart.Annotations.Add(label);
        }

        public void Plot(TextAnnotation label, string text, p3D.Point3D center, double rot_x, double rot_y, double rot_z)
        {
            label.Text = text;
            Convert3DTo2D(rot_x, rot_y, rot_z, center.X, center.Y, center.Z, ref x, ref y, ref z);
            chart.Annotations[label.Name].AxisX = chart.ChartAreas[0].AxisX;
            chart.Annotations[label.Name].AxisY = chart.ChartAreas[0].AxisY;
            chart.Annotations[label.Name].AnchorX = x;
            chart.Annotations[label.Name].AnchorY = y;
        }

        public void deleteShape(TextAnnotation label)
        {
            this.chart.Annotations.Remove(label);
        }

        //Converts 3D coordinates to 2D

        private void Convert3DTo2D(double rx, double ry, double rz, double px, double py, double pz, ref double screen_x, ref double screen_y, ref double screen_z)
        {
            double rot_x = (rx - 90) * Math.PI / 180.0; //(rx -90) rotates the axes so that x+ is right, y+ is into the screen, and z+ is up
            double rot_y = ry * Math.PI / 180.0;
            double rot_z = rz * Math.PI / 180.0;

            double sx = Math.Sin(rot_x);
            double cx = Math.Cos(rot_x);
            double sy = Math.Sin(rot_y);
            double cy = Math.Cos(rot_y);
            double sz = Math.Sin(rot_z);
            double cz = Math.Cos(rot_z);

            double x, y, z, xy, xz, yx, yz, zx, zy;
            x = px;
            y = py;
            z = pz;

            // rotation around x
            xy = cx * y - sx * z;
            xz = sx * y + cx * z;

            // rotation around y
            yz = cy * xz - sy * x;
            yx = sy * xz + cy * x;

            // rotation around z
            zx = cz * yx - sz * xy;
            zy = sz * yx + cz * xy;

            x = zx; // screen x
            y = zy; // screen y
            z = yz; // depth

            screen_x = x;
            screen_y = y;
            screen_z = z;
        }

        //Sets maximum and minimum values of chart axes

        private void chartSizer(System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            List<DataPoint> points = new List<DataPoint>();

            foreach (var series in chart.Series)
                foreach (var point in series.Points)
                    points.Add(point);
            DataPoint center = new DataPoint(points.Average(p => p.XValue), points.Average(p => p.YValues[0]));
            double range = Math.Max(points.OrderBy(p => p.XValue).Last().XValue, points.OrderBy(p => p.YValues[0]).Last().YValues[0])
                - Math.Max(points.OrderBy(p => p.XValue).First().XValue, points.OrderBy(p => p.YValues[0]).First().YValues[0]);
            double chartSpacing = 1.55; //<=2, at 2 the maxes and mins are the same as the max and min points on the chart 
            chart.ChartAreas[0].AxisX.Maximum = center.XValue + range / chartSpacing;
            chart.ChartAreas[0].AxisY.Maximum = center.YValues[0] + range / chartSpacing;
            chart.ChartAreas[0].AxisX.Minimum = center.XValue - range / chartSpacing;
            chart.ChartAreas[0].AxisY.Minimum = center.YValues[0] - range / chartSpacing;
        }
    }
}
