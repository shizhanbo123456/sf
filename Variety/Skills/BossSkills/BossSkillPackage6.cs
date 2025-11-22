using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss6
{
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 3f;
            cd = 8f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 3, true, 1, true, true));
            for(int offset = -5; offset <= 5; offset++)
            {
                var b = GetBullet(7);
                b.Init(1f);
                BulletAngleSystem.RegistObject(b,3f,3f,10f,offset);
                BulletDamageTimeSystem.Regist(b,0.5f);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 15f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(10,true).Count>0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(20, true);
            if (!t) return;
            Target.ApplyMotion(new MotionDir((t.transform.position-Target.transform.position).normalized*10, 2, true, 1, true, true));
            var b = GetBullet(6);
            b.Init(3.5f);
            BulletFollowSystem.RegistObject(b,3f,2f,Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
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
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 1, true, 1, true, true));
            var b = GetBullet(6);
            b.Init(2.5f);
            BulletFollowSystem.RegistObject(b,3f,4f,Target);
            BulletDamageTimeSystem.Regist(b,0.5f);
            b.Shoot();
            AddEvent(1f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(front * -10, 1, true, 1, true, true));
            });
            AddEvent(2f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(front * 10, 1, true, 1, true, true));
            });
            AddEvent(3f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(front * -10, 1, true, 1, true, true));
            });
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
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 2, true, 1, true, true));
            var b = GetBullet(5);
            b.Init(0.2f,hitback:(b,t)=>Bullet.HitBackBulletAttracitve(10,b,t));
            BulletFollowSystem.RegistObject(b,4f,2f,Target);
            BulletDamageTimeSystem.Regist(b,0.2f);
            b.Shoot();
            AddEvent(2, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.2f,hitback:(b,t)=>Bullet.HitBackBulletAttracitve(10,b,t));
                BulletDirSystem.RegistObject(b,4,1f,15f,new Vector2(1,1));
                BulletDamageTimeSystem.Regist(b,0.2f);
                b.Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 30f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            WarningRect.Warn(Target.transform.position - front * 15, Target.transform.position + front * 15, 4, 2);
            for (int i = 0; i < 20; i++)
            {
                int j = i;
                AddEvent(2+i*0.1f, (d) =>
                {
                    Vector3 offset = new Vector3(0, (j * 0.726f) % 1*8-4);
                    var b = GetBullet(7);
                    b.Init(0.5f,liftstoiclevel:0);
                    BulletFromToSystem.RegistObject(b,1f,3f,d.Target.transform.position-front*15+offset, d.Target.transform.position + front * 15 + offset);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        private EffectCollection ec;
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 50f;
            restoreTime = 1;

            ec = new EffectCollection(Target, (Effects.ArmorShatter, 30, 10));
        }
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            var b = GetBullet(5);
            b.Init(0.05f,hitback:(b,t)=>Bullet.HitBackBulletAttracitve(10f,b,t));
            BulletStaticSystem.RegistObject(b,4,3,Target.transform.position+front*5f);
            BulletDamageTimeSystem.Regist(b,0.2f);
            b.Shoot();
            Target.ApplyMotion(new MotionStatic(1, true, 1, true, true));
            AddEvent(1, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(front * 10, 2, true, 1, true, true));
                var b = GetBullet(5);
                b.Init(5f,liftstoiclevel:2,ec:ec);
                BulletFollowSystem.RegistObject(b,4f,2f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
}