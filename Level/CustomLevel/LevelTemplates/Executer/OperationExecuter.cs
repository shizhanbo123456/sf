using LevelCreator.TargetTemplate;
using System.Collections.Generic;
using Variety.Base;

namespace LevelCreator.Executer
{
    public struct OperationExecuter
    {
        public static void Execute(ushort id, ushort delay, Target target)
        {
            var info=Tool.LevelCreatorManager.GetOperationInfo(id);

            foreach (var i in info.subSkillOperators)
            {
                if (i.subSkillId >= id)
                {
                    UnityEngine.Debug.LogWarning($"SubSkillId {i.subSkillId} is equal or greater than OperationId {id}, which may cause an infinite loop. Skipping this sub skill.");
                    //continue;
                }
                target.TimeLineWork.AddEvent(i.delay*0.001f+delay*0.001f,i);
            }
            foreach (var i in info.bulletShoots)
            {
                target.TimeLineWork.AddEvent(i.delay * 0.001f + delay * 0.001f, i);
            }
            foreach (var i in info.motionActions)
            {
                target.TimeLineWork.AddEvent(i.delay * 0.001f + delay * 0.001f, i);
            }
            foreach (var i in info.effectOperations)
            {
                target.TimeLineWork.AddEvent(i.delay * 0.001f + delay * 0.001f, i);
            }
        }
    }
}