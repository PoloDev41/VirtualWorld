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
    }
}
