using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss15
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];

            if (lantern != null) lantern.ApplyEffect(new Burning(target, lantern, (int)(lantern.Shengming * 0.02f), 999999));
        }
        protected override void Repeat()
        {
            if (!lantern) return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target, t, 30, 1)));
        }
    }

    // 技能0：环绕式能量弹幕
    public class Skill0 : SkillBoss
    {
        private Lantern lantern;

        public Skill0(Target t) : base(t)
        {
            Description = "在靠近的敌人周围生成环绕能量弹";
            TimeNeeded = 2.5f;
            cd = 8f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                Vector3 enemyPos = enemy.transform.position;

                for (int i = 0; i < 6; i++)
                {
                    if (enemy == null) return;
                    var b = GetBullet(4);
                    b.Init(1.6f,liftstoiclevel:0);
                    BulletOrbitWorldSystem.RegistObject(b,0.4f,7f,3f,120,i*60,enemyPos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;

        public Skill1(Target t) : base(t)
        {
            Description = "在敌人位置显示警告后引发爆炸";
            TimeNeeded = 2f;
            cd = 6f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var nearestEnemy = Target.GetNearestEnemy();
            if (nearestEnemy == null) return;

            Vector3 enemyPos = nearestEnemy.transform.position;

            WarningCircle.Warn(enemyPos, 5f, 1f);

            // 1秒后爆炸
            AddEvent(1f, (d) =>
            {
                var b = GetBullet(11);
                b.Init(5);
                BulletStaticSystem.RegistObject(b,4f,0.5f,enemyPos);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        private Lantern lantern;

        public Skill2(Target t) : base(t)
        {
            Description = "在敌人周围生成持续伤害雾气";
            TimeNeeded = 3f;
            cd = 12f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];
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
                Vector3 enemyPos = enemy.transform.position;
                WarningCircle.Warn(enemyPos, 4, 3f);
                pos.Add(enemyPos);
            }
            AddEvent(3f, (d) =>
            {
                foreach (var i in pos)
                {
                    var b = GetBullet(4);
                    b.Init(0.2f,hitback:(b,t)=>Bullet.HitBackBulletAttracitve(4,b,t));
                    BulletStaticSystem.RegistObject(b,6,4,i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;

        public Skill3(Target t) : base(t)
        {
            Description = "标记敌人并施加燃烧与防御降低";
            TimeNeeded = 2f;
            cd = 10f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3)&&Target.GetNearestEnemy(15);
        }

        protected override void OnUseSkill()
        {
            WarningCircle.Warn(Target.transform.position, 15, 5f);
            AddEvent(5, (d) =>
            {
                var enemies = Target.GetEnemyInRange(15);
                foreach (var enemy in enemies)
                {
                    enemy.ApplyEffect(new Burning(d.Target, enemy, (int)(enemy.Shengming * 0.03f), 5));
                    enemy.ApplyEffect(new DefenseDecrease(d.Target, enemy, 0.2f, 8));
                }
            });
        }
    }

    // 技能4：追踪火球弹幕
    public class Skill4 : SkillBoss
    {
        private Lantern lantern;

        public Skill4(Target t) : base(t)
        {
            Description = "向靠近的敌人发射追踪火球";
            TimeNeeded = 2.5f;
            cd = 7f;
            restoreTime = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3)&&Target.GetNearestEnemy(20);
        }

        protected override void OnUseSkill()
        {
            for (int i = 0; i < 12; i++)
            {
                AddEvent(i * 0.2f, (d) =>
                {
                    foreach (var enemy in Target.GetEnemyInRange())
                    {
                        if (enemy == null) continue;
                        float angle = Dt2Degree(enemy.transform.position - Target.transform.position);
                        var b = GetBullet(12);
                        b.Init(0.6f,liftstoiclevel:0);
                        BulletAngleNonFacingSystem.RegistObject(b,0.8f,2,15,angle);
                        BulletDamageOnceSystem.Regist(b);
                    }
                });
            }
        }
    }
    public class Skill5 : Common.Skill5For14_18
    {
        public Skill5(Target t) : base(t)
        {
            cd = 26f;
            restoreTime = 0.4f;
        }
        protected override void GetLantern()
        {
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];
        }
    }
}