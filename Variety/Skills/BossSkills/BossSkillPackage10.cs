using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss10
{
    public class Skill0 : SkillBoss
    {
        private static List<Vector3> startVelocity = new List<Vector3>()
        {
            new Vector3(0,4),
            new Vector3(0,6),
            new Vector3(0,9),
        };
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "烟花散射";
            Tag = "烟花散射";
            Description = "向正面发射三枚不同初速度的烟火子弹，呈抛物线轨迹飞行";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        public override bool Detect(Target target)
        {
            return target.GetEnemyInRect(20, 3, true).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            foreach (var v in startVelocity) {
                var b = GetBullet(16);
                b.Init(1.2f);
                BulletProectileSystem.RegistObject(b, 0.8f, 3f, Target.transform.position, v+front*20);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "近身爆破";
            Tag = "近身爆破";
            Description = "生成警告圈，1.2秒后触发爆炸，同时自身获得攻击加成";
            TimeNeeded = 0.5f;
            cd = 22f;
        }
        public override bool Detect(Target target)
        {
            return target.GetEnemyInRange(3f).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 3, 1.2f);
            AddEvent(1.2f, (d) =>
            {
                var b = GetBullet(11);
                b.Init(1.2f);
                BulletStaticSystem.RegistObject(b,3f,0.6f,d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                Target.ApplyEffect(new EffectCollection(d.Target.ObjectId, (EffectType.AttackBoost, 0.3f, 10f)));
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "俯射弹幕";
            Tag = "俯射弹幕";
            Description = "先向上跃起，悬停并向斜下方发射5枚子弹";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target target)
        {
            return target.GetEnemyInRect(12, 3, true).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Vector2.up*12, 0.5f, true, 1));
            AddEvent(0.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.5f, true, 1));
                for(int i = -15; i >= -75; i -= 15)
                {
                    var b = GetBullet(7);
                    b.Init(0.5f);
                    BulletAngleSystem.RegistObject(b,0.3f,1f,15f,i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "双轨环绕";
            Tag = "双轨环绕";
            Description = "召唤两枚环绕自身的子弹，持续10秒";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target target)
        {
            return target.GetEnemyInRange(10).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(7);
            b.Init(0.6f,liftstoiclevel:0);
            BulletOrbitSystem.RegistObject(b,0.7f,10,6,120,0);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            b = GetBullet(7);
            b.Init(0.6f, liftstoiclevel: 0);
            BulletOrbitSystem.RegistObject(b, 0.7f, 10, 6, 120, 180);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "五向制导";
            Tag = "五向制导";
            Description = "生成矩形警告区域，1.2秒后从自身位置发射5枚制导子弹，命中目标区域";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            WarningRect.Warn(Target.transform.position, Target.transform.position + front * 20, 4, 1.2f);
            AddEvent(1.2f,new TimeLineData(Target, Target.transform.position + front * 20) ,(d) =>
            {
                for(int j=0;j<5;j++)
                {
                    var b = GetBullet(7);
                    b.Init(0.6f);
                    BulletProectileAimSystem.RegistObject(b,0.7f,2f,d.Target.transform.position,BulletSystemCommon.AngleToVector(j*72f)*20, d.pos,1.4f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "俯冲重击";
            Tag = "俯冲重击";
            Description = "先向上跃起，1秒后锁定最近敌人并高速俯冲，撞击地面触发大范围爆炸";
            TimeNeeded = 2f;
            cd = 40f;
        }
        public override bool Detect(Target target)
        {
            return target.GetNearestEnemy() != null;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Vector2.up * 10, 1f, true, 1));
            AddEvent(1f, (d) =>
            {
                var t = Target.GetNearestEnemy();
                d.Target.ApplyMotion(new MotionDir((t.transform.position - d.Target.transform.position) * 4, 0.25f, true, 1));
            });
            AddEvent(1.25f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.3f, true, 1));
                var b = GetBullet(11);
                b.Init(4f);
                BulletStaticSystem.RegistObject(b,3f,0.3f,d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
}