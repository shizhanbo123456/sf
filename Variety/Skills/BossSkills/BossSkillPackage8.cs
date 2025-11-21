using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss8
{
    public class RepeatBoss8 : RepeatContent
    {
        public RepeatBoss8(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            bool f = false;
            foreach(var i in Lantern.Lanterns.Values) 
                if (i.TimeOfDie > 0.01f)
                {
                    f=true; 
                    break;
                }
            if (f)
            {
                foreach(var i in target.GetEnemyInRange(99999, false))
                {
                    i.ApplyEffect(new Speed(target, i, 6, 1));
                    i.ApplyEffect(new JumpBoost(target, i, 10, 1));
                    i.ApplyEffect(new Stoic(target, i, 1, 1));
                }
            }
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(99999, false);
            Target.ApplyMotion(new MotionDir((t.transform.position - Target.transform.position).normalized * 20, 1f, true, 1, true, true));
            GetBullet(4).Init(new BulletFollow(Target,1,4f), new BulletDataSlight(Target, new Damage_Once(), 2f)).Shoot();
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetNearestEnemy(5,false)!=null;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(5).Init(new BulletStaticScaleChange(Target,1f,2,5), new BulletDataAttract(Target,0.2f,0.05f,7)).Shoot();
            AddEvent(1, (d) =>
            {
                GetBullet(5).Init(new BulletAngle(d.Target, 1, 10, -90, 2f), new BulletDataAttract(d.Target, 0.2f, 0.05f,8)).Shoot();
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(99999, false);
            GetBullet(7).Init(new BulletAim(Target,3,Target.transform.position,12,t.transform.position,1.2f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        private EffectCollection ec;
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 3f;
            cd = 60f;
            restoreTime = 1;

            ec = new EffectCollection(Target, (Effects.Paralysis, 0, 3));
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(4).Init(new BulletStaticScaleChange(Target,3,0,15), new BulletDataSlight(Target, new Damage_VFXOnly(), 0.5f)).Shoot();
            AddEvent(3f, (d) =>
            {
                foreach(var i in Monster.Monsters.Values)
                {
                    i.ApplyEffect(new DamageBoost(Target, i, 50, 30));
                }
                for(int startangle=0;startangle<=270;startangle+=90)
                GetBullet(4).Init(new BulletOrbit(d.Target,10,4,90,startangle,0.5f), new BulletDataCommon(d.Target, new Damage_Once(), 1.2f,ec:ec)).Shoot();

                GetBullet(4).Init(new BulletStaticScaleChange(Target, 3, 15), new BulletDataSlight(Target, new Damage_Once(), 1.5f, ec: ec)).Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        private EffectCollection ec;
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 3f;
            cd = 20f;
            restoreTime = 1;

            ec = new EffectCollection(t,(Effects.Sticky,0,3),(Effects.AccuracyDecrease,20,5));
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var angle = Dt2Degree(Target.GetNearestEnemy(99999, false).transform.position - Target.transform.position);
            for(int i=-20;i<=20;i+=5)
                GetBullet(7).Init(new BulletAngleNonFacing(Target, 5, 8, angle+i, 0.6f), new BulletDataSlight(Target, new Damage_Once(), 0.5f,ec)).Shoot();
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 5f;
            cd = 35f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(8,false).Count>0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int i = 0; i < 15; i++)
            {
                AddEvent(i * 0.2f, new TimeLineData(Target,i),(d) =>
                {
                    float angle = -12 * Mathf.Deg2Rad * d.index;
                    var t = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle))*6 + d.Target.transform.position;
                    GetBullet(7).Init(new BulletProjectileAim(d.Target,1.5f, d.Target.transform.position,Vector3.up*55,t,0.8f,0.4f), new BulletDataSlight(Target, new Damage_Once(), 3.5f)).Shoot();
                });
            }
        }
    }
}