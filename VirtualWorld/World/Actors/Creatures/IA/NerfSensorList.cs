using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld.World.Actors.Creature;

namespace VirtualWorld.World.Actors.Creatures.IA
{
    public static class NerfSensorList
    {
        private static Fruit FindNearestFruit(Monde m, Individu proprietaire)
        {
            Vector2 posParcelle = new Vector2((int)proprietaire.Position.X / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX,
                                            (int)proprietaire.Position.Y / ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX);
            double distanceNearestFruit = double.MaxValue;
            Fruit refFruit = null;
            int radius = 0;
            while (m.Fruits.Count > 0)
            {
                List<ParcelleTerrain> pt = m.GetParcellePerimeter(posParcelle, radius);
                List<Fruit> fruits = new List<Fruit>();
                for (int i = 0; i < pt.Count; i++)
                {
                    fruits.AddRange(pt[i].RefFruits);
                }

                if (fruits.Count == 0)
                {
                    radius++;
                }
                else //there is fruits in this perimeter
                {
                    for (int i = 0; i < fruits.Count; i++)
                    {
                        float distance = Vector2.DistanceSquared(proprietaire.Position, fruits[i].Position);
                        if(distance < distanceNearestFruit)
                        {
                            refFruit = fruits[i];
                            distanceNearestFruit = distance;
                        }
                    }
                    if(refFruit == null)
                    {
                        distanceNearestFruit = double.MaxValue;
                        radius++;
                    }
                    else
                    {
                        return refFruit;
                    }
                }
            }
            return null;
        }

        public static double AngleNearestFruit(Monde m, Individu proprietaire)
        {
            if (m.Fruits.Count == 0)
                return 0;

            Fruit nearest = FindNearestFruit(m, proprietaire);
            double angle = Math.Atan2(nearest.Position.Y - proprietaire.Position.Y, nearest.Position.X - proprietaire.Position.X);

            return angle - proprietaire.Angle;
        }

        public static NerfSensor[] SensorStock = new NerfSensor[]
        {
            (NerfSensor)AngleNearestFruit
        };
    }
}
