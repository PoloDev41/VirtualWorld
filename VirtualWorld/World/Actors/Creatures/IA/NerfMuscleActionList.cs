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
        public static void RotationIndividu(Monde m, Individu proprietaire, double power, float deltaTime)
        {
            proprietaire.Angle += (float)(power * deltaTime);
            /*if (proprietaire.Angle > Math.PI)
                proprietaire.Angle -= (float)(Math.PI * 2);
            if (proprietaire.Angle < -Math.PI)
                proprietaire.Angle += (float)(Math.PI * 2);*/

            //proprietaire.Angle = (float)(proprietaire.Angle % Math.PI*2);
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
            WalkRight
        };
    }
}
