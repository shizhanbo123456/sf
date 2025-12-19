using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss4
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "正弦散射";
            Tag = "范围、压制";
            Description = "锁定最近敌人方向，2秒内连续发射20枚子弹，子弹角度随正弦规律偏移±45度，形成波浪形弹幕覆盖目标区域";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var angle = t?Dt2Degree(t.transform.position - Target.transform.position):(Target.FaceRight?0:180);
            for(int i = 0; i < 20; i++)
            {
                AddEvent(0.1f * i, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.3f,liftstoiclevel:0);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,5,10, angle+Mathf.Sin(d.index*0.5f)*45);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "挑空突袭";
            Tag = "单体、连招";
            Description = "先将最近敌人向上击飞，0.7秒后向敌人方向高速突进，同时发射一枚跟随自身的高额伤害子弹，衔接后续打击";
            TimeNeeded = 0.5f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t=Target.GetNearestEnemy();
            t.ApplyMotion(new MotionDir(Vector2.up * 10, 0.7f, false, 1));
            var v = t.transform.position - Target.transform.position;
            AddEvent(0.7f, new TimeLineData(Target,v),(d) =>
            {
                d.Target.ApplyMotion(new MotionDir(d.pos.normalized*30,1,true,1)); 
                var b = GetBullet(12);
                b.Init(2.5f,liftstoiclevel:2);
                BulletFollowSystem.RegistObject(b,3f,1f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "瘟疫爆破";
            Tag = "区域、破甲";
            Description = "引爆自身位置，发射高额伤害子弹并附加55点破甲效果（持续10秒），同时释放大范围减速持续伤害弹幕，限制敌人走位";
            TimeNeeded = 0.5f;
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(11);
            b.Init(3f, liftstoiclevel: 2, ec: new EffectCollection(Target.ObjectId, (EffectType.ArmorShatter, 55f, 10f)));
            BulletStaticSystem.RegistObject(b, 3f, 0.5f, Target.transform.position);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            b = GetBullet(6);
            b.Init(0.2f, liftstoiclevel: 0, ec: new EffectCollection(Target.ObjectId, (EffectType.Slowness, 2f, 1f)));
            BulletStaticSystem.RegistObject(b, 0, 20f,Target.transform.position);
            BulletDamageTimeSystem.Regist(b, 0.5f);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "直线贯穿";
            Tag = "远程、封锁";
            Description = "锁定最近敌人方向生成超长直线预警区域，1秒后发射一枚贯穿全场的巨型子弹，封锁直线路径上的敌人";
            TimeNeeded = 0.5f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy().transform.position;
            WarningRect.Warn(Target.transform.position, (t - Target.transform.position).normalized * 60 + Target.transform.position, 3, 1f);
            AddEvent(1f, (d) =>
            {
                var b = GetBullet(12);
                b.Init(3.3f, liftstoiclevel: 2);
                BulletFromToSystem.RegistObject(b, 3f, 3,d.Target.transform.position, (t - d.Target.transform.position).normalized * 60 + d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "三角夹击";
            Tag = "单体、控制";
            Description = "在最近敌人位置生成预警圈，1秒后从三个斜向方向发射3枚子弹，交叉撞击目标区域，形成夹击封锁";
            TimeNeeded = 0.5f;
            cd = 14f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy().transform.position;
            WarningCircle.Warn(t,2,0.5f);
            AddEvent(1f, (d) =>
            {
                for(int i = 0; i < 3; i++)
                {
                    Vector3 v = new Vector3(Mathf.Cos(i * 120 * Mathf.Deg2Rad), Mathf.Sin(i * 120 * Mathf.Deg2Rad))*10;
                    var b = GetBullet(12);
                    b.Init(3.3f, liftstoiclevel: 2);
                    BulletFromToSystem.RegistObject(b,2,1,t+v,t-v);
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
            sprite = new Vector2Int(5, 0);
            Name = "引力领域";
            Tag = "范围、控制";
            Description = "在自身位置生成大范围引力持续伤害弹幕，吸附8范围内的敌人，限制其移动并持续造成伤害";
            TimeNeeded = 0.5f;
            cd = 30f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(8,false).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(4);
            b.Init(0.1f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletStaticSystem.RegistObject(b,8,4,Target.transform.position);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}