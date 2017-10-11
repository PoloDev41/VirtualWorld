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
        public static bool IsCloosed(EtreVivant ev1, EtreVivant ev2)
        {
            float dist = Vector2.Distance(ev1.Position, ev2.Position);

            if(dist < (10*ev1.FactorAgrandissement*ev2.FactorAgrandissement))
            {
                return true;
            }
            return false;
        }

        public static Fruit FindNearestFruit(Monde m, Individu proprietaire)
        {
            if (proprietaire.NearestFruit_Opti != null)
                return proprietaire.NearestFruit_Opti;

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
                        if (fruits[i] == null)
                            continue;

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
                        proprietaire.NearestFruit_Opti = refFruit;
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

            angle = angle - proprietaire.Angle;

            if (angle > Math.PI)
                angle = (float)(-2 * Math.PI) + proprietaire.Angle;
            else if (angle < -Math.PI)
                angle = (float)(2 * Math.PI) + proprietaire.Angle;

            return angle;
        }

        public static double DeltaTemperatureIndividu(Monde m, Individu proprietaire)
        {
            return Math.Abs(proprietaire.IdealTemperature - proprietaire.RefParcelle.Temperature);
        }

        public static double HealthIndividu(Monde m, Individu proprietaire)
        {
            return proprietaire.PointDeVie / proprietaire.PointDeVieDemarrage;
        }

        public static double DistanceNearestFruit(Monde m, Individu proprietaire)
        {
            if (m.Fruits.Count == 0)
                return 0;

            Fruit nearest = FindNearestFruit(m, proprietaire);
            return 100/Vector2.Distance(nearest.Position, proprietaire.Position);
        }

        public static NerfSensor[] SensorStock = new NerfSensor[]
        {
            (NerfSensor)AngleNearestFruit,
            DistanceNearestFruit,
            HealthIndividu,
            DeltaTemperatureIndividu
        };

        public static NerfSensor GetRandomSensor()
        {
            return SensorStock[Monde.rand.Next(0, SensorStock.Length)];
        }
    }
}
