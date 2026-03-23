using LevelCreator.TargetTemplate;
using UnityEngine;
using Variety.Base;

namespace LevelCreator.Skills 
{
    public class SkillNonCD : SkillBase
    {
        public SkillNonCD():base()
        { 
        }
        public override SkillBaseController CreateSkillController(Target t,int index, bool createUI)
        {
            return SkillNonCDController.Create(index,t,createUI);
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            BulletSystemCommon.CurrentShooter = Target;
            OnUse(Target,pos,faceright);
        }

        protected virtual void OnUse(Target Target, Vector3 pos, bool faceright)
        {

        }
    }
    public class SkillCD : SkillBase
    {
        protected float cd;
        public SkillCD():base()
        { 
        }
        public override SkillBaseController CreateSkillController(Target t, int index, bool createUI)
        {
            return SkillCDController.Create(index,t, cd,createUI);
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            BulletSystemCommon.CurrentShooter = Target;
            OnUse(Target,pos,faceright);
        }

        protected virtual void OnUse(Target Target, Vector3 pos, bool faceright)
        {

        }
    }
    public class SkillStorable : SkillBase
    {
        protected int MaxstoreTime;
        protected float cd;
        public SkillStorable() : base()
        {
        }
        public override SkillBaseController CreateSkillController(Target t, int index, bool createUI)
        {
            return SkillStorableController.Create(index,t,MaxstoreTime,cd,createUI);
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            BulletSystemCommon.CurrentShooter = Target;
            OnUse(Target,pos,faceright);
        }
        protected virtual void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            
        }
    }
}