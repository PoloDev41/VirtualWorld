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

            if(nearest != null && NerfSensorList.IsCloosed(nearest, proprietaire) == true)
            {
                proprietaire.PointDeVie += nearest.TakeLife(proprietaire.FactorAgrandissement / (nearest.FactorAgrandissement * 2)
                                            * (float)power * deltaTime * 100)*2; //*100 to compense deltaTime
            }

        }

        public static void RotationIndividu(Monde m, Individu proprietaire, double power, float deltaTime)
        {
            proprietaire.Angle += (float)(power * deltaTime)*5;
        }

        public static void WalkRight(Monde m, Individu proprietaire, double power, float deltaTime)
        {
            double distanceX = power * deltaTime*50;
            double moveX = distanceX * Math.Cos(proprietaire.Angle);
            double moveY = distanceX * Math.Sin(proprietaire.Angle);

            proprietaire.Position += new Microsoft.Xna.Framework.Vector2((float)moveX, (float)moveY);
        }

        public static NerfMuscleAction[] MuscleActionStock = new NerfMuscleAction[]
        {
            (NerfMuscleAction)RotationIndividu,
            WalkRight,
            EatNearestFruit
        };
    }
}
