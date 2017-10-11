using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualWorld.World.Actors.Creature;
using VirtualWorld.World.Actors.Creature.IA;

namespace WinForm_DataView
{
    public partial class Form1 : Form
    {
        private const int square_neurone_size = 50;
        Bitmap DrawArea;

        public Form1()
        {
            InitializeComponent();
            DrawArea = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.pictureBox1.Image = DrawArea;
        }

        private Brain _brain;


        public void ResetChart()
        {
            foreach (var item in this.chart1.Series)
            {
                item.Points.Clear();
            }
        }

        public void AddSample(int nbrIndividu, int nbrEggs)
        {
            this.chart1.Series["Individus"].Points.AddY(nbrIndividu);
            this.chart1.Series["Eggs"].Points.AddY(nbrEggs);
        }

        public void DrawBrain(Brain b)
        {
            _brain = b;
            Graphics g = Graphics.FromImage(DrawArea);
            g.Clear(Color.White);
            Font drawFont = new Font("Arial", 8);
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            int X = 50;
            int Y = 10;
            List<Point> coordNeurone = new List<Point>();
            foreach (var item in _brain.Neurones)
            {
                RectangleF r = new RectangleF(X, Y, square_neurone_size, square_neurone_size);
                coordNeurone.Add(new Point(X, Y));
                g.FillRectangle(Brushes.Red,r);
                if(item is Nerf)
                {
                    Nerf n = (Nerf)item;
                    SizeF s = g.MeasureString(n.Process.Method.Name, drawFont);
                    g.DrawString(n.Process.Method.Name, drawFont, Brushes.Black, new Point((int)(X - s.Width /2 + square_neurone_size/2), Y + square_neurone_size/2));
                }
                else if(item is Neurone)
                {
                    Neurone n = (Neurone)item;
                    if(n.ActionMuscle != null)
                    {
                        SizeF s = g.MeasureString(n.ActionMuscle.Method.Name, drawFont);
                        g.DrawString(n.ActionMuscle.Method.Name, drawFont, Brushes.Black, new Point((int)(X - s.Width / 2 + square_neurone_size / 2), Y + square_neurone_size / 2));
                    }
                }
                Y += 100;
                if (Y >= 300)
                {
                    X += 100;
                    Y = 0;
                }
            }

            for(int i = 0; i < _brain.Neurones.Length; i++)
            {
                if (_brain.Neurones[i] is Neurone)
                {
                    Neurone n = (Neurone)_brain.Neurones[i];
                    Point p_left = new Point(coordNeurone[i].X, coordNeurone[i].Y + square_neurone_size / 2);
                    foreach (var synapse in n.Synapses)
                    {
                        g.DrawLine(Pens.Black, p_left,
                                    new Point(coordNeurone[synapse.IndexNeurone].X + square_neurone_size, coordNeurone[synapse.IndexNeurone].Y + square_neurone_size / 2));
                    }
                }
            }

            this.pictureBox1.Image = DrawArea;
            g.Dispose();
        }
    }
}
