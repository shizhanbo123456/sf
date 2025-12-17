using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss11
{
    public class RepeatBoss : RepeatContent
    {
        public RepeatBoss() : base()
        {
        }
        public override void Repeat(Target target)
        {
            Lantern lantern=null;
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[^1];
            else return;
            if (lantern.Alive)
            {
                foreach (var p in target.GetEnemyInRange())
                {
                    p.ApplyEffect(new ArmorFortity(target.ObjectId, p, 20, 1));
                }
                target.ApplyEffect(new Luck(target.ObjectId, target, 10, 1));
            }
            else
            {
                target.ApplyEffect(new AttackDecrease(target.ObjectId, target, 0.5f, 1));
            }
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "坠击爆破";
            Tag = "单体、爆发";
            Description = "锁定最近敌人位置，先在目标上方生成纵向预警区域，1秒后发射子弹俯冲至目标位置，随后触发范围爆炸造成二次伤害";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t!=null?t.transform.position:Target.transform.position;
            WarningRect.Warn(p, p + Vector3.up * 5, 1.3f, 1f);
            AddEvent(1, new TimeLineData(Target,p),(d) =>
            {
                var b = GetBullet(7);
                b.Init(0.8f);
                BulletFromToSystem.RegistObject(b,0.4f,0.3f, d.pos + Vector3.up * 5, d.pos);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
            AddEvent(1.3f, new TimeLineData(Target,p),(d) =>
            {
                var b = GetBullet(11);
                b.Init(0.8f);
                BulletStaticSystem.RegistObject(b,1f,0.5f,d.pos);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "霰弹雪球";
            Tag = "范围、压制";
            Description = "4秒蓄力期间持续发射30枚子弹，子弹沿多角度抛物线轨迹锁定最近敌人，形成密集弹幕覆盖目标区域";
            TimeNeeded = 4f;
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for (int i = 0; i < 30; i++)
            {
                int j = i;
                AddEvent(i * 0.1f, (d) =>
                {
                    float a = (j * 129) % 90 + 45;
                    Vector3 v = new Vector3(Mathf.Cos(a), Mathf.Sin(a));
                    var t = Target.GetNearestEnemy();
                    var p = t != null ? t.transform.position : Target.transform.position;
                    var b = GetBullet(10);
                    b.Init(0.6f,liftstoiclevel:0);
                    BulletProectileAimSystem.RegistObject(b,0.6f,1.5f,d.Target.transform.position, v * 30, p,1f);
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
            sprite = new Vector2Int(2, 0);
            Name = "横向雾袭";
            Tag = "范围、封锁";
            Description = "在最近敌人位置生成横向大范围预警区域，警告1秒后发射雾型子弹从左至右横扫，封锁横向移动路径";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t != null ? t.transform.position : Target.transform.position;
            WarningRect.Warn(p + Vector3.left * 5, p + Vector3.right * 5, 2, 1f);
            AddEvent(1f, new TimeLineData(Target,p),(d) =>
            {
                var b = GetBullet(13);
                b.Init(1.5f);
                BulletFromToSystem.RegistObject(b, 1,0.7f,d.pos + Vector3.left * 5, d.pos + Vector3.right * 5);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "交叉雾网";
            Tag = "范围、控制";
            Description = "在最近敌人位置生成两组交叉的预警区域，1秒后发射两枚雾型子弹沿交叉轨迹穿行，形成网状封锁区域";
            TimeNeeded = 0.5f;
            cd = 7f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t != null ? t.transform.position : Target.transform.position;
            WarningRect.Warn(p + Angle2Vector(45)*5, p + Angle2Vector(225)*5, 2, 1f);
            WarningRect.Warn(p + Angle2Vector(-45)*5, p + Angle2Vector(135)*5, 2, 1f);
            AddEvent(1,new TimeLineData(Target,p), (d) =>
            {
                var b = GetBullet(15);
                b.Init(1.4f);
                BulletFromToSystem.RegistObject(b, 1,0.5f, d.pos + Angle2Vector(45)*5, d.pos + Angle2Vector(225)*5);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                b = GetBullet(15);
                b.Init(1.4f);
                BulletFromToSystem.RegistObject(b, 1, 0.5f, d.pos + Angle2Vector(135) * 5, d.pos + Angle2Vector(-45) * 5);
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
            Name = "环形火雨";
            Tag = "全屏、高额伤害";
            Description = "在最近敌人周围生成环形分布的18个预警区域，2.5秒后发射20枚火球子弹，沿环形交叉轨迹撞击目标区域，每枚造成3.6倍高额伤害";
            TimeNeeded = 0.5f;
            cd = 28f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t != null ? t.transform.position : Target.transform.position;
            for (int i = 0; i < 18; i++)
            {
                WarningRect.Warn(p + Angle2Vector(i * 18 + 45)*10, p + Angle2Vector(i * 18 + 225)*10, 0.2f, 2.5f - 0.1f * i);
            }
            AddEvent(2.5f, new TimeLineData(Target,p),(d) =>
            {
                for (int i = 0; i < 20; i++)
                {
                    var b = GetBullet(12);
                    b.Init(3.6f);
                    BulletFromToSystem.RegistObject(b, 1, 0.5f, d.pos + Angle2Vector(i * 18 + 45)*9, d.pos + Angle2Vector(i * 18 + 225)*9);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        private static List<(Vector3, Vector3)> Pos = new List<(Vector3, Vector3)>()
        {
            (new Vector3( -9 , -11 ),new Vector3( -2 , 13 )),
            (new Vector3( 7 , -5 ),new Vector3( 10 , 18 )),
            (new Vector3( -12 , 8 ),new Vector3( -7 , -3 )),
            (new Vector3( -11 , 11 ),new Vector3( -5 , -9 )),
            (new Vector3( -18 , 3 ),new Vector3( 4 , -4 )),
            (new Vector3( -5 , -5 ),new Vector3( -14 , -17 )),
            (new Vector3( 4 , 10 ),new Vector3( -15 , -15 )),
            (new Vector3( -17 , -6 ),new Vector3( -11 , -16 )),
            (new Vector3( 1 , -13 ),new Vector3( 20 , -19 )),
            (new Vector3( -7 , -17 ),new Vector3( 0 , 15 )),
            (new Vector3( -14 , -19 ),new Vector3( -20 , 4 )),
            (new Vector3( 7 , -10 ),new Vector3( 4 , 14 )),
            (new Vector3( 0 , 9 ),new Vector3( 13 , 10 )),
            (new Vector3( 2 , -7 ),new Vector3( -16 , -20 )),
            (new Vector3( -3 , -4 ),new Vector3( 8 , 17 )),
            (new Vector3( -2 , 0 ),new Vector3( -6 , -17 )),
            (new Vector3( 3 , -4 ),new Vector3( 5 , 20 )),
            (new Vector3( 4 , -16 ),new Vector3( -18 , -6 )),
            (new Vector3( -13 , -18 ),new Vector3( 3 , 12 )),
            (new Vector3( 8 , 3 ),new Vector3( 4 , -8 )),
            (new Vector3( -20 , -17 ),new Vector3( -5 , 12 )),
            (new Vector3( -15 , 1 ),new Vector3( -20 , 2 )),
            (new Vector3( 3 , -12 ),new Vector3( -12 , -18 )),
            (new Vector3( 5 , 19 ),new Vector3( -19 , 7 )),
            (new Vector3( -2 , 18 ),new Vector3( 14 , 16 )),
            (new Vector3( -9 , 17 ),new Vector3( -13 , 3 )),
            (new Vector3( 8 , 15 ),new Vector3( -14 , 8 )),
            (new Vector3( -7 , 16 ),new Vector3( 5 , -8 )),
            (new Vector3( 17 , 5 ),new Vector3( 4 , -5 )),
            (new Vector3( 10 , 11 ),new Vector3( 7 , 2 )),
            (new Vector3( 4 , 9 ),new Vector3( 12 , 11 )),
            (new Vector3( 0 , 17 ),new Vector3( -13 , -4 )),
            (new Vector3( 20 , 10 ),new Vector3( 5 , 10 )),
            (new Vector3( -18 , -8 ),new Vector3( 2 , -11 )),
            (new Vector3( -11 , -14 ),new Vector3( -14 , 12 )),
            (new Vector3( -11 , 3 ),new Vector3( -16 , -18 )),
            (new Vector3( -17 , 3 ),new Vector3( -17 , 8 )),
            (new Vector3( -17 , -15 ),new Vector3( -13 , -3 )),
            (new Vector3( 7 , -19 ),new Vector3( -12 , -7 )),
            (new Vector3( 20 , -17 ),new Vector3( -8 , 5 )),
        };
        public Skill5() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "乱序弹幕";
            Tag = "乱序";
            Description = "4秒蓄力后，在最近敌人周围生成40个随机轨迹的警告区域，3秒后发射20枚距离型子弹沿预设轨迹突袭，每枚造成2.5倍伤害";
            TimeNeeded = 4f;
            cd = 25f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t != null ? t.transform.position : Target.transform.position;
            for (int i = 0; i < 40; i++)
            {
                WarningRect.Warn(p + Pos[i].Item1*0.5f, p + Pos[i].Item2*0.5f, 0.2f, 3);
            }
            AddEvent(3, new TimeLineData(Target,p),(d) =>
            {
                for (int i = 0; i < 20; i++)
                {
                    var b = GetBullet(7);
                    b.Init(2.5f);
                    BulletFromToSystem.RegistObject(b, 0.2f,0.2f,d.pos + Pos[i].Item1*0.5f, d.pos + Pos[i].Item2 * 0.5f );
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
}