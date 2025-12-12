using UnityEngine;
using Variety.Base;

//可通过继承BulletBase发射子弹，继承HitEventBase实现命中事件，设置EffectCollection实现效果
//技能中直接添加效果，使用 Local 版本
//Apply MotionBase实现移动控制，Apply TimeLineEvents实现延时调用
namespace Variety.Template
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
        public sealed override bool CanUse(Target Target)
        {
            return true;
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
        protected float CD;
        public SkillCD():base()
        { 
        }
        public override SkillBaseController CreateSkillController(Target t, int index, bool createUI)
        {
            return SkillCDController.Create(index,t, CD,createUI);
        }
        public sealed override bool CanUse(Target Target)
        {
            return true;
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
        protected float CD;
        public SkillStorable() : base()
        {
        }
        public override SkillBaseController CreateSkillController(Target t, int index, bool createUI)
        {
            return SkillStorableController.Create(index,t,MaxstoreTime,CD,createUI);
        }
        public sealed override bool CanUse(Target Target)
        {
            return true;
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
    public class SkillBoss : SkillBase
    {
        protected float cd;
        public SkillBoss() : base()
        {
        }
        public override SkillBaseController CreateSkillController(Target t, int index, bool createUI)
        {
            return SkillCDController.Create(index, t, cd,createUI);
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            BulletSystemCommon.CurrentShooter = Target;
            OnUse(Target,pos,faceright);
        }
        protected virtual void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            
        }
        protected static float Dt2Degree(Vector3 dt)
        {
            return Mathf.Atan(dt.y / dt.x)*Mathf.Rad2Deg+(dt.x<0?180:0);
        }
        protected static Vector3 Angle2Vector(float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}