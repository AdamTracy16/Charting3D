using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using p3D = System.Windows.Media.Media3D;
using System.Windows.Forms.DataVisualization.Charting;
using Charting3D;

namespace TestCharting3D
{
    public partial class TestCharting3D : Form
    {
        List<Shapes.BoundingBox> boxes = new List<Shapes.BoundingBox>();
        List<p3D.Point3D> box_centers = new List<p3D.Point3D>();
        List<bool> box_rendered = new List<bool>();
        List<Shapes.Cylinder> cylinders = new List<Shapes.Cylinder>();
        List<p3D.Point3D> cylinder_centers1 = new List<p3D.Point3D>();
        List<p3D.Point3D> cylinder_centers2 = new List<p3D.Point3D>();
        List<bool> cylinder_rendered = new List<bool>();
        List<Shapes.Point> points = new List<Shapes.Point>();
        List<p3D.Point3D> point_centers = new List<p3D.Point3D>();
        List<TextAnnotation> labels = new List<TextAnnotation>();
        List<string> label_texts = new List<string>();
        List<p3D.Point3D> label_centers = new List<p3D.Point3D>();
        List<string> label_names = new List<string>();
        Random rnd = new Random();
        Charting3D.ucCharting3D m_ucCharting3D;

        public TestCharting3D()
        {
            InitializeComponent();
        }

        private void TestCharting3D_Load(object sender, EventArgs e)
        {
            this.tcChartingTabs.SelectedIndex = 0;
            this.nudX_rot.Value = 20;
            this.nudY_rot.Value = 20;
            this.nudZ_rot.Value = 10;
            m_ucCharting3D = new ucCharting3D();
            m_ucCharting3D.Dock = DockStyle.Fill;
            scCharting.Panel1.Controls.Add(m_ucCharting3D);
        }

        private void plotShapes()
        {
            for (int i = 0; i < boxes.Count; i++)
                m_ucCharting3D.Plot(boxes[i], box_centers[i], (double)nudX_rot.Value, (double)nudY_rot.Value, (double)nudZ_rot.Value, box_rendered[i]);
            for (int i = 0; i < cylinders.Count; i++)
                m_ucCharting3D.Plot(cylinders[i], cylinder_centers1[i], cylinder_centers2[i], (double)nudX_rot.Value, (double)nudY_rot.Value, (double)nudZ_rot.Value, cylinder_rendered[i]);
            for (int i = 0; i < points.Count; i++)
                m_ucCharting3D.Plot(points[i], point_centers[i], (double)nudX_rot.Value, (double)nudY_rot.Value, (double)nudZ_rot.Value);
            for (int i = 0; i < labels.Count; i++)
                m_ucCharting3D.Plot(labels[i], label_texts[i], label_centers[i], (double)nudX_rot.Value, (double)nudY_rot.Value, (double)nudZ_rot.Value);
        }

        private void nudX_rot_ValueChanged(object sender, EventArgs e)
        {
            controlsEnabled(false);
            Task.Run(() =>
            {
                BeginInvoke(new Action(() =>
                {
                    plotShapes();
                    controlsEnabled(true);
                }));
            });
        }

        private void nudY_rot_ValueChanged(object sender, EventArgs e)
        {
            controlsEnabled(false);
            Task.Run(() =>
            {
                BeginInvoke(new Action(() =>
                {
                    plotShapes();
                    controlsEnabled(true);
                }));
            });
        }

        private void nudZ_rot_ValueChanged(object sender, EventArgs e)
        {
            controlsEnabled(false);
            Task.Run(() =>
            {
                BeginInvoke(new Action(() =>
                {
                    plotShapes();
                    controlsEnabled(true);
                }));
            });
        }

        private void controlsEnabled(bool enabled)
        {
            this.nudX_rot.Enabled = enabled;
            this.nudY_rot.Enabled = enabled;
            this.nudZ_rot.Enabled = enabled;
            this.cbShape.Enabled = enabled;
            this.cbAddOrDelete.Enabled = enabled;
            this.txtBoxName.Enabled = enabled;
            this.nudBoxWidth.Enabled = enabled;
            this.nudBoxHeight.Enabled = enabled;
            this.nudBoxLength.Enabled = enabled;
            this.nudBoxCenterX.Enabled = enabled;
            this.nudBoxCenterY.Enabled = enabled;
            this.nudBoxCenterZ.Enabled = enabled;
            this.txtCylinderName.Enabled = enabled;
            this.nudCylinderRadius.Enabled = enabled;
            this.nudCylinderFacetNumber.Enabled = enabled;
            this.nudCylinderP1X.Enabled = enabled;
            this.nudCylinderP1Y.Enabled = enabled;
            this.nudCylinderP1Z.Enabled = enabled;
            this.nudCylinderP2X.Enabled = enabled;
            this.nudCylinderP2Y.Enabled = enabled;
            this.nudCylinderP2Z.Enabled = enabled;
            this.txtPointName.Enabled = enabled;
            this.nudPointX.Enabled = enabled;
            this.nudPointY.Enabled = enabled;
            this.nudPointZ.Enabled = enabled;
            this.txtLabelText.Enabled = enabled;
            this.nudLabelX.Enabled = enabled;
            this.nudLabelY.Enabled = enabled;
            this.nudLabelZ.Enabled = enabled;
            this.txtDeleteName.Enabled = enabled;
            this.cbBoxRendered.Enabled = enabled;
            this.cbCylinderRendered.Enabled = enabled;
            if (this.m_ucCharting3D != null) { this.m_ucCharting3D.Enabled = enabled; }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (this.cbShape.SelectedIndex == 0 && this.cbAddOrDelete.SelectedIndex == 0)
            {
                double d_BoxWidth = Convert.ToDouble(this.nudBoxWidth.Value);
                double d_BoxHeight = Convert.ToDouble(this.nudBoxHeight.Value);
                double d_BoxLength = Convert.ToDouble(this.nudBoxLength.Value);
                double d_BoxCenterX = Convert.ToDouble(this.nudBoxCenterX.Value);
                double d_BoxCenterY = Convert.ToDouble(this.nudBoxCenterY.Value);
                double d_BoxCenterZ = Convert.ToDouble(this.nudBoxCenterZ.Value);
                bool rendered = (this.cbBoxRendered.Checked == true) ? true : false;
                Shapes.BoundingBox b = new Shapes.BoundingBox(this.txtBoxName.Text, d_BoxWidth, d_BoxHeight, d_BoxLength);
                boxes.Add(b);
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                m_ucCharting3D.plotSetup(b, randomColor);
                p3D.Point3D c = new p3D.Point3D(d_BoxCenterX, d_BoxCenterY, d_BoxCenterZ);
                box_centers.Add(c);
                box_rendered.Add(rendered);
                this.txtBoxName.Text = "";
                this.nudBoxWidth.Value = 0;
                this.nudBoxHeight.Value = 0;
                this.nudBoxLength.Value = 0;
                this.nudBoxCenterX.Value = 0;
                this.nudBoxCenterY.Value = 0;
                this.nudBoxCenterZ.Value = 0;
                this.cbBoxRendered.Checked = false;
                controlsEnabled(false);
                Task.Run(() =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        plotShapes();
                        controlsEnabled(true);
                    }));
                });
            }

            else if (this.cbShape.SelectedIndex == 1 && this.cbAddOrDelete.SelectedIndex == 0)
            {
                double d_CylinderRadius = Convert.ToDouble(this.nudCylinderRadius.Value);
                double d_CylinderFacetNumber = Convert.ToDouble(this.nudCylinderFacetNumber.Value);
                double d_CylinderP1X = Convert.ToDouble(this.nudCylinderP1X.Value);
                double d_CylinderP1Y = Convert.ToDouble(this.nudCylinderP1Y.Value);
                double d_CylinderP1Z = Convert.ToDouble(this.nudCylinderP1Z.Value);
                double d_CylinderP2X = Convert.ToDouble(this.nudCylinderP2X.Value);
                double d_CylinderP2Y = Convert.ToDouble(this.nudCylinderP2Y.Value);
                double d_CylinderP2Z = Convert.ToDouble(this.nudCylinderP2Z.Value);
                bool rendered = (this.cbCylinderRendered.Checked == true) ? true : false;
                Shapes.Cylinder c = new Shapes.Cylinder(this.txtCylinderName.Text, d_CylinderRadius, d_CylinderFacetNumber, 1);
                cylinders.Add(c);
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                m_ucCharting3D.plotSetup(c, randomColor);
                p3D.Point3D p1 = new p3D.Point3D(d_CylinderP1X, d_CylinderP1Y, d_CylinderP1Z);
                cylinder_centers1.Add(p1);
                p3D.Point3D p2 = new p3D.Point3D(d_CylinderP2X, d_CylinderP2Y, d_CylinderP2Z);
                cylinder_centers2.Add(p2);
                cylinder_rendered.Add(rendered);
                this.txtCylinderName.Text = "";
                this.nudCylinderRadius.Value = 0;
                this.nudCylinderFacetNumber.Value = 0;
                this.nudCylinderP1X.Value = 0;
                this.nudCylinderP1Y.Value = 0;
                this.nudCylinderP1Z.Value = 0;
                this.nudCylinderP2X.Value = 0;
                this.nudCylinderP2Y.Value = 0;
                this.nudCylinderP2Z.Value = 0;
                this.cbCylinderRendered.Checked = false;
                controlsEnabled(false);
                Task.Run(() =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        plotShapes();
                        controlsEnabled(true);
                    }));
                });
            }

            else if (this.cbShape.SelectedIndex == 2 && this.cbAddOrDelete.SelectedIndex == 0)
            {
                double d_PointX = Convert.ToDouble(this.nudPointX.Value);
                double d_PointY = Convert.ToDouble(this.nudPointY.Value);
                double d_PointZ = Convert.ToDouble(this.nudPointZ.Value);
                Shapes.Point p = new Shapes.Point(this.txtPointName.Text);
                points.Add(p);
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                m_ucCharting3D.plotSetup(p, randomColor);
                p3D.Point3D c = new p3D.Point3D(d_PointX, d_PointY, d_PointZ);
                point_centers.Add(c);
                this.txtPointName.Text = "";
                this.nudPointX.Value = 0;
                this.nudPointY.Value = 0;
                this.nudPointZ.Value = 0;
                controlsEnabled(false);
                Task.Run(() =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        plotShapes();
                        controlsEnabled(true);
                    }));
                });
            }

            else if (this.cbShape.SelectedIndex == 3 && this.cbAddOrDelete.SelectedIndex == 0)
            {
                double d_LabelX = Convert.ToDouble(this.nudLabelX.Value);
                double d_LabelY = Convert.ToDouble(this.nudLabelY.Value);
                double d_LabelZ = Convert.ToDouble(this.nudLabelZ.Value);
                TextAnnotation l = new TextAnnotation();
                labels.Add(l);
                m_ucCharting3D.plotSetup(l);
                label_texts.Add(this.txtLabelText.Text);
                p3D.Point3D c = new p3D.Point3D(d_LabelX, d_LabelY, d_LabelZ);
                label_centers.Add(c);
                label_names.Add(this.txtLabelName.Text);
                this.txtLabelText.Text = "";
                this.nudLabelX.Value = 0;
                this.nudLabelY.Value = 0;
                this.nudLabelZ.Value = 0;
                controlsEnabled(false);
                Task.Run(() =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        plotShapes();
                        controlsEnabled(true);
                    }));
                });
            }

            else if (this.cbShape.SelectedIndex == 0 && this.cbAddOrDelete.SelectedIndex == 1)
            {
                List<int> indexes = new List<int>();

                foreach (Shapes.BoundingBox box in boxes)
                {
                    if (box.Name == this.txtDeleteName.Text)
                        indexes.Add(boxes.IndexOf(box));
                }

                foreach (int index in indexes)
                {
                    m_ucCharting3D.deleteShape(boxes[index]);
                    boxes.Remove(boxes[index]);
                    box_centers.Remove(box_centers[index]);
                }
                this.txtDeleteName.Text = "";
            }

            else if (this.cbShape.SelectedIndex == 1 && this.cbAddOrDelete.SelectedIndex == 1)
            {
                List<int> indexes = new List<int>();
                foreach (Shapes.Cylinder cylinder in cylinders)
                {
                    if (cylinder.Name == this.txtDeleteName.Text)
                        indexes.Add(cylinders.IndexOf(cylinder));
                }

                foreach (int index in indexes)
                {
                    m_ucCharting3D.deleteShape(cylinders[index]);
                    cylinders.Remove(cylinders[index]);
                    cylinder_centers1.Remove(cylinder_centers1[index]);
                    cylinder_centers2.Remove(cylinder_centers2[index]);
                }
                this.txtDeleteName.Text = "";
            }

            else if (this.cbShape.SelectedIndex == 2 && this.cbAddOrDelete.SelectedIndex == 1)
            {
                List<int> indexes = new List<int>();
                foreach (Shapes.Point point in points)
                {
                    if (point.Name == this.txtDeleteName.Text)
                        indexes.Add(points.IndexOf(point));
                }
                foreach (int index in indexes)
                {
                    m_ucCharting3D.deleteShape(points[index]);
                    points.Remove(points[index]);
                    point_centers.Remove(point_centers[index]);
                }
                this.txtDeleteName.Text = "";
            }

            else if (this.cbShape.SelectedIndex == 3 && this.cbAddOrDelete.SelectedIndex == 1)
            {
                List<int> indexes = new List<int>();
                foreach (string label in label_names)
                {
                    if (label == this.txtDeleteName.Text)
                        indexes.Add(label_names.IndexOf(label));
                }
                foreach (int index in indexes)
                {
                    m_ucCharting3D.deleteShape(labels[index]);
                    labels.Remove(labels[index]);
                    label_texts.Remove(label_texts[index]);
                    label_centers.Remove(label_centers[index]);
                    label_names.Remove(label_names[index]);
                }
                this.txtDeleteName.Text = "";
            }

            else
                this.tcChartingTabs.SelectedIndex = 0;
        }

        private void cbShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAddOrDelete.SelectedIndex == 0)
            {
                this.tcChartingTabs.SelectedIndex = this.cbShape.SelectedIndex + 1;
            }

            else if (cbAddOrDelete.SelectedIndex == 1)
            {
                this.tcChartingTabs.SelectedIndex = 5;
            }

            else
            {
                this.tcChartingTabs.SelectedIndex = 0;
            }
        }

        private void cbAddOrDelete_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAddOrDelete.SelectedIndex == 0)
            {
                this.tcChartingTabs.SelectedIndex = this.cbShape.SelectedIndex + 1;
            }

            else if (cbAddOrDelete.SelectedIndex == 1)
            {
                this.tcChartingTabs.SelectedIndex = 5;
            }

            else
            {
                this.tcChartingTabs.SelectedIndex = 0;
            }
        }
    }
}
