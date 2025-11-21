using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss12
{
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.3f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(4, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = Target.FaceRight ? 1 : -1;
            GetBullet(7).Init(new BulletFollow(Target, 0.25f, 0.4f), new BulletDataCommon(Target, new Damage_Once(), 1.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(new Vector2(front * 20, 0), 0.25f, true, 1, true, true));
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(4, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(7).Init(new BulletAngle(Target, 1.5f, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.4f)).Shoot();
            GetBullet(7).Init(new BulletAngle(Target, 1.5f, 5, 10, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.4f)).Shoot();
            GetBullet(7).Init(new BulletAngle(Target, 1.5f, 5, -10, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.4f)).Shoot();
            AddEvent(0.5f, (d) =>
            {
                var front = d.Target.FaceRight ? 1 : -1;
                GetBullet(7).Init(new BulletFollow(d.Target, 0.25f, 0.4f), new BulletDataCommon(d.Target, new Damage_Once(), 2.5f)).Shoot();
                d.Target.ApplyMotion(new MotionDir(new Vector2(front * 30, 0), 0.25f, true, 1, true, true));
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
            for (int i = 0; i < 3; i++)
            {
                AddEvent(i * 0.3f, (d) =>
                {
                    GetBullet(4).Init(new BulletStaticScaleChange(d.Target, 0.3f, 3), new BulletDataSlight(d.Target, new Damage_Once(), 0.5f)).Shoot();
                });
            }
            Target.ApplyEffect(new Speed(Target, Target, 3f, 12f));
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(8, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(5).Init(new BulletStaticScaleChange(Target, 1f, 0, 4f), new BulletDataAttract(Target, 0.2f, 0.04f, 7.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(0.7f, true, 1, true, true));
            AddEvent(0.8f, (d) =>
            {
                var front = d.Target.FaceRight ? 1 : -1;
                GetBullet(14).Init(new BulletFollow(d.Target, 0.25f, 0.4f), new BulletDataStrike(d.Target, new Damage_Once(), 5f)).Shoot();
                Target.ApplyMotion(new MotionDir(new Vector2(front * 30, 0), 0.25f, true, 1, true, true));
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = Target.FaceRight ? 1 : -1;
            Target.ApplyMotion(new MotionDir(new Vector2(front * 20, 0), 1, true, 1, true, true));
            for (int i = 0; i < 4; i++)
            {
                int j = i;
                AddEvent(i * 0.25f, (d) =>
                {
                    GetBullet(14).Init(new BulletOrbit(d.Target, 0.25f, 0.5f, (i % 2 == 0) ? 360 : -360, (i % 2 == 0) ? -135 : 135, 1.3f), new BulletDataCommon(d.Target, new Damage_Once(), 1.2f)).Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 5f;
            cd = 32f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up * 40, 0.25f, true, 1, true, true));
            Vector3 p = Vector3.zero;
            AddEvent(0.25f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(5f, true, 1, true, true));
                p = d.Target.GetNearestEnemy(99999, false).transform.position;
                WarningCircle.Warn(p, 2, 0.6f);
            });
            for (int i = 0; i < 36; i++)
            {
                var angle = i * 20f * Mathf.Deg2Rad;
                AddEvent(0.9f + i * 0.1f, (d) =>
                {
                    GetBullet(7).Init(new BulletProjectileAim(d.Target, 2, d.Target.transform.position, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 20, p, 1, 0.7f), new BulletDataSlight(d.Target, new Damage_Once(), 0.5f)).Shoot();
                });
            }
        }
    }
}