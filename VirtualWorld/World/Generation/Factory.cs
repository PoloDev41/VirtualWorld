﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld.World
{
    static class Factory
    {
        static Random rand = new Random();

        public static List<Plante> AddPlantes(Monde m, int number)
        {
            List<Plante> list = new List<Plante>();
            for (int i = 0; i < number; i++)
            {
                float x = rand.Next(0, (int)m.TaillePx.X);
                float y = rand.Next(0, (int)m.TaillePx.Y);

                list.Add(CreatePlante(m.Parcelles[(int)(x / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX)][(int)(y / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX)],
                                        x, y, m));
            }

            return list;
        }

        private static Plante CreatePlante(ParcelleTerrain pt, float x, float y, Monde m)
        {
            Plante p = new Plante(new Microsoft.Xna.Framework.Vector2(x, y), m);

            int tempMin = Math.Min((int)Math.Round(pt.Temperature_Saison1),
                                    (int)Math.Round(pt.Temperature_Saison2));
            int tempMax = Math.Max((int)Math.Round(pt.Temperature_Saison1),
                                   (int)Math.Round(pt.Temperature_Saison2));
            p.TemperatureIdeal = (float)rand.Next(tempMin, tempMax + 1) + (float)rand.NextDouble();

            int ten = (int)Math.Round(p.PointDeVieDemarrage * 1.1f);
            int _ten = (int)Math.Round(p.PointDeVieDemarrage * .9f);
            p.PointDeVieDemarrage = p.PointDeVie = rand.Next(_ten, ten) + (float)rand.NextDouble();
            p.TempsGraine = (float)(rand.Next(4, 6) + rand.NextDouble());
            p.AltitudeIdeal = pt.Altitude;

            return p;
        }

        public static ParcelleTerrain[][] GenerateGround(int sizex, int sizey)
        {
            int increaseX = sizex + 10;
            int increaseY = sizey + 10;
            
            float[][] altitude = PerlinGenerator.GenerateMap(increaseX, increaseY, ParcelleTerrain.AMPLITUDE_ALTITUDE, 2, 2);
            float[][] temperature1 = PerlinGenerator.GenerateMap(increaseX, increaseY, ParcelleTerrain.AMPLITUDE_ALTITUDE, 2, 3);
            float[][] temperature2 = PerlinGenerator.GenerateMap(increaseX, increaseY, ParcelleTerrain.AMPLITUDE_ALTITUDE, 2, 3);

            ParcelleTerrain[][] ground = new ParcelleTerrain[sizex][];
            for (int i = 0; i < ground.Length; i++)
            {
                ground[i] = new ParcelleTerrain[sizey];
                for (int j = 0; j < ground[i].Length; j++)
                {
                    ground[i][j] = new ParcelleTerrain()
                    {
                        Engrais = ParcelleTerrain.ENGRAIS_MAX,
                        Altitude = altitude[i][j],
                        Temperature_Saison1 = temperature1[i][j] - Math.Abs(altitude[i][j]) / ParcelleTerrain.FACTOR_ALTITUDE_TEMPERATURE + ParcelleTerrain.OFFSET_TEMPERATURE,
                        Temperature_Saison2 = temperature2[i][j] - Math.Abs(altitude[i][j]) / ParcelleTerrain.FACTOR_ALTITUDE_TEMPERATURE + ParcelleTerrain.OFFSET_TEMPERATURE,
                        Temperature = temperature1[i][j] - Math.Abs(altitude[i][j]) / ParcelleTerrain.FACTOR_ALTITUDE_TEMPERATURE + ParcelleTerrain.OFFSET_TEMPERATURE,
                    };
                }
            }
            return ground;
        }

        internal static List<Fruit> AddFruits(Monde monde, int n)
        {
            List<Fruit> list = new List<Fruit>();
            for (int i = 0; i < n; i++)
            {
                float x = rand.Next(0, (int)monde.TaillePx.X);
                float y = rand.Next(0, (int)monde.TaillePx.Y);

                Plante p = CreatePlante(monde.Parcelles[(int)(x / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX)][(int)(y / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX)],
                                        x, y, monde);

                list.Add(new Fruit(new Microsoft.Xna.Framework.Vector2(x, y),
                    p,
                    monde));
            }

            return list;
        }
    }
}
