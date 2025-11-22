using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss1
{
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int i = 0; i < 24; i++)
            {
                var b = GetBullet(11);
                b.Init(0.5f,liftstoiclevel:0);
                BulletAngleSystem.RegistObject(b,0.6f,5,10,i*15);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            cd = 8f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var angle=Dt2Degree(Target.GetNearestEnemy(1000, false).transform.position-Target.transform.position);
            for(int i = -2; i < 3; i++)
            {
                var b = GetBullet(12);
                b.Init(0.9f);
                BulletAngleNonFacingSystem.RegistObject(b,0.6f,2,15,angle+i*10);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2(Target t) : base(t)
        {
            Description = "";
            cd = 10f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(10,true).Count>0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t=Target.GetNearestEnemy(20, true);
            if (t != null)
            {
                var x = t.transform.position.x - Target.transform.position.x;
                Target.ApplyMotion(new MotionDir(new Vector2(x*15,0),1,true,2,true,true));
                var b = GetBullet(14);
                b.Init(3.2f);
                BulletFollowSystem.RegistObject(b,2f,2f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill3 : SkillBoss
    {
        private static List<Vector3> VelocityStart = new List<Vector3>() 
        {
            new Vector3(6,11),
            new Vector3(-6,11),
            new Vector3(11,6),
            new Vector3(-11,6),
        };
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 25f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            Target.ApplyMotion(new MotionDir(Vector2.up * 30, 1, true, 1, true, true));
            AddEvent(1, (d) =>
            {
                var t=d.Target.GetNearestEnemy(99999, false);
                float x=t.transform.position.x-d.Target.transform.position.x;
                Target.ApplyMotion(new MotionDir(new Vector2(x*2,0), 0.5f, true, 1, true, true));
            });
            AddEvent(1.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(Vector2.down * 60, 0.5f, true, 1, true, true));
            });
            AddEvent(2f, (d) =>
            {
                var b = GetBullet(11);
                b.Init(2.8f);
                BulletStaticSystem.RegistObject(b, 6f, 0.4f, d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();

                foreach (var i in VelocityStart)
                {
                    b = GetBullet(7);
                    b.Init(1.6f);
                    BulletProectileSystem.RegistObject(b, 2f, 4f, d.Target.transform.position,i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 5f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(5,false).Count==0;
        }
        protected override void OnUseSkill()
        {
            var p = Target.transform.position;
            for(int i = 0; i < 10; i++)
            {
                AddEvent(i * 0.1f,new TimeLineData(Target,i), (d) =>
                {
                    var b = GetBullet(12);
                    b.Init(0.9f);
                    BulletDirAwaitSystem.RegistObject(b, 0.5f, 6,1.2f, p + new Vector3(5, d.index * 1.3f), 8,Vector3.right);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                    b = GetBullet(12);
                    b.Init(0.9f);
                    BulletDirAwaitSystem.RegistObject(b, 0.5f, 6, 1.2f, p + new Vector3(-5, d.index * 1.3f), 8, Vector3.left);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 15f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            foreach(var i in Target.GetEnemyInRange(99999, false))
            {
                if (i.effectController != null)
                {
                    i.effectController.AddEffect(new Silence(Target, i, 4));
                }
            }
            for(int i = -5; i < 5; i+=2)
            {
                WarningCircle.Warn(Target.transform.position + new Vector3(i * 5, 0, 0), 3, 1);
            }
            AddEvent(0.5f, (d) =>
            {

                for (int i = -4; i < 4; i += 2)
                {
                    WarningCircle.Warn(d.Target.transform.position + new Vector3(i * 5, 0, 0), 3, 1);
                }
            });
            AddEvent(1, (d) =>
            {
                for (int i = -5; i < 5; i += 2)
                {
                    var b = GetBullet(11);
                    b.Init(2.8f);
                    BulletStaticSystem.RegistObject(b,3,0.3f, d.Target.transform.position + new Vector3(i * 5, 0, 0));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
            AddEvent(1.5f, (d) =>
            {
                for (int i = -4; i < 4; i += 2)
                {
                    var b = GetBullet(11);
                    b.Init(2.8f);
                    BulletStaticSystem.RegistObject(b, 3, 0.3f, d.Target.transform.position + new Vector3(i * 5, 0, 0));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
}