using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld
{
    public class Plante : EtreVivant
    {
        public static readonly int TAILLE_IMAGE_PLANTE_PX = 32;

        public static Texture2D Plante1 { get; set; }
        public static Texture2D Plante_Froide { get; set; }
        public static Texture2D Plante_Chaude { get; set; }

        public List<ParcelleTerrain> TerreProche { get; set; }

        /// <summary>
        /// More is this variable more the tree grow quickly
        /// </summary>
        public float BienEtre { get; set; }

        public float TemperatureIdeal { get; set; }
        public float AltitudeIdeal { get; set; }

        private int PommeAAjouter { get; set; }

        public float TempsGraine { get; set; }

        public Plante(Vector2 pos, Monde m):
            base(pos, m)
        {
            Init(m);

            this.ComputeRender();
        }

        /// <summary>
        /// cree une plante à partir d'un parent
        /// </summary>
        /// <param name="plante">plante parent</param>
        /// <param name="pos">position</param>
        /// <param name="m">monde</param>
        public Plante(Plante plante, Vector2 pos, Monde m) :
            base(plante, pos)
        {
            Init(m);

            this.TempsGraine = plante.TempsGraine;
            this.AltitudeIdeal = plante.AltitudeIdeal;
            this.ApplyGenetic();

            this.ComputeRender();
        }

        private void Init(Monde m)
        {
            this.TerreProche = new List<ParcelleTerrain>();
            this.FactorAgrandissement = 0.5f;
            this.PositionImage = new Vector2(this.Position.X - TAILLE_IMAGE_PLANTE_PX * this.FactorAgrandissement / 2, this.Position.Y - TAILLE_IMAGE_PLANTE_PX * this.FactorAgrandissement);
            TemperatureIdeal = rand.Next((int)Math.Round(m.TemperatureMin),
                                            (int)Math.Round(m.TemperatureMax)) + (float)rand.NextDouble();
            this.RefParcelle = ParcelleTerrain.TransformPixelToParcelle(m, this.Position);
            this.BienEtre = 0;
        }

        private void ComputeRender()
        {
            if (this.TemperatureIdeal <= -10)
                this.PictureUsed = Plante.Plante_Froide;
            else if (this.TemperatureIdeal >= 20)
                this.PictureUsed = Plante.Plante_Chaude;
            else
                this.PictureUsed = Plante.Plante1;
        }

        protected override void ApplyGenetic()
        {
            base.ApplyGenetic();

            this.TemperatureIdeal += (float)(rand.Next(-2,3) * rand.NextDouble());
            this.AltitudeIdeal += (float)(rand.Next(-2, 3) * rand.NextDouble());

            int t_ = (int)Math.Max((int)Math.Round(this.TempsGraine * 1.1f),
                            this.TempsGraine+1);
            int _t = (int)Math.Min((int)Math.Round(this.TempsGraine * .9f),
                            this.TempsGraine - 1);
            this.TempsGraine = (float)(rand.Next(_t, t_) + rand.NextDouble());
        }

        private void RefreshCroissance(float deltaTime)
        {
            this.BienEtre = Math.Max(0, 
                this.BienEtre + 
                (5 - (
                Math.Abs(TemperatureIdeal - this.RefParcelle.Temperature) +
                Math.Abs(AltitudeIdeal - this.RefParcelle.Altitude)
                )) * deltaTime);
            if (this.BienEtre > 50)
                this.BienEtre = 50;
        }

        public override void UpdateAsynch(float deltaTime, Monde monde)
        {
            //TODO: check how tree can have an growing factor upper than 16
            float previous = this.PointDeVie;
            base.UpdateAsynch(deltaTime, monde);

            RefreshCroissance(deltaTime);

            float wish = BienEtre * FactorAgrandissement * deltaTime;
            float engrai = RefParcelle.RemoveEngrais(wish);
            this.PointDeVie += engrai*.60f;

            float f = this.PointDeVie - previous;

            while (this.PointDeVie > this.PointDeVieDemarrage * 4)
            {
                this.PommeAAjouter+=2;
                this.PointDeVie -= this.PointDeVieDemarrage * 1.10f;
            }

            if (f > 0 )
            {
                float prevFactor = FactorAgrandissement;
                this.FactorAgrandissement += f / (this.PointDeVie * this.FactorAgrandissement);

                if (this.FactorAgrandissement > 16)
                    this.FactorAgrandissement = 16;

                if((int)prevFactor < (int)this.FactorAgrandissement)
                {
                    ParcelleTerrain p = null;
                    do
                    {
                        p = GetParcelleProche(monde, (int)FactorAgrandissement);
                    } while (this.TerreProche.IndexOf(p) != -1);
                    this.TerreProche.Add(p);
                }
                this.PositionImage = new Vector2(this.Position.X - TAILLE_IMAGE_PLANTE_PX * this.FactorAgrandissement / 2, this.Position.Y - TAILLE_IMAGE_PLANTE_PX * this.FactorAgrandissement);
            }

            for (int i = 0; i < this.TerreProche.Count; i++)
            {
                this.TerreProche[i].RemoveEngrais(wish*1.75f);
            }
        }

        public override void UpdateSynch(float deltaTime, Monde monde)
        {
            float x, y;

            while (this.PommeAAjouter > 0)
            {
                x = this.Position.X + EtreVivant.rand.Next(-TAILLE_IMAGE_PLANTE_PX, TAILLE_IMAGE_PLANTE_PX + 1) * this.FactorAgrandissement *2;
                y = this.Position.Y + EtreVivant.rand.Next(-TAILLE_IMAGE_PLANTE_PX, TAILLE_IMAGE_PLANTE_PX + 1) * this.FactorAgrandissement *2;

                if (x >= 0 && x < monde.TaillePx.X &&
                    y >= 0 && y < monde.TaillePx.Y)
                    monde.Fruits.Add(new Fruit(new Vector2(x, y), this, monde));

                this.PommeAAjouter--;
            }
        }
    }
}
