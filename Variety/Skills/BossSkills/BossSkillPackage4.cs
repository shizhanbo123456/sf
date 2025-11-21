using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss4
{
    public class RepeatBoss4 : RepeatContent
    {
        public RepeatBoss4(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            int c = Ore.Ores.Values.Count;
            if (c == 0) return;
            target.ApplyEffect(new Speed(target, target, c * 0.7f,1));
            target.ApplyEffect(new DefenseBoost(target, target, c * 1.2f, 1));
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 8f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(99999, false);
            var angle = Dt2Degree(t.transform.position - Target.transform.position);
            for(int i = 0; i < 20; i++)
            {
                AddEvent(0.1f * i, new TimeLineData(Target,i),(d) =>
                {
                    GetBullet(7).Init(new BulletAngleNonFacing(d.Target, 5, 10, angle+Mathf.Sin(d.index*0.5f)*45, 0.3f), new BulletDataSlight(d.Target, new Damage_Once(), 0.3f)).Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 10f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            var t=Target.GetNearestEnemy(99999, false);
            t.ApplyMotion(new MotionDir(Vector2.up * 10, 0.7f, false, 1, false,false));
            AddEvent(0.7f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir((t.transform.position- d.Target.transform.position).normalized*30,1,true,1,true,true));
                GetBullet(12).Init(new BulletFollow(d.Target, 1, 3), new BulletDataStrike(d.Target, new Damage_Once(), 2.5f)).Shoot();
            });
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill2 : SkillBoss
    {
        private EffectCollection ec1;
        private EffectCollection ec2;
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 30f;
            restoreTime = 1;

            ec1 = new EffectCollection(t, (Effects.Slowness, 2f, 1f));
            ec2 = new EffectCollection(t, (Effects.ArmorShatter, 55f, 10f));
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            foreach(var i in Ore.Ores.Values)
            {
                GetBullet(11).Init(new BulletStatic(Target, 0.5f, 3, i.transform.position), new BulletDataStrike(Target, new Damage_Once(), 3f,ec:ec2)).Shoot();
                GetBullet(6).Init(new BulletStatic(Target,20,3,i.transform.position), new BulletDataSlight(Target, new Damage_Time(0.5f), 0.2f,ec1)).Shoot();
            }
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 10f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            var t=Target.GetNearestEnemy(99999, false).transform.position;
            WarningRect.Warn(Target.transform.position, (t - Target.transform.position).normalized * 60 + Target.transform.position, 3, 1f);
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            AddEvent(1f, (d) =>
            {
                GetBullet(12).Init(new BulletFromTo(d.Target,3, d.Target.transform.position, (t - d.Target.transform.position).normalized * 60 + d.Target.transform.position,3), new BulletDataStrike(d.Target, new Damage_Once(), 3.3f)).Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 14f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            var t = Target.GetNearestEnemy(99999, false).transform.position;
            WarningCircle.Warn(t,2,0.5f);
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            AddEvent(1f, (d) =>
            {
                for(int i = 0; i < 3; i++)
                {
                    Vector3 v = new Vector3(Mathf.Cos(i * 120 * Mathf.Deg2Rad), Mathf.Sin(i * 120 * Mathf.Deg2Rad))*10;
                    GetBullet(12).Init(new BulletFromTo(d.Target,1,t+v,t-v,2), new BulletDataStrike(d.Target, new Damage_Once(), 3.3f)).Shoot();
                }
            }); 
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 30f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(8,false).Count>0;
        }
        protected override void OnUseSkill()
        {
            GetBullet(4).Init(new BulletStatic(Target, 4,8,Target.transform.position), new BulletDataAttract(Target, 0.1f, 0.1f, 8)).Shoot();
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
}