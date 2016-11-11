using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld.World.Actors
{
    public class Egg : EtreVivant
    {
        public static readonly int TAILLE_IMAGE_EGG_PX = 25;
        public static Texture2D EggGround { get; set; }

        public Individu RefIndividu { get; set; }

        public float TempsEgg { get; set; }

        public Egg(Individu ev, Vector2 pos, Monde m):
            base(pos, m)
        {
            this.TempsEgg = ev.TempsEgg;
            this.FactorAgrandissement = 1f;
            this.RefIndividu = ev;
            this.PositionImage = new Vector2(this.Position.X - TAILLE_IMAGE_EGG_PX * this.FactorAgrandissement / 2,
                                            this.Position.Y - TAILLE_IMAGE_EGG_PX * this.FactorAgrandissement / 2);
        }

        public Egg(Individu ev, Monde m):
            base(ev.Position, m)
        {
            this.TempsEgg = ev.TempsEgg;
            this.FactorAgrandissement = 1f;
            this.RefIndividu = ev.Clone(m);
            this.PositionImage = new Vector2(this.Position.X - TAILLE_IMAGE_EGG_PX * this.FactorAgrandissement / 2,
                                            this.Position.Y - TAILLE_IMAGE_EGG_PX * this.FactorAgrandissement / 2);
        }

        public override void UpdateAsynch(float deltaTime, Monde monde)
        {
            this.TempsEgg -= deltaTime;
            if (this.TempsEgg <= 0)
            {
                this.Mort = true;
            }
        }
    }
}
