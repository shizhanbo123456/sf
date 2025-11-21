using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
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
                GetBullet(7).Init(new BulletAngle(Target, 5, 10, i*15, 0.6f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
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
                GetBullet(12).Init(new BulletAngleNonFacing(Target, 2, 15, angle+i*10, 0.6f), new BulletDataCommon(Target, new Damage_Once(), 0.5f)).Shoot();
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
                GetBullet(14).Init(new BulletFollow(Target, 2, 2f), new BulletDataCommon(Target, new Damage_Once(), 3.2f)).Shoot();
            }
        }
    }
    public class Skill3 : SkillBoss
    {
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
                GetBullet(11).Init(new BulletStatic(d.Target,0.4f,6,Target.transform.position), new BulletDataStrike(d.Target, new Damage_Once(), 2.8f)).Shoot();
                GetBullet(7).Init(new BulletProjectile(d.Target,4f,new Vector3(4,7),2), new BulletDataCommon(d.Target, new Damage_Once(), 1.6f)).Shoot();
                GetBullet(7).Init(new BulletProjectile(d.Target,4f,new Vector3(-4,7),2), new BulletDataCommon(d.Target, new Damage_Once(), 1.6f)).Shoot();
                GetBullet(7).Init(new BulletProjectile(d.Target,4f,new Vector3(7,4),2), new BulletDataCommon(d.Target, new Damage_Once(), 1.6f)).Shoot();
                GetBullet(7).Init(new BulletProjectile(d.Target,4f,new Vector3(-7,4),2), new BulletDataCommon(d.Target, new Damage_Once(), 1.6f)).Shoot();
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
                    GetBullet(12).Init(new BulletAimAwait(d.Target,p+new Vector3(5,d.index*1.3f),1.2f,6,8,Vector2.right,0.5f), new BulletDataCommon(d.Target, new Damage_Once(), 0.9f)).Shoot();
                    GetBullet(12).Init(new BulletAimAwait(d.Target,p+new Vector3(-5,d.index*1.3f),1.2f,6,8,Vector2.left,0.5f), new BulletDataCommon(d.Target, new Damage_Once(), 0.9f)).Shoot();
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
                    GetBullet(11).Init(new BulletStatic(d.Target,0.3f,3, d.Target.transform.position + new Vector3(i * 5, 0, 0)), new BulletDataCommon(d.Target, new Damage_Once(), 2.8f)).Shoot();
                }
            });
            AddEvent(1.5f, (d) =>
            {
                for (int i = -4; i < 4; i += 2)
                {
                    GetBullet(11).Init(new BulletStatic(d.Target, 0.3f, 3, d.Target.transform.position + new Vector3(i * 5, 0, 0)), new BulletDataCommon(d.Target, new Damage_Once(), 2.8f)).Shoot();
                }
            });
        }
    }
}