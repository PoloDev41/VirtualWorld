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

        /// <summary>
        /// Temperature de la parcelle
        /// </summary>
        public float Temperature
        {
            get;

            set;
        }

        public void UpdateAsynch(float deltaTime, Monde monde)
        {
            switch (monde.SaisonCourante)
            {
                case SeasonTournament.SAISON1_2:
                    this.Temperature += deltaTime * (this.Temperature_Saison1 - this.Temperature_Saison2) / Monde.TimeInterSeason;
                    break;
                case SeasonTournament.SAISON2_1:
                    this.Temperature += deltaTime * (this.Temperature_Saison2 - this.Temperature_Saison1) / Monde.TimeInterSeason;
                    break;
                case SeasonTournament.SAISON1:
                case SeasonTournament.SAISON2:
                default:
                    break;
            }
            this.Engrais += 100 * deltaTime;
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
