using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Variety.Base;
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
        public Skill0() : base()
        {
            Description = "在靠近的敌人周围生成持续伤害雾气";
            TimeNeeded = 2f;
            cd = 10f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];

            ec = new EffectCollection(Target, (Effects.Slowness, 2f, 5f));
        }

        public override bool CanUse(Target Target)
        {
            return (lantern == null||lantern.Shengming<3);
        }

        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange().Select(t=>t.transform.position); // 获取范围内敌人
            foreach (var i in enemies)
            {
                WarningCircle.Warn(i, 3, 2);
                AddEvent(2, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.4f, ec: ec);
                    BulletStaticSystem.RegistObject(b, 5, 2, d.pos);
                    BulletDamageTimeSystem.Regist(b, 0.2f);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;

        public Skill1():base()
        {
            Description = "向靠近的敌人发射追踪火球";
            TimeNeeded = 2.5f;
            cd = 7f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }

        public override bool CanUse(Target Target)
        {
            return (lantern == null || lantern.Shengming < 3)&&Target.GetNearestEnemy(20);
        }

        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy(); // 获取最近敌人
            if (nearestEnemy == null) return;

            // 分3波发射火球，每波3颗
            for (int wave = 0; wave < 3; wave++)
            {
                AddEvent(wave * 0.6f, (d) => // 每0.6秒一波
                {
                    if (nearestEnemy == null) return; 
                    var b = GetBullet(12);
                    b.Init(2.2f);
                    BulletAimSystem.RegistObject(b,0.7f,4f,Target.transform.position,6f,nearestEnemy.transform.position);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }

    public class Skill2 : SkillBoss
    {
        private Lantern lantern;

        public Skill2() : base()
        {
            Description = "在密集敌人区域引发能量爆发";
            TimeNeeded = 3f;
            cd = 12f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }

        public override bool CanUse(Target Target)
        {
            return (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemy = Target.GetNearestEnemy();
            Vector3 centerPos = enemy.transform.position;

            WarningCircle.Warn(centerPos, 4f, 2f);

            AddEvent(2f, (d) =>
            {
                for (int i = 0; i < 8; i++)
                {
                    var b = GetBullet(4);
                    b.Init(3);
                    BulletFromToSystem.RegistObject(b,0.8f,0.5f,centerPos+Angle2Vector(i*45), centerPos + Angle2Vector(i * 45+180));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }

    // 技能3：诅咒标记（给敌人叠加debuff）
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;
        private EffectCollection ec;
        public Skill3() : base()
        {
            Description = "标记靠近的敌人，使其受到持续伤害并降低防御";
            TimeNeeded = 2f;
            cd = 15f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];

            ec = new EffectCollection(Target, 
                (Effects.Burning, Target.effectController.GetFloatingAttributes().Gongji.Value * 0.5f, 10), 
                (Effects.DefenseDecrease, 0.2f, 10));
        }

        public override bool CanUse(Target Target)
        {
            return (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange().Select(t=>t.transform.position);
            foreach (var i in enemies)
            {
                WarningCircle.Warn(i, 2f, 1);
                AddEvent(1, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(16);
                    b.Init(0.2f, liftstoiclevel: 0, ec: ec);
                    BulletStaticSystem.RegistObject(b, 2f, 10f, d.pos);
                    BulletDamageTimeSystem.Regist(b, 0.2f);
                    b.Shoot();
                });
            }
        }
    }

    // 技能4：延迟陷阱（敌人经过时触发）
    public class Skill4 : SkillBoss
    {
        private Lantern lantern;
        public Skill4() : base()
        {
            Description = "在敌人路径上生成延迟陷阱，触发后造成范围伤害";
            TimeNeeded = 2.5f;
            cd = 9f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }

        public override bool CanUse(Target Target)
        {
            return (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                // 预测敌人移动路径，在前方2单位生成陷阱
                Vector3 trapPos = enemy.transform.position + (Vector3)enemy.GetComponent<Rigidbody2D>().velocity.normalized * 5f;

                WarningCircle.Warn(trapPos, 2f, 1f);
                AddEvent(1f, new TimeLineData(Target,trapPos),(d) =>
                {
                    var b = GetBullet(11);
                    b.Init(2.4f);
                    BulletStaticSystem.RegistObject(b, 2f, 0.5f, d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : Variety.Skill.Common.Skill5For14_18
    {
        public Skill5():base()
        {
            cd = 21f;
        }
        protected override void GetLantern()
        {
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
    }
}