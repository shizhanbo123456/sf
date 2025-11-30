using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Skill.Common;
using Variety.Template;

namespace Variety.Skill.Boss16
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];

            if (lantern != null) lantern.ApplyEffect(new Burning(target.ObjectId, lantern, (int)(lantern.Shengming * 0.02f), 999999));
        }
        protected override void Repeat()
        {
            if (!lantern) return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target.ObjectId, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target.ObjectId, t, 30, 1)));
        }
    }
    public class Skill0 : SkillCommonFor14_18
    {
        public Skill0() : base()
        {
            Description = "向靠近的敌人发射扩散能量球";
            TimeNeeded = 2f;
            cd = 6f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy();
            if (nearestEnemy == null) return;

            Vector3 enemyPos = nearestEnemy.transform.position;
            for (int i = 0; i < 4; i++) WarningRect.Warn(enemyPos + Angle2Vector(i * 30 + 45) * 10, enemyPos - Angle2Vector(i * 30 + 45) * 10,2.5f,0.5f);
            AddEvent(0.5f,new TimeLineData(Target,enemyPos), (d) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    var b = GetBullet(4);
                    b.Init(1.2f);
                    BulletFromToSystem.RegistObject(b,1.2f,2f,d.pos + Angle2Vector(i * 30 + 45) * 10, d.pos - Angle2Vector(i * 30 + 45) * 10);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill1 : SkillCommonFor14_18
    {
        public Skill1() : base()
        {
            Description = "在敌人路径生成烟雾陷阱，触发后造成持续伤害";
            TimeNeeded = 2.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                // 预测移动路径，在前方3单位生成陷阱
                Vector3 trapPos = enemy.transform.position + (Vector3)enemy.GetComponent<Rigidbody2D>().velocity * 1.5f;

                WarningCircle.Warn(trapPos, 4, 1f);
                AddEvent(1f, new TimeLineData(Target,trapPos),(d) =>
                {
                    var b = GetBullet(14);
                    b.Init(0.5f, liftstoiclevel: 0);
                    BulletStaticSystem.RegistObject(b, 3, 4, d.pos);
                    BulletDamageTimeSystem.Regist(b, 0.3f);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill2 : SkillCommonFor14_18
    {
        public Skill2() : base()
        {
            Description = "向单个敌人发射精准火球，附带击退";
            TimeNeeded = 1.8f;
            cd = 5f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var targetEnemy = Target.GetNearestEnemy();
            if (targetEnemy == null) return;

            Vector3 enemyPos = targetEnemy.transform.position;
            WarningCircle.Warn(enemyPos, 1f, 0.8f);

            // 0.8秒后发射火球
            AddEvent(0.8f, (d) =>
            {
                var b = GetBullet(12);
                b.Init(3f, liftstoiclevel: 2);
                BulletAimSystem.RegistObject(b,1.5f,2f,d.Target.transform.position,20,enemyPos);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill3 : SkillCommonFor14_18
    {
        public Skill3() : base()
        {
            Description = "在敌人周围生成诅咒环，附加燃烧与易伤";
            TimeNeeded = 3f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            List<Vector3>pos= new List<Vector3>();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                Vector3 enemyPos = enemy.transform.position;
                WarningCircle.Warn(enemyPos, 8f, 1.5f);
                pos.Add(enemyPos);
            }
            AddEvent(1.5f, (d) =>
            {
                foreach (var p in pos)
                {
                    var b = GetBullet(6);
                    b.Init(0.2f,liftstoiclevel: 0, new EffectCollection(d.Target, (EffectType.Burning, Target.effectController.GetFloatingAttributes().Gongji.Value, 8)));
                    BulletStaticScaleChangeSystem.RegistObject(b,0,8,5,p);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }

    public class Skill4 : SkillCommonFor14_18
    {
        public Skill4() : base()
        {
            Description = "向多个敌人发射密集弹幕";
            TimeNeeded = 2.2f;
            cd = 7f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            foreach (var i in Target.GetEnemyInRange()) WarningCircle.Warn(i.transform, 1.5f, 1);
            // 每个敌人生成5颗子弹，快速连续发射
            for (int i = 0; i < 5; i++)
            {
                AddEvent(0.8f+i * 0.15f, (d) =>
                {
                    foreach (var i in Target.GetEnemyInRange())
                    {
                        var b = GetBullet(7);
                        b.Init(0.9f);
                        BulletAimSystem.RegistObject(b,0.7f,2f,d.Target.transform.position,20,i.transform.position);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
                });
            }
        }
    }
    public class Skill5 : Common.Skill5For14_18
    {
        public Skill5() : base()
        {
            cd = 36f;
        }
    }
}