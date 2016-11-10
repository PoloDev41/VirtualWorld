using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWorld.World
{
    static class PerlinGenerator
    {
        /// <summary>
        /// generateur aléatoire pour les algorithmes de perlin
        /// </summary>
        private static Random Generator = new Random();

        /// <summary>
        /// genere une carte aléatoire
        /// </summary>
        /// <param name="sizex">taille x</param>
        /// <param name="sizey">taille y</param>
        /// <param name="amplitude">amplitude min/max de la carte aléatoire</param>
        /// <returns>map généré</returns>
        public static float[][] GenerateMap(int sizex, int sizey, float amplitude, int frequence, int octave)
        {
            float[][][] rand = new float[octave][][];
            for (int i = 0; i < octave; i++)
            {
                rand[i] = RandomLayer(sizex, sizey, amplitude);
            }

            float[][] map = new float[sizex][];
            int f_courante = frequence;

            float persistance;
            float somme_persis = 0; ;
            for (int n = 0; n < octave; n++)
            {
                persistance = 1 / (float)(n+2);
                somme_persis += persistance;
                for (int i = 0; i < sizex; i++)
                {
                    if(map[i] == null)
                        map[i] = new float[sizey];
                    for (int j = 0; j < sizey; j++)
                    {
                        float a = valeur_interpolee(i, j, f_courante, rand[n]) * persistance;
                        map[i][j] += a;
                    }
                }
                    
                f_courante *= frequence;
            }

            for (int i = 0; i < sizex; i++)
            {
                for (int j = 0; j < sizey; j++)
                {
                    map[i][j] /= somme_persis;
                }
            }

            return map;
        }


        private static float valeur_interpolee(int i, int j, int frequence, float[][] layer)
        {
            int borne1x, borne1y, borne2x, borne2y, q;
            float pas;
            pas = (float)layer.Length / frequence;

            q = (int)(i / pas);
            borne1x = (int)(q * pas);
            borne2x = (int)((q+1) *pas);

            if (borne2x >= layer.Length)
                borne2x = layer.Length - 1;

            q = (int)(j / pas);
            borne1y = (int)(q * pas);
            borne2y = (int)((q + 1) * pas);

            if (borne2y >= layer[0].Length)
                borne2y = layer[0].Length - 1;

            /* récupérations des valeurs aléatoires aux bornes */
            float b00, b01, b10, b11;
            b00 = layer[borne1x][borne1y];
            b01 = layer[borne1x][borne2y];
            b10 = layer[borne2x][borne1y];
            b11 = layer[borne2x][borne2y];

            float v1 = interpolate(b00, b01, borne2y - borne1y, j - borne1y);
            float v2 = interpolate(b10, b11, borne2y - borne1y, j - borne1y);
            float fin = interpolate(v1, v2, borne2x - borne1x, i - borne1x);

            return fin;
        }


        private static float interpolate(float y1, float y2, int n, int delta)
        {
            if (n == 0)
                return y1;
            if (n == 1)
                return y2;

            float a = (float)delta / n;

            double v1 = 3 * Math.Pow(1 - a, 2) - 2 * Math.Pow(1 - a, 3);
            double v2 = 3 * Math.Pow(a, 2) - 2 * Math.Pow(a, 3);

            return (float)(y1 * v1 +  y2* v2);
        }


        /// <summary>
        /// génère le calque aléatoire
        /// </summary>
        /// <param name="sizex">taille x</param>
        /// <param name="sizey">taille y</param>
        /// <param name="amplitude">amplitude min/max de la carte aléatoire</param>
        /// <returns>calque aléatoire</returns>
        private static float[][] RandomLayer(int sizex, int sizey, float amplitude)
        {
            float[][] layer = new float[sizex][];
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new float[sizey];
                for (int j = 0; j < layer[i].Length; j++)
                {
                    layer[i][j] = (((float)Generator.NextDouble() - 0.5f) * 2) * amplitude;
                }
            }

            return layer;
        }

    }
}
