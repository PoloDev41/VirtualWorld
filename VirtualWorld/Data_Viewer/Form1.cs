using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm_DataView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

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
    }
}
