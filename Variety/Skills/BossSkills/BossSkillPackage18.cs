using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Skill.Common;

namespace Variety.Skill.Boss18
{
    public class Skill0 : SkillCommonFor14_18
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "能量扩散";
            Tag = "范围";
            Description = "在最近敌人位置生成多层扩散能量环";
            TimeNeeded = 2.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy();
            Vector3 enemyPos = nearestEnemy?nearestEnemy.transform.position:Target.transform.position;
            WarningCircle.Warn(enemyPos, 3f, 0.8f);
            // 生成3层能量环，逐层扩散
            for (int i = 0; i < 3; i++)
            {
                AddEvent(i * 0.8f,new TimeLineData(Target,i), (d) =>
                {
                    float radius = 3f + (d.index * 2f);
                    var b = GetBullet(6);
                    b.Init(4);
                    BulletStaticScaleChangeSystem.RegistObject(b,radius*0.5f,radius,1f,enemyPos);
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
            sprite = new Vector2Int(4, 0);
            Name = "焰火追击";
            Tag = "多体、压制";
            Description = "向场上所有敌人发射多枚火弹";
            TimeNeeded = 2f;
            cd = 6f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange().Select(t=> 
            {
                var p = t.transform.position;
                return Target.transform.position+(p-Target.transform.position).normalized*30; 
            });
            foreach (var i in enemies) WarningRect.Warn(Target.transform.position, i, 1f, 0.4f);

            for (int i = 0; i < 4; i++)
            {
                foreach (var enemy in enemies)
                {
                    AddEvent(0.4f + i * 0.3f, new TimeLineData(Target,enemy),(d) =>
                    {
                        var b = GetBullet(16);
                        b.Init(0.5f);
                        BulletAimSystem.RegistObject(b, 0.5f, 2f, d.Target.transform.position, 7f,d.pos);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    });
                }
            }
        }
    }
    public class Skill2 : SkillCommonFor14_18
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "禁锢压制";
            Tag = "压制、范围";
            Description = "在最近的敌人处生成牵引力场";
            TimeNeeded = 3f;
            cd = 15f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var e = Target.GetNearestEnemy();
            var p=e?e.transform.position:Target.transform.position;
            WarningCircle.Warn(p, 12f, 1.5f);
            if(e!=null)e.ApplyEffect(new Slowness(Target.ObjectId, e, 3, 3f));
            // 1.5秒后生成禁锢场
            AddEvent(1.5f, new TimeLineData(Target,p),(d) =>
            {
                var b = GetBullet(3);
                b.Init(0.1f, hitback: (b, t) => Bullet.FigureAttractForce(b,t));
                BulletStaticScaleChangeSystem.RegistObject(b,0,12,6,d.pos);
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
        }
    }

    public class Skill3 : SkillCommonFor14_18
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "预瞄爆破";
            Tag = "多体";
            Description = "预判所有敌人的位置并造成爆破";
            TimeNeeded = 2.5f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            if (enemies.Count == 0) enemies.Add(Target);
            foreach (var i in enemies)
            {
                WarningCircle.Warn(i.transform, 2, 1f);
                AddEvent(1f, new TimeLineData(i),(d) =>
                {
                    var rb = d.Target.GetComponent<Rigidbody2D>();
                    Vector2 velocity = rb ? rb.velocity : Vector2.zero;
                    Vector3 trapPos = d.Target.transform.position + (Vector3)rb.velocity;

                    var b = GetBullet(11);
                    b.Init(2);
                    BulletStaticSystem.RegistObject(b, 2f, 0.5f, trapPos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill4 : SkillCommonFor14_18
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "炎狱灼烧弹";
            Tag = "多体";
            Description = "向所有敌人发射附加灼烧效果的子弹";
            TimeNeeded = 3f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            foreach (var e in enemies)
            {
                WarningCircle.Warn(e.transform.position, 4, 2);
                AddEvent(2f, new TimeLineData(Target, e.transform.position),(d) =>
                {
                    var b = GetBullet(11);
                    b.Init(2, ec:new EffectCollection(d.Target, (EffectType.Burning, Target.effectController.GetFloatingAttributes().Gongji.Value * 0.05f, 4f)));
                    BulletStaticSystem.RegistObject(b, 4f, 0.5f, d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : Skill5For14_18
    {
        public Skill5() : base()
        {
            cd = 72f;
        }
    }
}