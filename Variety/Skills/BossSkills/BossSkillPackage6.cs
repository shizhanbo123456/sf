using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
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
            GetBullet(6).Init(new BulletAngle(Target, 3,10, 0, 3), new BulletDataSlight(Target, new Damage_Time(1f), 1.5f)).Shoot();
            GetBullet(6).Init(new BulletAngle(Target, 3,10, 5, 3), new BulletDataSlight(Target, new Damage_Time(1f), 1.5f)).Shoot();
            GetBullet(6).Init(new BulletAngle(Target, 3,10, -5, 3), new BulletDataSlight(Target, new Damage_Time(1f), 1.5f)).Shoot();
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
            GetBullet(6).Init(new BulletFollow(Target,2, 3), new BulletDataCommon(Target, new Damage_Once(), 3.5f)).Shoot();
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
            GetBullet(6).Init(new BulletFollow(Target, 4, 3), new BulletDataCommon(Target, new Damage_Once(), 3.5f)).Shoot();
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
            GetBullet(5).Init(new BulletFollow(Target, 2, 4), new BulletDataAttract(Target,0.2f,0.1f,10)).Shoot();
            AddEvent(2, (d) =>
            {
                GetBullet(5).Init(new BulletDir(d.Target,1,15,new Vector2(1,1),4), new BulletDataAttract(d.Target, 0.2f, 0.1f, 10)).Shoot();
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
                    GetBullet(7).Init(new BulletFromTo(d.Target,3, d.Target.transform.position-front*15+offset, d.Target.transform.position + front * 15 + offset,1), new BulletDataSlight(d.Target, new Damage_Once(), 0.5f)).Shoot();
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
            GetBullet(5).Init(new BulletStatic(Target,3,4,Target.transform.position+front*5f), new BulletDataAttract(Target,0.2f,0.05f,10f)).Shoot();
            Target.ApplyMotion(new MotionStatic(1, true, 1, true, true));
            AddEvent(1, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(front * 10, 2, true, 1, true, true));
                GetBullet(5).Init(new BulletFollow(d.Target, 2, 4), new BulletDataStrike(d.Target,new Damage_Once(),5f,ec:ec)).Shoot();
            });
        }
    }
}