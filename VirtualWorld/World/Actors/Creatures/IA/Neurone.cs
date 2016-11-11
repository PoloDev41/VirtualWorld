using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld;

namespace VirtualWorld.World.Actors.Creature
{
    public delegate double NerfSensor(Monde m, Individu proprietaire);
    public delegate void NerfMuscleAction(Monde m, Individu proprietaire, double power, float deltaTime);

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
    /// base of kind of intelligence
    /// </summary>
    public class StemCell
    {
        public double Output { get; set; }
        protected double NewOutput { get; set; }

        public virtual void UpdateAsynch(Monde m, Individu proprietaire, float deltaTime)
        {
            proprietaire.PointDeVie--;
        }

        public virtual void UpdateSynch(Monde m, Individu proprietaire, float deltaTime)
        {
            this.Output = this.NewOutput;
        }
    }

    public class Nerf: StemCell
    {
        public NerfSensor Process { get; set; }

        public override void UpdateAsynch(Monde m, Individu proprietaire, float deltaTime)
        {
            base.UpdateAsynch(m, proprietaire, deltaTime);
            this.NewOutput = this.Process(m, proprietaire);
        }
    }

    /// <summary>
    /// class used to emulate a neurone
    /// </summary>
    public class Neurone : StemCell
    {
        private static Random rand = new Random();

        public static double GenerateNewWeight()
        {
            return rand.Next(-20, 21) * rand.NextDouble();
        }

        public double Biais { get; set; }
        public Synapse[] Synapses { get; set; }
        public NerfMuscleAction ActionMuscle { get; set; }

        public override void UpdateAsynch(Monde m, Individu proprietaire, float deltaTime)
        {
            base.UpdateAsynch(m, proprietaire, deltaTime);

            double sum = Biais;
            for (int i = 0; i < this.Synapses.Length; i++)
            {
                sum += proprietaire.Intelligence.Neurones[this.Synapses[i].IndexNeurone].Output * this.Synapses[i].Weight;
            }
            this.NewOutput = SigmoideFct(sum);
        }

        public override void UpdateSynch(Monde m, Individu proprietaire, float deltaTime)
        {
            base.UpdateSynch(m, proprietaire, deltaTime);
            if(this.ActionMuscle != null)
            {
                this.ActionMuscle(m, proprietaire, this.NewOutput, deltaTime);
            }
        }

        public static double SigmoideFct(double x)
        {
            if (x >= 40)
                return 1;
            else if (x < -40)
                return -1;

            return 2d / (1 + Math.Exp(-x)) -1;
        }
    }
}
