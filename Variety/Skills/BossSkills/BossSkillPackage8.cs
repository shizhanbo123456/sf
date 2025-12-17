using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss8
{
    public class RepeatBoss : RepeatContent
    {
        public RepeatBoss() : base()
        {
        }
        public override void Repeat(Target target)
        {
            bool f = false;
            foreach(var i in Lantern.Lanterns.Values) 
                if (!i.Alive)
                {
                    f=true; 
                    break;
                }
            if (f)
            {
                foreach(var i in target.GetEnemyInRange())
                {
                    i.ApplyEffect(new Speed(target.ObjectId, i, 6, 1));
                    i.ApplyEffect(new JumpBoost(target.ObjectId, i, 10, 1));
                    i.ApplyEffect(new Stoic(target.ObjectId, i, 1, 1));
                }
            }
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "突进追击";
            Tag = "单体、突进";
            Description = "锁定最近敌人方向，自身向目标方向突进，同时发射一枚跟随自身的子弹，对路径上敌人造成伤害";
            TimeNeeded = 2f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var v = t ? t.transform.position - Target.transform.position : (Target.transform.position + Target.Front);
            Target.ApplyMotion(new MotionDir(v.normalized * 20, 1f, true, 1));
            var b = GetBullet(4);
            b.Init(2f,liftstoiclevel:0);
            BulletFollowSystem.RegistObject(b,4f,1f, Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "引力双生";
            Tag = "范围、控制";
            Description = "先发射一枚具有引力效果的持续伤害子弹，1秒后再发射一枚向下倾斜90度的引力子弹，双重吸附并持续打击近距离敌人";
            TimeNeeded = 2f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(5,false)!=null;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var b = GetBullet(5);
            b.Init(0.05f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletStaticScaleChangeSystem.RegistObject(b,5f,2f,1f);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            AddEvent(1, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.05f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
                BulletAngleSystem.RegistObject(b,2f,1f,10f,-90);
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "精准锁敌";
            Tag = "单体、精准";
            Description = "锁定最近敌人位置，发射一枚高速子弹直扑目标，精准打击单个敌人";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var b = GetBullet(7);
            b.Init(0.5f, liftstoiclevel: 0);
            BulletAimSystem.RegistObject(b,1.2f,3f,Target.transform.position,12,t?t.transform.position:Target.transform.position+Target.Front);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "圣佑环爆";
            Tag = "全屏、增益、控制";
            Description = "3秒蓄力期间生成逐渐扩大后缩小的巨型弹幕，蓄力结束后为范围内队友附加50点伤害提升（持续30秒），同时发射4枚沿轨道旋转的麻痹弹幕，最后触发二次巨型爆炸弹幕";
            TimeNeeded = 3f;
            cd = 60f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(4);
            b.Init(0f);
            BulletStaticScaleChangeSystem.RegistObject(b,15f,0f,3f);
            b.Shoot();
            AddEvent(3f, (d) =>
            {
                foreach(var i in Target.GetPartnerInRange())
                {
                    i.ApplyEffect(new DamageBoost(Target.ObjectId, i, 50, 30));
                }
                for (int startangle = 0; startangle <= 270; startangle += 90)
                {
                    var b_ = GetBullet(4);
                    b_.Init(1.2f,ec: new EffectCollection(d.Target, (EffectType.Paralysis, 0, 3)));
                    BulletOrbitSystem.RegistObject(b_,0.5f,10f,4f,90f,startangle);
                    BulletDamageOnceSystem.Regist(b_);
                    b_.Shoot();
                }
                var _b = GetBullet(4);
                _b.Init(1.5f);
                BulletStaticScaleChangeSystem.RegistObject(_b,0f,15f,3f);
                BulletDamageOnceSystem.Regist(_b);
                _b.Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "黏滞干扰";
            Tag = "范围、debuff";
            Description = "锁定最近敌人方向，向目标及左右±20度范围发射9枚散射子弹，命中敌人后附加黏滞效果（持续3秒）和20点精准度降低（持续5秒）";
            TimeNeeded = 3f;
            cd = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var angle = t?Dt2Degree(t.transform.position - Target.transform.position):(Target.FaceRight?0:180);
            for(int i = -20; i <= 20; i += 5)
            {
                var b = GetBullet(7);
                b.Init(1.4f, liftstoiclevel: 0,ec: new EffectCollection(Target, (EffectType.Sticky, 0, 3), (EffectType.AccuracyDecrease, 20, 5)));
                BulletAngleNonFacingSystem.RegistObject(b,0.6f,5f,8f,angle+i);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(5, 0);
            Name = "天坠流星";
            Tag = "范围、爆发";
            Description = "5秒内连续发射15枚抛物线子弹，呈弧形轨迹从天而降，覆盖8范围敌人区域，造成高额群体伤害";
            TimeNeeded = 5f;
            cd = 35f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(8,false).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for(int i = 0; i < 15; i++)
            {
                AddEvent(i * 0.2f, new TimeLineData(Target,i),(d) =>
                {
                    float angle = -12 * Mathf.Deg2Rad * d.index;
                    var t = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle))*6 + d.Target.transform.position;
                    var b = GetBullet(5);
                    b.Init(3.5f);
                    BulletProectileAimSystem.RegistObject(b,0.4f,1.5f,d.Target.transform.position,Vector3.up*55,t,1.2f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}