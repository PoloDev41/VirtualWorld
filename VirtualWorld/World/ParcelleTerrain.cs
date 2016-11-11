using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld
{
    public class ParcelleTerrain
    {
        public static readonly int TAILLE_IMAGE_PARCELLE_PX = 30;

        public static readonly float ENGRAIS_MAX = 1000;
        public static readonly int AMPLITUDE_ALTITUDE = 50;
        public static readonly int AMPLITUDE_TEMPERATURE = 30;
        public static readonly int OFFSET_TEMPERATURE = 10;
        public static readonly int FACTOR_ALTITUDE_TEMPERATURE = 5;
        //public static readonly int TEMPERATURE_MAX = AMPLITUDE_TEMPERATURE + OFFSET_TEMPERATURE + 20;
        //public static readonly int TEMPERATURE_MIN = -AMPLITUDE_TEMPERATURE + OFFSET_TEMPERATURE - AMPLITUDE_ALTITUDE / 5;

        public static Texture2D ParcelleHerbe;
        public static Texture2D ParcelleNeige;
        public static Texture2D ParcelleDesert;
        public static Texture2D ParcelleEauPeuProfonde;
        public static Texture2D ParcelleEauProfonde;
        public static Texture2D Blanc;

        public static ParcelleTerrain TransformPixelToParcelle(Monde m, Vector2 posPx)
        {
            return m.Parcelles[(int)posPx.X / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX][(int)posPx.Y / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX];
        }

        public static float OffsetTemperatureParcelle { get; set; } = 0;

        private float _Engrais;

        /// <summary>
        /// engrais de la parcelle
        /// </summary>
        public float Engrais
        {
            get { return _Engrais; }
            set {
                _Engrais = Math.Min(value, ENGRAIS_MAX);
            }
        }

        /// <summary>
        /// Altitude de la parcelle
        /// </summary>
        public float Altitude
        {
            get;

            set;
        }

        /// <summary>
        /// temperature de la 1ere saison
        /// </summary>
        public float Temperature_Saison1 { get; set; }
        /// <summary>
        /// temperature de la 2ème saison
        /// </summary>
        public float Temperature_Saison2 { get; set; }


        private float _temperature;
        /// <summary>
        /// Temperature de la parcelle
        /// </summary>
        public float Temperature
        {
            get { return _temperature + OffsetTemperatureParcelle; }
            set { _temperature = value; }
        }

        /// <summary>
        /// lsit of fruit on this parcelle
        /// </summary>
        public List<Fruit> RefFruits { get; set; } = new List<Fruit>();

        public void UpdateAsynch(float deltaTime, Monde monde)
        {
            switch (monde.SaisonCourante)
            {
                case SeasonTournament.SAISON1_2:
                    this._temperature += deltaTime * (this.Temperature_Saison1 - this.Temperature_Saison2) / Monde.TimeInterSeason;
                    break;
                case SeasonTournament.SAISON2_1:
                    this._temperature += deltaTime * (this.Temperature_Saison2 - this.Temperature_Saison1) / Monde.TimeInterSeason;
                    break;
                case SeasonTournament.SAISON1:
                case SeasonTournament.SAISON2:
                default:
                    break;
            }
            this.Engrais += 100 * deltaTime;
            for (int i = this.RefFruits.Count - 1; i >= 0; i--)
            {
                if(this.RefFruits[i].Mort == true)
                {
                    this.RefFruits.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// add a fruit as a reference
        /// </summary>
        /// <param name="reff">referenced fruit</param>
        public void AddFruit(Fruit reff)
        {
            this.RefFruits.Add(reff);
        }

        public float RemoveEngrais(float wish)
        {
            if(wish > Engrais)
            {
                float t = Engrais;
                Engrais = 0;
                return t;
            }
            else
            {
                Engrais -= wish;
                return wish;
            }
        }
    }
}
