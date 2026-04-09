using LevelCreator.TargetTemplate;
using System.Collections.Generic;
using Variety.Base;

namespace LevelCreator.Executer
{
    public struct MotionExecuter
    {
        public static void Execute(ushort id, Target receiver)
        {
            receiver.ApplyMotion(new MotionBase(id));
        }
    }
}