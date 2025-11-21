using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss5
{
    public class RepeatBoss5 : RepeatContent
    {
        public RepeatBoss5(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            int c = Ore.Ores.Values.Count;
            if (c == 6) return;
            target.ApplyEffect(new Speed(target, target, (6-c) * 0.7f, 1));
            target.ApplyEffect(new DefenseBoost(target, target, (6-c) * 1.2f, 1));
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

            GetBullet(7).Init(new BulletAngleNonFacing(Target, 10, 5, angle, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.3f)).Shoot();
            for (int i = 1; i < 5; i++)
            {
                int j = i;
                AddEvent(0.1f * i, (d) =>
                {
                    GetBullet(7).Init(new BulletAngleNonFacing(d.Target, 10, 5, angle+j*5, 0.3f), new BulletDataSlight(d.Target, new Damage_Once(), 0.3f)).Shoot();
                    GetBullet(7).Init(new BulletAngleNonFacing(d.Target, 10, 5, angle-j*5, 0.3f), new BulletDataSlight(d.Target, new Damage_Once(), 0.3f)).Shoot();
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
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for (int i = 0; i < 5; i++)
            {
                float rad = (30 + i * 30)*Mathf.Deg2Rad;
                GetBullet(7).Init(new BulletProjectile(Target, 3, new Vector3(Mathf.Cos(rad),Mathf.Sin(rad))*12, 1f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            }
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1f;
            cd = 10f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up * 20, 1f, true, 2, true, true));
            AddEvent(1f, (d) =>
            {
                var p = d.Target.GetNearestEnemy(99999, false);
                var angle= Dt2Degree(p.transform.position - Target.transform.position);
                for(int offset=-10;offset<=10;offset++)
                    GetBullet(7).Init(new BulletAngle(d.Target, 2, 15, angle , 0.3f), new BulletDataSlight(d.Target, new Damage_Once(), 0.5f)).Shoot();
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1.2f;
            cd = 20f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var list = Target.GetEnemyInRange(99999, false);
            foreach (var i in list) WarningCircle.Warn(i.transform.position, 2, 1);
            AddEvent(1f, (d) =>
            {
                foreach (var i in list)
                {
                    for(int j=0;j<4;j++)
                        GetBullet(12).Init(new BulletOrbitWorld(d.Target, 7,i.transform.position, 4, 90,90*j,0.8f), new BulletDataCommon(d.Target, new Damage_Once(), 0.3f)).Shoot();
                }
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1.5f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(8,true).Count>0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(80, true);
            if (t == null) return;
            WarningCircle.Warn(t.transform, 2.5f, 0.6f);
            Vector3 v=new Vector3();
            AddEvent(0.6f, (d) =>
            {
                v= t.transform.position;
                d.Target.ApplyMotion(new MotionDir((t.transform.position - d.Target.transform.position) * 2.5f, 0.4f, true, 1, true, true));
            });
            AddEvent(1f, (d) =>
            {
                GetBullet(11).Init(new BulletStatic(d.Target,0.3f,2.5f,v), new BulletDataCommon(d.Target, new Damage_Once(), 2.2f)).Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 5;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int i = 0; i < 100; i++)
            {
                int j = i;
                AddEvent(j * 0.05f, (d) =>
                {
                    GetBullet(7).Init(new BulletAngle(d.Target, 2, 20, 7*j, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 0.9f)).Shoot();
                });
            }
        }
    }
}