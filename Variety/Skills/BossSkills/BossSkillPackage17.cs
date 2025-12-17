using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Skill.Common;

namespace Variety.Skill.Boss17
{
    public class RepeatBoss : RepeatContent
    {
        public RepeatBoss() : base()
        {
            dt = 1f;
        }
        public override void Repeat(Target target)
        {
            Lantern lantern = null;
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];
            else return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target.ObjectId, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target.ObjectId, t, 30, 1)));
        }
    }
    public class Skill0 : SkillCommonFor14_18
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "预判火球术";
            Tag = "单体、预判、精准";
            Description = "根据敌人移动速度预测未来位置，生成直线预警区域，0.8秒后发射火球沿预测路径飞行，精准打击移动中的敌人";
            TimeNeeded = 2.2f;
            cd = 7f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            List<(Vector3, Vector3)> pos = new List<(Vector3, Vector3)>();
            foreach (var enemy in enemies)
            {
                if (!enemy.TryGetComponent<Rigidbody2D>(out var rb)) continue;

                Vector3 predictPos = enemy.transform.position + (Vector3)(rb.velocity * 1.2f);
                Vector3 startPos = (Target.transform.position - enemy.transform.position).normalized * 20 + enemy.transform.position;
                pos.Add((startPos, predictPos));

                WarningRect.Warn(pos[^1].Item1, pos[^1].Item2, 1.2f, 0.8f);
            }
            foreach (var i in pos)
            {
                AddEvent(0.8f, (d) =>
                {
                    var b = GetBullet(12);
                    b.Init(2);
                    BulletFromToSystem.RegistObject(b, 0.6f, 0.5f, i.Item1, i.Item2);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillCommonFor14_18
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "极速地雷阵";
            Tag = "范围、预判、高额伤害";
            Description = "预测敌人移动路径，在其前方生成地雷预警圈，1秒后地雷激活，敌人移动速度越快，触发后受到的伤害越高";
            TimeNeeded = 2.5f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            List<Vector3>pos=new List<Vector3>();
            foreach (var enemy in enemies)
            {
                if (!enemy.TryGetComponent<Rigidbody2D>(out var rb)) continue;

                // 根据速度计算地雷生成位置
                Vector2 velocity = rb.velocity;
                float speed = velocity.magnitude;
                Vector3 spawnPos = enemy.transform.position + (Vector3)(velocity.normalized * Mathf.Min(speed * 0.8f, 12f));
                pos.Add(spawnPos);
            }
            foreach (var i in pos)
            {
                WarningCircle.Warn(i, 3, 1);
                AddEvent(1, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(6);
                    b.Init(3);
                    BulletStaticSystem.RegistObject(b, 2.5f, 1.5f, d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }

    public class Skill2 : SkillCommonFor14_18
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "禁锢减速环";
            Tag = "单体、控制、持续伤害";
            Description = "锁定最近敌人位置生成预警圈，1秒后触发禁锢环，持续5秒造成高频伤害，同时附加3点减速效果（持续5秒），限制敌人移动";
            TimeNeeded = 3f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy();
            Vector3 enemyPos = nearestEnemy?nearestEnemy.transform.position:Target.transform.position;
            WarningCircle.Warn(enemyPos, 3, 1);
            AddEvent(1,new TimeLineData(Target,enemyPos), (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.6f,liftstoiclevel:0, new EffectCollection(d.Target, (EffectType.Slowness, 3, 5)));
                BulletStaticSystem.RegistObject(b,3,5,d.pos);
                BulletDamageTimeSystem.Regist(b,0.3f);
                b.Shoot();
            });
        }
    }
    public class Skill3 : SkillCommonFor14_18
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "侧移封锁弹幕";
            Tag = "范围、封锁、预判";
            Description = "锁定敌人移动方向，5秒后分5波在敌人两侧发射弹幕，形成横向封锁线，阻断敌人前进路径";
            TimeNeeded = 2f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            if(enemies.Count==0)enemies.Add(Target);
            foreach (var i in enemies) WarningCircle.Warn(i.transform, 2f, 5f);
            foreach (var enemy in enemies)
            {
                if (!enemy.TryGetComponent<Rigidbody2D>(out var rb)) continue;

                // 每侧发射5颗子弹
                for (int i = 0; i < 5; i++)
                {
                    float delay = i * 0.2f;
                    // 左侧弹幕
                    AddEvent(5+delay,new TimeLineData(Target,rb.velocity), (d) =>
                    {
                        var dir=d.pos.magnitude > 0.1f ? d.pos.normalized : Vector3.right;
                        Vector3 side1 = new Vector2(-dir.y, dir.x);
                        Vector3 side2 = new Vector2(dir.y, -dir.x);
                        var b = GetBullet(7);
                        b.Init(0.6f);
                        BulletFromToSystem.RegistObject(b,0.4f,1f,enemy.transform.position + side1 * 5, enemy.transform.position - side1 * 2);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();

                        b = GetBullet(7);
                        b.Init(0.6f);
                        BulletFromToSystem.RegistObject(b, 0.4f, 1f, enemy.transform.position + side2 * 5, enemy.transform.position - side2 * 2);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    });
                }
            }
        }
    }

    public class Skill4 : SkillCommonFor14_18
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "变速追踪导弹";
            Tag = "多体、压制、跟随";
            Description = "2.8秒内分7波向范围内所有敌人发射追踪导弹，导弹速度随敌人移动状态动态调整，持续追击目标造成伤害";
            TimeNeeded = 2.8f;
            cd = 15f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            if (enemies.Count == 0) enemies.Add(Target);
            for (int i = 0; i < 7;i++)
            {
                AddEvent(i * 0.4f, (d) =>
                {
                    foreach (var enemy in enemies)
                    {
                        var b = GetBullet(7);
                        b.Init(0.8f);
                        BulletProectileAimSystem.RegistObject(b,0.4f,2f, d.Target.transform.position, Vector3.up * 20, enemy.transform.position,1.2f);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
                });
            }
        }
    }
    public class Skill5 : Skill5For14_18
    {
        public Skill5() : base()
        {
            cd = 55f;
        }
    }
}