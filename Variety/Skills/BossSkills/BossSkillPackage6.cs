using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss6
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "冲锋散射";
            Tag = "范围、突进";
            Description = "向面朝方向持续冲锋3秒，期间发射5枚呈±10度、±5度、0度分布的持续伤害子弹，覆盖前方大范围区域";
            TimeNeeded = 3f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 3, true, 1));
            for(int offset = -10; offset <= 10; offset+=5)
            {
                var b = GetBullet(5);
                b.Init(1f);
                BulletAngleSystem.RegistObject(b,2f,3f,10f,offset);
                BulletDamageTimeSystem.Regist(b,0.5f);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "锁敌突袭";
            Tag = "单体、精准";
            Description = "锁定20范围内最近敌人，生成直线预警区域，1秒后向目标方向突进并发射一枚跟随自身的高额伤害子弹，精准打击目标";
            TimeNeeded = 2;
            cd = 15f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(10,true).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy(20, true);
            var p = t?t.transform.position:(Target.transform.position+Target.Front);
            WarningRect.Warn(Target.transform.position, (p - Target.transform.position).normalized * 20 + Target.transform.position, 1, 1);
            AddEvent(1f, new TimeLineData(Target,p),(d) =>
            {
                d.Target.ApplyMotion(new MotionDir((d.pos - d.Target.transform.position).normalized * 10, 2, true, 1));
                var b = GetBullet(6);
                b.Init(3.5f);
                BulletFollowSystem.RegistObject(b, 3f, 2f, Target);
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
            Name = "往复突袭";
            Tag = "范围、持续压制";
            Description = "先向前突进，随后在1秒、2秒、3秒时交替反向和正向突进，同时发射一枚跟随自身的持续伤害子弹，反复拉扯并打击路径上敌人";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Target.Front * 10, 1, true, 1));
            var b = GetBullet(6);
            b.Init(2.5f);
            BulletFollowSystem.RegistObject(b,3f,4f,Target);
            BulletDamageTimeSystem.Regist(b,0.5f);
            b.Shoot();
            AddEvent(1f, new TimeLineData(Target),(d) =>
            {
                d.Target.ApplyMotion(new MotionDir(d.Target.Front * -10, 1, true, 1));
            });
            AddEvent(2f, new TimeLineData(Target), (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(d.Target.Front * 10, 1, true, 1));
            });
            AddEvent(3f, new TimeLineData(Target), (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(d.Target.Front * -10, 1, true, 1));
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "引力冲锋";
            Tag = "范围、控制";
            Description = "向前突进2秒，期间发射一枚跟随自身的引力持续伤害子弹吸附敌人，突进结束后额外发射一枚斜向上方的引力子弹，双重控制敌人";
            TimeNeeded = 0.5f;
            cd = 10f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Target.Front * 10, 2, true, 1));
            var b = GetBullet(5);
            b.Init(0.2f, hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletFollowSystem.RegistObject(b,4f,2f,Target);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            AddEvent(2, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.2f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
                BulletDirSystem.RegistObject(b,4,1f,15f,new Vector2(1,1));
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "横向弹幕墙";
            Tag = "全屏、封锁";
            Description = "生成横向超大范围预警区域，2秒后连续发射20枚横向穿行的雾型子弹，子弹高度随机分布，全面封锁横向移动空间";
            TimeNeeded = 0.5f;
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = Target.FaceRight ? 1 : -1;
            var p = Target.transform.position;
            WarningRect.Warn(p - 15*Vector3.right, p + 15 * Vector3.right, 6, 2);
            for (int i = 0; i < 20; i++)
            {
                AddEvent(2+i*0.1f,new TimeLineData(Target,i), (d) =>
                {
                    Vector3 offset = new Vector3(0, (d.index * 0.726f) % 1*8-4);
                    var b = GetBullet(13);
                    b.Init(0.5f,liftstoiclevel:0);
                    BulletFromToSystem.RegistObject(b,1f,3f,d.pos-d.Target.Front*15+offset, d.pos + d.Target.Front * 15 + offset);
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
            sprite = new Vector2Int(5, 0);
            Name = "引力破甲斩";
            Tag = "范围、破甲";
            Description = "短暂静止1.5秒，期间在侧方生成引力持续伤害子弹吸附敌人，随后向面朝方向突进并发射一枚高额伤害子弹，命中敌人附加30点破甲效果（持续10秒）";
            TimeNeeded = 0.5f;
            cd = 50f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(1.5f, true, 1));
            int r = Target.FaceRight ? 1 : -1;
            AddEvent(0.5f,new TimeLineData(Target,r) ,(d) =>
            {
                var b = GetBullet(5);
                b.Init(0.05f, hitback: (b, t) => Bullet.FigureAttractForce(b, t));
                BulletStaticScaleChangeSystem.RegistObject(b, 4,0, 1.2f, Target.transform.position + 5f * d.index * Vector3.right);
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
            AddEvent(1.5f,new TimeLineData(Target,r), (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(10f * d.index * Vector3.right, 2, true, 1));
                var b = GetBullet(5);
                b.Init(5f,liftstoiclevel:2,ec: new EffectCollection(d.Target.ObjectId, (EffectType.ArmorShatter, 30, 10)));
                BulletFollowSystem.RegistObject(b,4f,2f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
}