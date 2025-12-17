using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Common
{
    public class SkillCommonFor14_18 : SkillBoss
    {
        public SkillCommonFor14_18() : base()
        {
        }

        public override sealed bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(Mathf.Lerp(25, 5, Lantern.GetAverageHealth()));
        }
    }
    public class Skill5For14_18 : SkillBoss
    {
        private static List<Vector3> Pos1 = new List<Vector3>()
        {
            new Vector3(-3.8f,-0.2f),
            new Vector3(3.1f,6.6f),
            new Vector3(7.5f,1.3f),
            new Vector3(-8.4f,-5.0f),
            new Vector3(-1.6f,6.5f),
            new Vector3(-2.7f,3.3f),
            new Vector3(-2.3f,-5.6f),
            new Vector3(6.4f,-2.7f),
            new Vector3(-9.1f,-5.2f),
            new Vector3(-8.6f,1.7f),
        };
        private static List<Vector3> Pos2 = new List<Vector3>()
        {
            new Vector3(-5.2f,7.8f),
            new Vector3(5.9f,3.2f),
            new Vector3(2.4f,4.1f),
            new Vector3(-4.2f,6.5f),
            new Vector3(-5.8f,6.9f),
            new Vector3(4.8f,-7.9f),
            new Vector3(3.9f,-2.0f),
            new Vector3(3.6f,-2.7f),
            new Vector3(-1.9f,-5.4f),
            new Vector3(-8.8f,-2.4f),
        };
        private static List<Vector3> Pos3 = new List<Vector3>()
        {
            new Vector3(-2.3f,2.3f),
            new Vector3(4.1f,3.0f),
            new Vector3(9.2f,-7.1f),
            new Vector3(-4.3f,6.4f),
            new Vector3(-6.6f,5.3f),
            new Vector3(-0.3f,5.2f),
            new Vector3(-4.1f,5.1f),
            new Vector3(4.0f,-5.6f),
            new Vector3(-8.3f,0.4f),
            new Vector3(-9.8f,-4.2f),
        };

        public Skill5For14_18() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "裂墟湮灭";
            Tag = "多体、压制";
            Description = "在最近敌人位置造成多轮巨大爆炸";
            TimeNeeded = 2f;
            cd = 3f;
        }
        public override sealed bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(15) && Lantern.GetAverageHealth()<0.2f;
        }
        protected override void OnUse(Target Target,Vector3 pos,bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var p = t?t.transform.position:Target.transform.position;
            foreach (var i in Pos1)
            {
                WarningCircle.Warn(i + p, 3, 2);
            }
            AddEvent(0.5f, new TimeLineData(Target,p),(d) =>
            {
                foreach (var i in Pos2)
                {
                    WarningCircle.Warn(i + d.pos, 3, 2);
                }
            });
            AddEvent(1f, new TimeLineData(Target, p), (d) =>
            {
                foreach (var i in Pos3)
                {
                    WarningCircle.Warn(i + d.pos, 3, 2);
                }
            });
            AddEvent(2, new TimeLineData(Target,p),(d) =>
            {
                ShootAtPos(Pos1,d.pos);
            });
            AddEvent(2.5f, new TimeLineData(Target, p), (d) =>
            {
                ShootAtPos(Pos1,d.pos);
            });
            AddEvent(3, new TimeLineData(Target, p), (d) =>
            {
                ShootAtPos(Pos1, d.pos);
            });
        }
        private void ShootAtPos(List<Vector3>pos,Vector3 offset)
        {
            foreach (var p in pos)
            {
                var b = GetBullet(0);
                b.Init(1.5f);
                BulletStaticSystem.RegistObject(b, 3f, 0.5f, p+offset);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
}