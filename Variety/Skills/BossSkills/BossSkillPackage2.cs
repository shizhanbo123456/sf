using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss2
{
    public class RepeatBoss2 : RepeatContent
    {
        public RepeatBoss2(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            int c = 0;
            foreach(var i in Lantern.Lanterns.Values)
            {
                if (i.TimeOfDie <0.1f) c++;
            }
            target.ApplyEffect(new AgileBoost(target.ObjectId, target, c * 10, 1));
            target.ApplyEffect(new AttackBoost(target.ObjectId, target, c * 0.1f, 1));
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int i = 0; i < 8; i++)
            {
                AddEvent(i * 0.3f,new TimeLineData(Target,i), (d) =>
                {
                    var t = d.Target.GetNearestEnemy(99999, false);
                    Vector3 startpos = d.Target.transform.position + new Vector3(Mathf.Cos(d.index * 0.785f), Mathf.Sin(d.index * 0.785f) + 1) * 4;
                    var b = GetBullet(7);
                    b.Init(0.7f,liftstoiclevel:0);
                    BulletAimSystem.RegistObject(b,0.3f,4,startpos,10,t.transform.position);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 1f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t=Target.GetNearestEnemy();
            Target.ApplyMotion(new MotionDir((t.transform.position - Target.transform.position).normalized * 20, 1.5f, true, 1));
            var b = GetBullet(4);
            b.Init(2.5f);
            BulletFollowSystem.RegistObject(b,4f,1.5f,Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            AddEvent(1.2f, (d) =>
            {
                var angle=Dt2Degree(t.transform.position- d.Target.transform.position);
                for(int offset = -15; offset <= 15; offset++)
                {
                    var b = GetBullet(7);
                    b.Init(1.5f);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,2,20,angle+offset);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill2 : SkillBoss
    {
        private EffectCollection eff;
        public Skill2() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 20f;

        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var b = GetBullet(4);
            b.Init(0.2f,liftstoiclevel:0,ec: new EffectCollection(Target, (EffectType.Burning, Target.DedicatedAttributes.Gongji * 0.5f, 10f)));
            BulletStaticScaleChangeSystem.RegistObject(b,0f,15f,1f);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(12,false).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(2.2f, true, 1));
            for(int i= 0; i < 6; i++)
            {
                var pos = Target.transform.position + Vector3.up*2+10*new Vector3(Mathf.Cos(3.14f/6f*i),Mathf.Sin(3.14f/6f*i));
                AddEvent(0.2f * i,new TimeLineData(Target,pos), (d) =>
                {
                    WarningCircle.Warn(d.pos, 2.5f, 1f);
                });
                AddEvent(0.2f * i+1f, new TimeLineData(Target,pos),(d) =>
                {
                    var b = GetBullet(3);
                    b.Init(1.5f);
                    BulletStaticSystem.RegistObject(b,2.5f,0.5f,d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
                pos = Target.transform.position + Vector3.up * 2 + 10 * new Vector3(Mathf.Cos(3.14f / 6f * i + 3.14f), Mathf.Sin(3.14f / 6f * i + 3.14f));
                AddEvent(0.2f * i, new TimeLineData(Target, pos), (d) =>
                {
                    WarningCircle.Warn(d.pos, 2.5f, 1f);
                });
                AddEvent(0.2f * i + 1f, new TimeLineData(Target, pos), (d) =>
                {
                    var b = GetBullet(3);
                    b.Init(1.5f);
                    BulletStaticSystem.RegistObject(b, 2.5f, 0.5f, d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool CanUse(Target Target)
        {
            var b = false;
            foreach(var l in Lantern.Lanterns.Values)
            {
                if (l.GetEnemyInRange(5, false).Count > 0)
                {
                    b = true; 
                    break;
                }
            }
            return b;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(2f, true, 1));
            for (int a = -1; a < 2; a++)
            {
                AddEvent(a * 0.4f, new TimeLineData(Target,a),(d) =>
                {
                    var List=Lantern.Lanterns.Values.ToList();
                    for (int i = 0; i < List.Count; i++)
                    {
                        var rad = (Dt2Degree(List[i].transform.position - d.Target.transform.position) + d.index * 5) * Mathf.Deg2Rad;
                        WarningRect.Warn(d.Target.transform.position, d.Target.transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * 60, 0.7f, 1.2f);
                    }
                });
                AddEvent(a * 0.4f + 1.2f, new TimeLineData(Target,a),(d) =>
                {
                    var List = Lantern.Lanterns.Values.ToList();
                    for (int i = 0; i < List.Count; i++)
                    {
                        var angle = Dt2Degree(List[i].transform.position - d.Target.transform.position);
                        var b = GetBullet(7);
                        b.Init(1.2f);
                        BulletAngleSystem.RegistObject(b,0.7f,3,20,angle+d.index*5);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
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
            cd = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            WarningCircle.Warn(Target.transform.position, 12, 0.6f);
            Target.ApplyMotion(new MotionStatic(3f, true, 1));
            for (int i = 0; i < 6; i++)
            {
                AddEvent(i * 0.2f + 0.6f,(d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.2f,liftstoiclevel:2,ec: new EffectCollection(d.Target, (EffectType.Stun, 0, 3f)));
                    BulletStaticScaleChangeSystem.RegistObject(b,0f,12f,1f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}