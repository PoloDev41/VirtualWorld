using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld
{
    public class Graine : EtreVivant
    {
        public static readonly int TAILLE_IMAGE_GRAINE_PX = 18;

        public static Texture2D GraineSol { get; set; }

        public Plante RefPlante { get; set; }

        public float TempsGraine { get; set; }

        public Graine(Plante ev, Vector2 pos, Monde m):
            base(pos, m)
        {
            this.TempsGraine = ev.TempsGraine;
            this.FactorAgrandissement = 1f;
            this.RefPlante = ev;
            this.PositionImage = new Vector2(this.Position.X - TAILLE_IMAGE_GRAINE_PX * this.FactorAgrandissement / 2, 
                                            this.Position.Y - TAILLE_IMAGE_GRAINE_PX * this.FactorAgrandissement / 2);
        }

        public override void UpdateAsynch(float deltaTime, Monde monde)
        {
            this.TempsGraine -= deltaTime;
            if (this.TempsGraine <= 0)
            {
                this.Mort = true;
            }
        }

    }
}
