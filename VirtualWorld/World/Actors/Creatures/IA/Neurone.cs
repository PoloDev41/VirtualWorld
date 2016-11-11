using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld;
using VirtualWorld.World.Actors.Creatures.IA;

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

        internal Synapse Clone()
        {
            Synapse clone = new Synapse();
            clone.IndexNeurone = this.IndexNeurone;
            clone.Weight = Weight + StemCell.rand.NextDouble()*2-1;
            return clone;
        }
    }

    /// <summary>
    /// base of kind of intelligence
    /// </summary>
    public class StemCell
    {
        public static Random rand = new Random();
        public double Output { get; set; }
        protected double NewOutput { get; set; }

        public virtual void UpdateAsynch(Monde m, Individu proprietaire, float deltaTime)
        {
            proprietaire.PointDeVie -= deltaTime;
        }

        public virtual void UpdateSynch(Monde m, Individu proprietaire, float deltaTime)
        {
            this.Output = this.NewOutput;
        }

        public virtual StemCell Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class Nerf: StemCell
    {
        public NerfSensor Process { get; set; }

        public override void UpdateAsynch(Monde m, Individu proprietaire, float deltaTime)
        {
            base.UpdateAsynch(m, proprietaire, deltaTime);
            proprietaire.PointDeVie -= deltaTime; //it's volontary, a nerf sub double
            this.NewOutput = this.Process(m, proprietaire);
        }

        public override StemCell Clone()
        {
            Nerf clone = new Nerf();
            clone.Process = this.Process;
            return clone;
        }
    }

    /// <summary>
    /// class used to emulate a neurone
    /// </summary>
    public class Neurone : StemCell
    {

        public static double GenerateNewWeight()
        {
            return rand.Next(-20, 21) * rand.NextDouble();
        }

        public double Biais { get; set; }
        public Synapse[] Synapses { get; set; }
        public NerfMuscleAction ActionMuscle { get; set; }

        public override StemCell Clone()
        {
            Neurone clone = new Neurone();
            clone.Biais = this.Biais + StemCell.rand.NextDouble() * 2 - 1;
            if (rand.Next(0, 101) > 1)
                clone.ActionMuscle = this.ActionMuscle;
            else
                clone.ActionMuscle = NerfMuscleActionList.GetRandomAction();
            clone.Synapses = new Synapse[this.Synapses.Length];
            for (int i = 0; i < clone.Synapses.Length; i++)
            {
                Synapse s = this.Synapses[i].Clone();
                clone.Synapses[i] = s;
            }
            return clone;
        }

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
