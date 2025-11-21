using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss14
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];

            if (lantern != null) lantern.ApplyEffect(new Burning(target, lantern, (int)(lantern.Shengming * 0.02f), 999999));
        }
        protected override void Repeat()
        {
            if (!lantern) return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target, target, 90, 1));
            else if(lantern.Shengming==1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target, t, 30, 1)));
        }
    }
    public class Skill0 : SkillBoss
    {
        private Lantern lantern;
        private EffectCollection ec;
        public Skill0(Target t) : base(t)
        {
            Description = "在靠近的敌人周围生成持续伤害雾气";
            TimeNeeded = 2f;
            cd = 10f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];

            ec = new EffectCollection(Target, (Effects.Slowness, 2f, 5f));
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null||lantern.Shengming<3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange().Select(t=>t.transform.position); // 获取范围内敌人
            foreach (var i in enemies) WarningCircle.Warn(i, 3, 2);
            AddEvent(2, (d) =>
            {
                foreach (var enemy in enemies)
                {
                    var bullet = GetBullet(4);
                    var trajectory = new BulletStatic(d.Target, 5f, 2f, enemy);
                    var data = new BulletDataSlight(d.Target, new Damage_Time(0.2f), 0.1f, ec);
                    bullet.Init(trajectory, data).Shoot();
                }
            });
        }
    }

    // 技能1：追踪式火球弹幕（自动瞄准最近敌人）
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;

        public Skill1(Target t) : base(t)
        {
            Description = "向靠近的敌人发射追踪火球";
            TimeNeeded = 2.5f;
            cd = 7f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3)&&Target.GetNearestEnemy(20);
        }

        protected override void OnUseSkill()
        {
            var nearestEnemy = Target.GetNearestEnemy(); // 获取最近敌人
            if (nearestEnemy == null) return;

            // 分3波发射火球，每波3颗
            for (int wave = 0; wave < 3; wave++)
            {
                AddEvent(wave * 0.6f, (d) => // 每0.6秒一波
                {
                    if (nearestEnemy == null) return;
                    Vector3 enemyPos = nearestEnemy.transform.position;
                    GetBullet(12).Init(
                            new BulletAim(d.Target, 4f, Target.transform.position, 6f, enemyPos, 0.7f),
                            new BulletDataCommon(d.Target, new Damage_Once(), 2.2f)).Shoot();
                });
            }
        }
    }

    public class Skill2 : SkillBoss
    {
        private Lantern lantern;

        public Skill2(Target t) : base(t)
        {
            Description = "在密集敌人区域引发能量爆发";
            TimeNeeded = 3f;
            cd = 12f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemy = Target.GetNearestEnemy();
            Vector3 centerPos = enemy.transform.position;

            WarningCircle.Warn(centerPos, 4f, 2f);

            AddEvent(2f, (d) =>
            {
                for (int i = 0; i < 8; i++)
                {
                    GetBullet(4).Init(
                        new BulletFromTo(d.Target, 0.5f,centerPos+Angle2Vector(i*45), centerPos + Angle2Vector(i * 45+180), 0.8f), 
                        new BulletDataCommon(d.Target, new Damage_Once(), 3)).Shoot();
                }
            });
        }
    }

    // 技能3：诅咒标记（给敌人叠加debuff）
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;
        private EffectCollection ec;
        public Skill3(Target t) : base(t)
        {
            Description = "标记靠近的敌人，使其受到持续伤害并降低防御";
            TimeNeeded = 2f;
            cd = 15f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];

            ec = new EffectCollection(Target, 
                (Effects.Burning, Target.effectController.GetFloatingAttributes().Gongji.Value * 0.5f, 10), 
                (Effects.DefenseDecrease, 0.2f, 10));
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange().Select(t=>t.transform.position);
            foreach (var i in enemies) WarningCircle.Warn(i, 2f, 1);
            AddEvent(1, (d) =>
            {
                foreach (var enemy in enemies)
                {
                    var bullet = GetBullet(16); // 16:烟火（特效）
                    var trajectory = new BulletStatic(d.Target, 10f, 2f, enemy);
                    var data = new BulletDataCommon(d.Target, new Damage_VFXOnly(), 0.5f,ec:ec); // 纯特效
                    bullet.Init(trajectory, data).Shoot();
                }
            });
        }
    }

    // 技能4：延迟陷阱（敌人经过时触发）
    public class Skill4 : SkillBoss
    {
        private Lantern lantern;
        public Skill4(Target t) : base(t)
        {
            Description = "在敌人路径上生成延迟陷阱，触发后造成范围伤害";
            TimeNeeded = 2.5f;
            cd = 9f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();
            List<Vector3>pos=new List<Vector3>();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                // 预测敌人移动路径，在前方2单位生成陷阱
                Vector3 trapPos = enemy.transform.position + (Vector3)enemy.GetComponent<Rigidbody2D>().velocity.normalized * 5f;

                WarningCircle.Warn(trapPos, 2f, 1f);
                pos.Add(trapPos);
            }
            AddEvent(1f, (d) =>
            {
                foreach(var i in pos)
                    GetBullet(11).Init(new BulletStatic(d.Target, 0.5f, 2f, i), new BulletDataCommon(d.Target, new Damage_Once(), 2.4f)).Shoot();
            });
        }
    }
    public class Skill5 : Common.Skill5For14_18
    {
        public Skill5(Target t) : base(t)
        {
            cd = 21f;
            restoreTime = 0.2f;
        }
        protected override void GetLantern()
        {
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
    }
}