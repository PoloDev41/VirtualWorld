using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld.World.Actors.Creatures.IA;

namespace VirtualWorld.World.Actors.Creature.IA
{
    /// <summary>
    /// Brain
    /// </summary>
    public class Brain
    {
        /// <summary>
        /// list of neurones
        /// </summary>
        public StemCell[] Neurones { get; set; }

        /// <summary>
        /// Create an empty brain
        /// </summary>
        public Brain()
        {
        }

        private StemCell CreateRandomStemCell(int nbrStemCell)
        {
            StemCell s = null;
            int r = Monde.rand.Next(0, 101);
            if(r < 80)
            {
                s = new Neurone();
                Neurone n = (Neurone)s;
                if(Monde.rand.Next(0,101) < 25)
                {
                    n.ActionMuscle = NerfMuscleActionList.GetRandomAction();
                }
                n.Biais = Neurone.GenerateNewWeight();
                int nbrSynapse = Math.Max(1, Monde.rand.Next(nbrStemCell+1));
                n.Synapses = new Synapse[nbrSynapse];
                for (int i = 0; i < nbrSynapse; i++)
                {
                    n.Synapses[i] = new Synapse()
                    {
                        IndexNeurone = Monde.rand.Next(nbrStemCell+1),
                        Weight = Neurone.GenerateNewWeight()
                    };
                }
            }
            else
            {
                s = new Nerf()
                {
                    Process = NerfSensorList.GetRandomSensor()
                };
            }

            return s;
        }

        public Brain Clone()
        {
            Brain clone = new Brain();
            List<StemCell> tmp = new List<StemCell>();
            for (int i = 0; i < this.Neurones.Length; i++)
            {
                StemCell s = this.Neurones[i].Clone();
                if(s is Neurone)
                {
                }
                tmp.Add(s);
            }
            if(tmp.Count > 0 && Monde.rand.Next(0, 101) < 2)
            {
                int indexToRemove = Monde.rand.Next(0, tmp.Count);
                tmp.RemoveAt(indexToRemove);
                for (int i = 0; i < tmp.Count; i++)
                {
                    if(tmp[i] is Neurone)
                    {
                        Neurone n = (Neurone)tmp[i];
                        for (int j = 0; j < n.Synapses.Length; j++)
                        {
                            if (n.Synapses[j].IndexNeurone > indexToRemove)
                            {
                                n.Synapses[j].IndexNeurone--;
                            }
                            else
                            {
                                n.Synapses[j].IndexNeurone = -1;
                            }
                        }
                    }
                }
            }
            if (Monde.rand.Next(0,101) < 2)
            {
                StemCell s = CreateRandomStemCell(tmp.Count);
                tmp.Add(s);
            }

            clone.Neurones = tmp.ToArray();

            return clone;
        }
    }
}
