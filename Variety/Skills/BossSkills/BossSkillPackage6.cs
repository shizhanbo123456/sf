using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss6
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            Description = "";
            TimeNeeded = 3f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 3, true, 1));
            for(int offset = -10; offset <= 10; offset+=5)
            {
                var b = GetBullet(5);
                b.Init(1f);
                BulletAngleSystem.RegistObject(b,2f,3f,10f,offset);
                BulletDamageTimeSystem.Regist(b,0.5f);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            Description = "";
            TimeNeeded = 2;
            cd = 15f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(10,true).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(20, true);
            if (!t) return;
            var p = t.transform.position;
            WarningRect.Warn(Target.transform.position, (p - Target.transform.position).normalized * 20 + Target.transform.position, 1, 1);
            AddEvent(1f, new TimeLineData(Target,p),(d) =>
            {
                d.Target.ApplyMotion(new MotionDir((d.pos - d.Target.transform.position).normalized * 10, 2, true, 1));
                var b = GetBullet(6);
                b.Init(3.5f);
                BulletFollowSystem.RegistObject(b, 3f, 2f, Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 1, true, 1));
            var b = GetBullet(6);
            b.Init(2.5f);
            BulletFollowSystem.RegistObject(b,3f,4f,Target);
            BulletDamageTimeSystem.Regist(b,0.5f);
            b.Shoot();
            AddEvent(1f, new TimeLineData(Target,front),(d) =>
            {
                d.Target.ApplyMotion(new MotionDir(d.pos * -10, 1, true, 1));
            });
            AddEvent(2f, new TimeLineData(Target, front), (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(d.pos * 10, 1, true, 1));
            });
            AddEvent(3f, new TimeLineData(Target, front), (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(d.pos * -10, 1, true, 1));
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 10f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 2, true, 1));
            var b = GetBullet(5);
            b.Init(0.2f, hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletFollowSystem.RegistObject(b,4f,2f,Target);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            AddEvent(2, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.2f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
                BulletDirSystem.RegistObject(b,4,1f,15f,new Vector2(1,1));
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = Target.FaceRight ? 1 : -1;
            var p = Target.transform.position;
            WarningRect.Warn(p - front * 15*Vector3.right, p + front * 15 * Vector3.right, 6, 2);
            for (int i = 0; i < 20; i++)
            {
                int j = i;
                AddEvent(2+i*0.1f,new TimeLineData(Target,front,p), (d) =>
                {
                    Vector3 offset = new Vector3(0, (j * 0.726f) % 1*8-4);
                    var b = GetBullet(13);
                    b.Init(0.5f,liftstoiclevel:0);
                    BulletFromToSystem.RegistObject(b,1f,3f,d.pos-d.index*15*Vector3.right+offset, d.pos + d.index* 15*Vector3.right + offset);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 50f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(1.5f, true, 1));
            int r = Target.FaceRight ? 1 : -1;
            AddEvent(0.5f,new TimeLineData(Target,r) ,(d) =>
            {
                var b = GetBullet(5);
                b.Init(0.05f, hitback: (b, t) => Bullet.FigureAttractForce(b, t));
                BulletStaticScaleChangeSystem.RegistObject(b, 4,0, 1.2f, Target.transform.position + 5f * d.index * Vector3.right);
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
            AddEvent(1.5f,new TimeLineData(Target,r), (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(10f * d.index * Vector3.right, 2, true, 1));
                var b = GetBullet(5);
                b.Init(5f,liftstoiclevel:2,ec: new EffectCollection(d.Target, (EffectType.ArmorShatter, 30, 10)));
                BulletFollowSystem.RegistObject(b,4f,2f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
}