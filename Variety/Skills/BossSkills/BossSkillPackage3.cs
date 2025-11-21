using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss3
{
    public class RepeatBoss3 : RepeatContent
    {
        public RepeatBoss3(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            int c = 0;
            foreach (var i in Lantern.Lanterns.Values)
            {
                if (i.TimeOfDie <0.1f) c++;
            }
            target.ApplyEffect(new DefenseBoost(target, target, c * 0.3f, 1));
            target.ApplyEffect(new ArmorFortity(target, target, c * 10, 1));
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for (int i = 0; i < 8; i++)
            {
                AddEvent(i* 0.3f,new TimeLineData(Target,i), (d) =>
                {
                    var t = d.Target.GetNearestEnemy(99999, false);
                    Vector3 startpos = d.Target.transform.position + new Vector3(Mathf.Cos(d.index * 0.785f), Mathf.Sin(d.index * 0.785f) + 1) * 4;
                    GetBullet(7).Init(new BulletAim(d.Target, 4, startpos, 10, t.transform.position, 0.3f), new BulletDataSlight(d.Target, new Damage_Once(), 0.7f)).Shoot();
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
            cd = 1f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            var t = Target.GetNearestEnemy(99999, false);
            Target.ApplyMotion(new MotionDir((t.transform.position - Target.transform.position).normalized * -10, 1f, true, 1, true, true));
            GetBullet(4).Init(new BulletFollow(Target, 1.5f, 4), new BulletDataCommon(Target, new Damage_Once(), 2.5f)).Shoot();
            AddEvent(1.2f, (d) =>
            {
                var angle = Dt2Degree(t.transform.position - Target.transform.position);
                for(int offset=-15;offset<=15;offset++)
                    GetBullet(7).Init(new BulletAngleNonFacing(d.Target, 2, 20, angle+offset, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 1.5f)).Shoot();
            });
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 20f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(4).Init(new BulletStaticScaleChange(Target, 1, 0, 20), new BulletDataAttract(Target,0.1f,0.1f,10)).Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 8f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(12, false).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(2.2f, true, 1, true, true));
            for (int i = 0; i < 6; i++)
            {
                AddEvent(0.2f * i, new TimeLineData(Target,i),(d) =>
                {
                    WarningRect.Warn(d.Target.transform.position + new Vector3(i * 10, 15), d.Target.transform.position + new Vector3(i * 10, -15),0.5f,1f);
                });
            }
            for (int i = 0; i < 6; i++)
            {
                AddEvent(0.2f * i + 1f, new TimeLineData(Target,i),(d) =>
                {
                    GetBullet(11).Init(new BulletFromTo(d.Target,0.5f, d.Target.transform.position + new Vector3(i * 10, 15), d.Target.transform.position + new Vector3(i * 10, -15),0.5f), new BulletDataCommon(d.Target, new Damage_Once(), 1.5f)).Shoot();
                });
            }
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            var b = false;
            foreach (var l in Lantern.Lanterns.Values)
            {
                if (l.GetEnemyInRange(5, false).Count > 0)
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(2f, true, 1, true, true));
            var List = Lantern.Lanterns.Values.ToList();
            for (int a = 0; a < 3; a++)
            {
                AddEvent(a * 0.4f, (d) =>
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        var List = Lantern.Lanterns.Values.ToList();
                        WarningCircle.Warn(List[i].transform.position, 3, 1);
                    }
                });
                AddEvent(a * 0.4f + 1.2f, (d) =>
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        GetBullet(4).Init(new BulletStatic(Target, 0.5f, 3, List[i].transform.position), new BulletDataCommon(Target, new Damage_Once(), 1.2f)).Shoot();
                    }
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        private EffectCollection eff;
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 20f;
            restoreTime = 1;

            eff = new EffectCollection(t, (Effects.Freeze, 0, 3f));
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            WarningCircle.Warn(Target.transform.position, 12, 0.6f);
            Target.ApplyMotion(new MotionStatic(3f, true, 1, true, true));
            for (int i = 0; i < 6; i++)
            {
                AddEvent(i * 0.2f + 0.6f, (d) =>
                {
                    GetBullet(4).Init(new BulletStaticScaleChange(d.Target, 1, 12, 0), new BulletDataStrike(d.Target, new Damage_Once(), 0.2f, ec: eff)).Shoot();
                });
            }
        }
    }
}