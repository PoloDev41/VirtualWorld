using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWorld.World.Actors.Creature;

namespace VirtualWorld.World.Actors.Creatures.IA
{
    public static class NerfMuscleActionList
    {
        public static void EatNearestFruit(Monde m, Individu proprietaire, double power, float deltaTime)
        {
            Fruit nearest = NerfSensorList.FindNearestFruit(m, proprietaire);

            proprietaire.PointDeVie -= (float)(power * deltaTime * proprietaire.FactorAgrandissement);
            if (nearest != null && NerfSensorList.IsCloosed(nearest, proprietaire) == true)
            {
                proprietaire.PointDeVie += nearest.TakeLife(proprietaire.FactorAgrandissement / (nearest.FactorAgrandissement * 2)
                                            * (float)power * deltaTime * 100)*2; //*100 to compense deltaTime
                nearest.LuckGraine /= 2;
            }

        }

        public static void RotationIndividu(Monde m, Individu proprietaire, double power, float deltaTime)
        {
            proprietaire.Angle += (float)(power * deltaTime)*5;
            if (proprietaire.Angle > Math.PI)
                proprietaire.Angle = (float)(-2*Math.PI) + proprietaire.Angle;
            else if (proprietaire.Angle < -Math.PI)
                proprietaire.Angle = (float)(2 * Math.PI) + proprietaire.Angle;

            proprietaire.PointDeVie -= (float)(power * deltaTime * proprietaire.FactorAgrandissement);
        }

        public static void WalkRight(Monde m, Individu proprietaire, double power, float deltaTime)
        {
            double distanceX = power * deltaTime*50;
            double moveX = distanceX * Math.Cos(proprietaire.Angle);
            double moveY = distanceX * Math.Sin(proprietaire.Angle);

            proprietaire.PointDeVie -= (float)(power * deltaTime * proprietaire.FactorAgrandissement * 2);
            proprietaire.Position += new Microsoft.Xna.Framework.Vector2((float)moveX, (float)moveY);
        }

        public static NerfMuscleAction[] MuscleActionStock = new NerfMuscleAction[]
        {
            (NerfMuscleAction)RotationIndividu,
            WalkRight,
            //EatNearestFruit
        };

        public static NerfMuscleAction GetRandomAction()
        {
            return MuscleActionStock[Monde.rand.Next(0, MuscleActionStock.Length)];
        }
    }
}
