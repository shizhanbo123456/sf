using LevelCreator.Executer;
using LevelCreator.TargetTemplate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.Skills
{
    /// <summary>
    /// 可重写init,CanUse,UseSkill,Update<br></br>
    /// 可使用ApplyMotion ApplyMotionTo ApplyEvents(延时)
    /// </summary>
    public struct SkillExecuter
    {
        public static SkillControllerBase CreateSkillController(short id,Target t)
        {
            var info=Tool.LevelCreatorManager.GetSkillInfo(id);
            if (info.cd == 0)
            {
                return SkillNonCDController.Create(id, t);
            }
            else if (info.maxStoreTime == 0)
            {
                return SkillCDController.Create(id, t, info.cd*0.001f);
            }
            else
            {
                return SkillStorableController.Create(id, t, info.maxStoreTime, info.cd*0.001f);
            }
        }
        public static bool CanUse(short id, Target Target)
        {
            return true;
        }
        public static void UseSkill(short id, Target Target, Vector3 pos, bool faceright)
        {
            var info = Tool.LevelCreatorManager.GetSkillInfo(id);

            BulletSystemCommon.CurrentShooter = Target;

            for (int index = 0; index < info.actionDelays.Count; index++)
            {
                short delay = info.actionDelays[index];
                short action = info.actionIds[index];

                OperationExecuter.Execute(action, delay, Target);
            }
        }
    }
}