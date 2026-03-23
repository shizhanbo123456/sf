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
    public abstract class SkillBase
    {
        public Vector2Int sprite;
        public string Name;
        public string Description;
        public float TimeNeeded=2;//释放所需的时间

        public SkillBase()
        {

        }
        public virtual SkillBaseController CreateSkillController(Target t, int index, bool createUI) => throw new Exception("该类不可创建技能栏");
        public virtual bool CanUse(Target Target)
        {
            return true;
        }
        public abstract void UseSkill(Target Target, Vector3 pos, bool faceright);
    }
}