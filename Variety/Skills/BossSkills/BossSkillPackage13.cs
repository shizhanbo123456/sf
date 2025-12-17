using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss13
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "散射弹幕";
            Tag = "远程、多段伤害";
            Description = "锁定最近敌人方向，发射3道散射弹幕，弹幕呈±5°角度分布，覆盖中等范围";
            TimeNeeded = 0.5f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var angle = Dt2Degree(Target.GetNearestEnemy().transform.position - Target.transform.position);
            for (int i = -5; i <= 5; i += 5)
            {
                var r = (angle + i) * Mathf.Deg2Rad;
                WarningRect.Warn(Target.transform.position, Target.transform.position + new Vector3(Mathf.Cos(r), Mathf.Sin(r))*90, 0.5f, 1f);
            }
            AddEvent(1, new TimeLineData(Target,(int)angle),(d) =>
            {
                for (int i = -5; i <= 5; i += 5)
                {
                    var b = GetBullet(14);
                    b.Init(0.5f,liftstoiclevel:0);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,6,15,d.index+i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "范围爆破";
            Tag = "AOE、持续伤害";
            Description = "锁定最近敌人位置，生成5个渐进式爆破弹，从中心向外扩张，造成范围伤害";
            TimeNeeded = 2f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var pos = t != null ? t.transform.position : Target.transform.position;
            WarningCircle.Warn(pos, 2.5f, 1.1f);
            for (int i = 0; i < 5; i++)
            {
                AddEvent(1 + 0.1f * i,new TimeLineData(Target,pos), (d) =>
                {
                    var b = GetBullet(6);
                    b.Init(1.5f);
                    BulletStaticScaleChangeSystem.RegistObject(b,0,2.5f,0.5f,d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "精准冲击";
            Tag = "单体、高伤害";
            Description = "锁定最近敌人方向，发射一枚大型穿透弹，沿直线高速飞行，造成高额单体伤害";
            TimeNeeded = 2f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t != null ? t.transform.position : Target.transform.position;
            var angle = Dt2Degree(p - Target.transform.position);
            var b = GetBullet(6);
            b.Init(4.5f);
            BulletAngleNonFacingSystem.RegistObject(b, 2f, 6, 15, angle);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "纵向切割";
            Tag = "区域封锁、持续伤害";
            Description = "在目标区域生成9道纵向切割线，从上方贯穿至下方，覆盖大范围区域，封锁敌人移动";
            TimeNeeded = 1.5f;
            cd = 15f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t != null ? t.transform.position : Target.transform.position;
            for (int i = -20; i <= 20; i += 5)
            {
                WarningRect.Warn(p + Vector3.up * 20+Vector3.right*i, p - Vector3.up * 20 + Vector3.right * i, 1.5f, 1);
            }
            AddEvent(1, new TimeLineData(Target,p),(d) =>
            {
                for (int i = -20; i <= 20; i += 5)
                {
                    var b = GetBullet(10);
                    b.Init(3.5f);
                    BulletFromToSystem.RegistObject(b,0.75f,0.6f,d.pos + Vector3.up * 20 + Vector3.right * i, d.pos- Vector3.up * 20 + Vector3.right * i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "双重攻势";
            Tag = "增益、混合伤害";
            Description = "自身获得30%攻击力提升（持续20秒），先发射环绕轨道弹封锁区域，后续切换为高速追踪弹进行收割";
            TimeNeeded = 10f;
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyEffect(new AttackBoost(Target.ObjectId, Target, 0.3f, 20));
            for (float i = 0; i < 4.9f; i += 0.5f)
            {
                AddEvent(i, (d) =>
                {
                    var b = GetBullet(12);
                    b.Init(0.2f);
                    BulletOrbitSystem.RegistObject(b,0.2f,5,2,420,0);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
            for (float i = 0; i < 4.9f; i += 0.5f)
            {
                AddEvent(i + 5.5f, (d) =>
                {
                    var t = d.Target.GetNearestEnemy();
                    var p = Dt2Degree(t != null ?( t.transform.position - d.Target.transform.position):d.Target.Front);
                    var b = GetBullet(12);
                    b.Init(0.6f);
                    BulletAngleNonFacingSystem.RegistObject(b,0.2f,2,40,p);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "弹幕雨";
            Tag = "全屏、压制";
            Description = "1秒内发射30枚抛物线弹幕，呈环形覆盖目标区域，弹幕带有重力下坠效果，形成压制性火力";
            TimeNeeded = 3f;
            cd = 40f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for (int j = 0; j < 30; j++)
            {
                AddEvent(j * 0.1f,new TimeLineData(Target,j), (d) =>
                {
                    float a = d.index * 147f * Mathf.Deg2Rad;
                    Vector3 v = new Vector3(Mathf.Cos(a), Mathf.Sin(a));
                    var t = Target.GetNearestEnemy();
                    var p = t != null ? t.transform.position : Target.transform.position;
                    var b = GetBullet(7);
                    b.Init(1.5f);
                    BulletProectileAimSystem.RegistObject(b,0.8f,1.5f,d.Target.transform.position, v * 20, p,1f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}