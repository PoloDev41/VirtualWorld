using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld
{
    public abstract class EtreVivant
    {
        public static Random rand = new Random();

        /// <summary>
        /// Picture used to draw
        /// </summary>
        public Texture2D PictureUsed { get; set; }

        /// <summary>
        /// position de l'etre vivant
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// point de vie de l'etre vivant
        /// </summary>
        public float PointDeVie { get; set; }

        /// <summary>
        /// if true: l'etre vivant doit être supprimé
        /// </summary>
        public bool Mort { get; set; }

        /// <summary>
        /// position de l'image
        /// </summary>
        public Vector2 PositionImage { get; set; }

        /// <summary>
        /// facteur de taille de l'etre vivant
        /// </summary>
        public float FactorAgrandissement { get; set; }

        /// <summary>
        /// point de vie au demarrage
        /// </summary>
        public float PointDeVieDemarrage { get; set; }

        public ParcelleTerrain RefParcelle { get; set; }

        /// <summary>
        /// cree une nouvelle base pour un etre vivant
        /// </summary>
        /// <param name="position">position de letre vivant</param>
        public EtreVivant(Vector2 position, Monde m)
        {
            this.RefParcelle = m.Parcelles[(int)this.Position.X / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX][(int)this.Position.Y / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX];
            this.PointDeVieDemarrage = 200;
            this.PointDeVie = this.PointDeVieDemarrage;
            this.Position = position;
            this.Mort = false;
        }

        /// <summary>
        /// cree une nouvelle base pour un etre vivant
        /// </summary>
        /// <param name="ev">etre vivant parent</param>
        /// <param name="pos">position de l'etre vivant</param>
        public EtreVivant(EtreVivant ev, Vector2 pos)
        {
            this.PointDeVieDemarrage = ev.PointDeVieDemarrage;
            this.PointDeVie = this.PointDeVieDemarrage;
            this.Position = pos;
            this.FactorAgrandissement = 0.5f;
            this.Mort = false;
        }

        protected ParcelleTerrain GetParcelleProche(Monde monde, int taille = 1)
        {
            List<ParcelleTerrain> pt = new List<ParcelleTerrain>();
            int indexx = (int)this.Position.X / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX;
            int indexy = (int)this.Position.Y / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX;

            for (int i = indexx - taille; i <= indexx + taille; i++)
            {
                for (int j = indexy - taille; j <= indexy + taille; j++)
                {
                    if (i > -1 && i < monde.Parcelles.Length &&
                        j > -1 && j < monde.Parcelles[0].Length &&
                        i != indexx && j != indexy)
                    {
                        pt.Add(monde.Parcelles[i][j]);
                    }
                }
            }

            return pt[rand.Next(pt.Count)];
        }

        protected virtual void ApplyGenetic()
        {
            int t = (int)(this.PointDeVieDemarrage * 0.1f);
            this.PointDeVieDemarrage += (float)(rand.Next(-t, t + 1) * rand.NextDouble());
        }

        public virtual float TakeLife(float factor)
        {
            float take = Math.Min(this.PointDeVie, this.PointDeVie * factor);
            this.PointDeVie -= take;
            return take;
        }

        public virtual void UpdateSynch(float deltaTime, Monde monde) { }

        public virtual void UpdateAsynch(float deltaTime, Monde monde)
        {
            this.PointDeVie -= (5 * FactorAgrandissement * deltaTime);
            if(this.PointDeVie <= 0)
            {
                this.Mort = true;
            }
        }
    }
}
