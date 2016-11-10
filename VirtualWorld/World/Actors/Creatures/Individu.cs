using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld.World.Actors.Creature.IA;

namespace VirtualWorld
{
    public class Individu: EtreVivant
    {
        public static readonly int TAILLE_IMAGE_INDIVIDU_PX_X = 12;
        public static readonly int TAILLE_IMAGE_INDIVIDU_PX_Y = 13;

        /// <summary>
        /// texture de l'individu
        /// </summary>
        public static Texture2D IndividuTexture { get; set; }

        /// <summary>
        /// intelligence of the individu
        /// </summary>
        public Brain Intelligence { get; set; }

        public Individu(Vector2 pos, Monde m)
            :base(pos, m)
        {
            Init(m);
            ComputeRender();
        }

        public override void UpdateAsynch(float deltaTime, Monde monde)
        {
            Parallel.ForEach(this.Intelligence.Neurones, x => x.UpdateAsynch(monde, this, deltaTime));

            base.UpdateAsynch(deltaTime, monde);
        }

        public override void UpdateSynch(float deltaTime, Monde monde)
        {
            Parallel.ForEach(this.Intelligence.Neurones, x => x.UpdateSynch(monde, this, deltaTime));

            base.UpdateSynch(deltaTime, monde);
        }

        private void Init(Monde m)
        {
            this.FactorAgrandissement = 1f;
            this.PositionImage = new Vector2(this.Position.X - TAILLE_IMAGE_INDIVIDU_PX_X * this.FactorAgrandissement / 2,
                                                this.Position.Y - TAILLE_IMAGE_INDIVIDU_PX_Y * this.FactorAgrandissement);
            /*TemperatureIdeal = rand.Next((int)Math.Round(m.TemperatureMin),
                                            (int)Math.Round(m.TemperatureMax)) + (float)rand.NextDouble();*/
            this.RefParcelle = m.Parcelles[(int)this.Position.X / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX][(int)this.Position.Y / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX];
            this.Intelligence = new Brain();
        }

        private void ComputeRender()
        {
            this.PictureUsed = Individu.IndividuTexture;
        }
    }
}