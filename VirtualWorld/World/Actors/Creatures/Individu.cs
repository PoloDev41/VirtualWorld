using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld.World.Actors.Creature;
using VirtualWorld.World.Actors.Creature.IA;
using VirtualWorld.World.Actors.Creatures.IA;

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
        /// angle of the individu
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// intelligence of the individu
        /// </summary>
        public Brain Intelligence { get; set; }

        public float SeuilEgg { get; set; }

        public float TempsEgg { get; internal set; }

        #region Optimize

        public Fruit NearestFruit_Opti { get; set; }
        public int LastTimeToComputeFruit_Opti { get; set; }
        
        #endregion

        public Individu(Vector2 pos, Monde m)
            :base(pos, m)
        {
            Init(m);
            ComputeRender();
            this.PointDeVieDemarrage *= 10;
            this.PointDeVie = this.PointDeVieDemarrage;
            this.SeuilEgg = this.PointDeVieDemarrage;
        }

        public Individu Clone(Monde m)
        {
            Individu clone = new Individu(new Vector2(this.Position.X, this.Position.Y), m);
            clone.PointDeVieDemarrage = this.PointDeVieDemarrage * ((float)Monde.rand.Next(90,111)/100f);
            clone.PointDeVie = this.PointDeVieDemarrage;
            clone.Intelligence = this.Intelligence.Clone();
            clone.TempsEgg = this.TempsEgg * ((float)Monde.rand.Next(90, 111) / 100f);
            clone.FactorAgrandissement = this.FactorAgrandissement * ((float)Monde.rand.Next(90, 111) / 100f);
            clone.SeuilEgg = clone.PointDeVieDemarrage;
            return clone;
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
            RefreshPosition(monde);

            NerfMuscleActionList.EatNearestFruit(monde, this, 1, deltaTime);

            if(this.PointDeVie >= this.SeuilEgg)
            {
                this.SeuilEgg *= 1.5f;
                this.PointDeVie -= this.PointDeVieDemarrage * 1.1f;
                monde.Eggs.Add(new World.Actors.Egg(this.Clone(monde), monde));
            }

            LastTimeToComputeFruit_Opti--;
            if(LastTimeToComputeFruit_Opti == 0 || NearestFruit_Opti.Mort == true)
            {
                NearestFruit_Opti = null;
                LastTimeToComputeFruit_Opti = 1;
            }
            
        }

        private void RefreshPosition(Monde monde)
        {
            this.Position = new Vector2(Math.Max(0, Math.Min(this.Position.X, monde.TaillePx.X-1)),
                                        Math.Max(0, Math.Min(this.Position.Y, monde.TaillePx.Y-1)));

            this.PositionImage = new Vector2(this.Position.X + TAILLE_IMAGE_INDIVIDU_PX_X * this.FactorAgrandissement / 2,
                                                this.Position.Y + TAILLE_IMAGE_INDIVIDU_PX_Y * this.FactorAgrandissement / 2);
        }

        private void Init(Monde m)
        { 
            this.FactorAgrandissement = 1f;
            RefreshPosition(m);
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