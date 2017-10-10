using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld
{
    public class Fruit : EtreVivant
    {
        public static readonly int TAILLE_IMAGE_POMME_LARGEUR_PX = 21;
        public static readonly int TAILLE_IMAGE_POMME_HAUTEUR_PX = 25;

        public Plante Plante { get; set; }

        public static Texture2D Pomme { get; set; }

        public float LuckGraine { get; set; } = 75;

        /// <summary>
        /// cree un nouveau fruit
        /// </summary>
        /// <param name="pos">position</param>
        public Fruit(Vector2 pos, Monde m):base(pos, m)
        {
            this.FactorAgrandissement = 1f;
            this.PointDeVie = 200;
            this.PositionImage = new Vector2(this.Position.X - TAILLE_IMAGE_POMME_LARGEUR_PX * this.FactorAgrandissement / 2,
                this.Position.Y - TAILLE_IMAGE_POMME_HAUTEUR_PX * this.FactorAgrandissement / 2);
            ParcelleTerrain.TransformPixelToParcelle(m, this.Position).AddFruit(this);
        }

        public Fruit(Vector2 pos, Plante plante, Monde m):
            base(pos, m)
        {
            this.FactorAgrandissement = 1f;
            float x = this.Position.X - TAILLE_IMAGE_POMME_LARGEUR_PX * this.FactorAgrandissement / 2;
            float y = this.Position.Y - TAILLE_IMAGE_POMME_HAUTEUR_PX * this.FactorAgrandissement / 2;
            this.PositionImage = new Vector2(x,y);
            this.PointDeVie = this.PointDeVieDemarrage;
            this.Plante = new Plante(plante, pos, m);
            ParcelleTerrain.TransformPixelToParcelle(m, this.Position).AddFruit(this);
        }

    }
}
