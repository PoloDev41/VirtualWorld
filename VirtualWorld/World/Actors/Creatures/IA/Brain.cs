using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Brain Clone()
        {
            Brain clone = new Brain();
            clone.Neurones = new StemCell[this.Neurones.Length];
            for (int i = 0; i < clone.Neurones.Length; i++)
            {
                clone.Neurones[i] = this.Neurones[i].Clone();
            }

            return clone;
        }
    }
}
