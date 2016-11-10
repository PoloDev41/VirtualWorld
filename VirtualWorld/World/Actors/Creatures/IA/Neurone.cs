using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld;

namespace VirtualWorld.World.Actors.Creature
{
    public class Synapse
    {
        /// <summary>
        /// index of the connected neurone
        /// </summary>
        public int IndexNeurone { get; set; }
        /// <summary>
        /// weight of the synapse
        /// </summary>
        public double Weight { get; set; }
    }

    /// <summary>
    /// class used to emulate a neurone
    /// </summary>
    public class Neurone
    {
        public Synapse[] Synapses { get; set; }

        public double Output { get; set; }
        private double NewOutput { get; set; }

        public void UpdateAsynch(Monde m, Individu proprietaire, float deltaTime)
        {
            double sum = 0;
            for (int i = 0; i < this.Synapses.Length; i++)
            {
                sum += proprietaire.Intelligence.Neurones[this.Synapses[i].IndexNeurone].Output * this.Synapses[i].Weight;
            }
            this.NewOutput = SigmoideFct(sum);
        }

        public void UpdateSynch(Monde m, Individu proprietaire, float deltaTime)
        {
            this.Output = this.NewOutput;
        }

        public static double SigmoideFct(double x)
        {
            if (x >= 40)
                return 1;
            else if (x < -40)
                return -1;

            return 1d / (1 + Math.Exp(-x));
        }
    }
}
