using AttributeSystem.Effect;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss9
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "三角散射";
            Tag = "范围、精准";
            Description = "锁定最近敌人方向，向目标及左右±15度方向发射3枚子弹，沿世界角度直线飞行，覆盖近距离区域";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var d = t?Dt2Degree(t.transform.position - Target.transform.position):(Target.FaceRight?0:180);
            for(int offset = -15; offset <= 15; offset += 15)
            {
                var b = GetBullet(7);
                b.Init(0.5f, liftstoiclevel: 0);
                BulletAngleNonFacingSystem.RegistObject(b,0.3f,6f,5f,d+offset);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "瘟疫爆破";
            Tag = "区域";
            Description = "短暂预警后释放高额伤害子弹并附带吸血效果，同时释放沉默持续伤害弹幕";
            TimeNeeded = 0.5f;
            cd = 40f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var p = Target.transform.position;
            WarningCircle.Warn(p, 3, 0.8f);
            AddEvent(0.8f, new TimeLineData(Target,p),(d) =>
            {
                var b = GetBullet(11);
                b.Init(3, liftstoiclevel: 2, ec: new EffectCollection(d.Target.ObjectId, (EffectType.LifeSteal, 0.5f, 10)));
                BulletStaticSystem.RegistObject(b, 3f, 0.5f, d.pos);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                b = GetBullet(6);
                b.Init(0.2f, liftstoiclevel: 0, ec: new EffectCollection(d.Target.ObjectId, (EffectType.Silence, 0, 1)));
                BulletStaticSystem.RegistObject(b, 3f, 0.5f, d.pos);
                BulletDamageTimeSystem.Regist(b, 0.5f);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "环形弹幕";
            Tag = "范围、抛物线";
            Description = "锁定最近敌人位置生成预警圈，1秒后向目标发射12枚抛物线子弹，呈360度环形分布，全面覆盖目标区域";
            TimeNeeded = 2f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t?t.transform.position:Target.transform.position+Target.Front;
            WarningCircle.Warn(p, 3, 1);
            AddEvent(1f, new TimeLineData(Target,p),(d) =>
            {
                for (int i = 0; i < 12; i++)
                {
                    float j = i * 30 * Mathf.Deg2Rad;
                    var b = GetBullet(4);
                    b.Init(0.8f);
                    BulletProectileAimSystem.RegistObject(b,0.4f,3f,d.Target.transform.position, 
                        new Vector3(Mathf.Cos(j), Mathf.Sin(j))*8, d.pos, 1.5f);
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
            sprite = new Vector2Int(3, 0);
            Name = "双重纵列冲击";
            Tag = "全屏、持续压制";
            Description = "先生成7列纵向预警区域，1秒后发射纵向贯穿子弹；2秒后生成8列纵向预警，3秒后再次发射子弹，双重封锁纵向走位";
            TimeNeeded =4f;
            cd = 25f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var p = Target.transform.position;
            for(int i = -7; i <= 7; i += 2)
            {
                WarningRect.Warn(p + new Vector3(i * 5, 10), p + new Vector3(i * 5, -30), 5, 1);
            }
            AddEvent(1f, new TimeLineData(Target,p),(d) =>
            {
                for(int i = -7; i <= 7; i += 2)
                {
                    var b = GetBullet(12);
                    b.Init(1.2f);
                    BulletFromToSystem.RegistObject(b,1.5f,4f,d.pos + new Vector3(i * 5, 10), d.pos + new Vector3(i * 5, -30));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
            AddEvent(2f,new TimeLineData(Target,p),(d) =>
            {
                for (int i = -8; i <= 8; i += 2)
                {
                    WarningRect.Warn(d.pos + new Vector3(i * 5, 10), d.pos + new Vector3(i * 5, -30), 5, 1);
                }
            });
            AddEvent(3f, new TimeLineData(Target,p),(d) =>
            {
                for (int i = -8; i <= 8; i+=2)
                {
                    var b = GetBullet(12);
                    b.Init(1.2f);
                    BulletFromToSystem.RegistObject(b, 1.5f, 4f, d.pos + new Vector3(i * 5, 10), d.pos + new Vector3(i * 5, -30));
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
            sprite = new Vector2Int(4, 0);
            Name = "弱化领域";
            Tag = "领域、debuff";
            Description = "自身周围生成6范围预警圈，1秒后爆发逐渐扩大的领域弹幕，命中敌人后降低其20%攻击力并附加10点厄运效果，持续10秒";
            TimeNeeded = 0.5f;
            cd = 20f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(6,false).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 6, 1f);
            AddEvent(1f, (d) =>
            {
                var b = GetBullet(6);
                b.Init(2.5f,liftstoiclevel:0,ec: new EffectCollection(d.Target.ObjectId, (EffectType.AttackDecrease, 0.2f, 10), (EffectType.BadLuck, 10, 10)));
                BulletStaticScaleChangeSystem.RegistObject(b,0f,6f,1f);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(5, 0);
            Name = "终焉裁决";
            Tag = "全屏、爆发、控制";
            Description = "生成30范围超大预警圈，5秒蓄力后触发，自身发射巨型爆炸弹幕，同时冻结所有敌人2秒";
            TimeNeeded = 0.5f;
            cd = 45f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 30, 5f);
            AddEvent(5f, (d)=>
            {
                var c = d.Target.GetEnemyInRange();
                foreach (var i in c)
                {
                    i.ApplyEffect(new EffectCollection(d.Target.ObjectId,(EffectType.Freeze, 0f, 2f)));
                }
                var b = GetBullet(3);
                b.Init(32f);
                BulletStaticScaleChangeSystem.RegistObject(b,0f,30f,1f);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
}