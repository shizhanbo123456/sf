using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.PackageC
{
    public class Skill0 : SkillNonCD
    {
        public Skill0(Target t) : base(t, Tool.SpriteManager.SkillPackageC[0])
        {
            Name = "上下开弓";
            Description = "召唤两发子弹射向前方，耗魔15";
            Tag = "平a";
            TimeNeeded = 0.3f;
            cost = 15;
        }
        protected override void OnUse()
        {
            Target.ApplyMotion(new MotionStatic(0.2f, true, 1, true, true));
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1,0);
            GetBullet(7).Init(new BulletFromTo(Target,0.5f,Target.transform.position+Vector3.up,Target.transform.position+Vector3.down+front*10,0.6f), new BulletDataSlight(Target, new Damage_Once(), 0.8f)).Shoot();
            GetBullet(7).Init(new BulletFromTo(Target,0.5f,Target.transform.position+Vector3.down,Target.transform.position+Vector3.up+front*10,0.6f), new BulletDataSlight(Target, new Damage_Once(), 0.8f)).Shoot();
        }
    }
    public class Skill1 : SkillStorable
    {
        public Skill1(Target t) : base(t, Tool.SpriteManager.SkillPackageC[1])
        {
            Name = "后撤步";
            Description = "迅速向后位移，期间中幅提升减伤";
            Tag = "平a";
            TimeNeeded = 0.5f;
            cost = 00;
            MaxstoreTime = 2;
            storeTime = 2;//初始储存
            CD = 10f;
        }
        protected override void OnUse()
        {
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1, 0);
            Target.ApplyMotion(new MotionDir(front*-20,0.25f,true,1,true,true));
            Target.effectController.AddEffect(new ArmorFortity(Target, Target, 50, 0.5f));
        }
    }
    public class Skill2 : SkillCD
    {
        public Skill2(Target t) : base(t, Tool.SpriteManager.SkillPackageC[2])
        {
            Name = "挥符";
            Description = "向前发射三颗子弹，耗魔100";
            Tag = "平a";
            TimeNeeded = 0.5f;
            cost = 100;
            CD = 2f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1, 0);
            var pos = Target.transform.position;
            GetBullet(14).Init(new BulletProjectileAim(Target,1.5f,Target.transform.position,Vector3.up*5,pos+front*10,1,0.8f), new BulletDataCommon(Target, new Damage_Once(), 2.2f)).Shoot();
            GetBullet(14).Init(new BulletProjectileAim(Target,1.5f,Target.transform.position,Vector3.zero,pos+front*10,1,0.8f), new BulletDataCommon(Target, new Damage_Once(), 2.2f)).Shoot();
            GetBullet(14).Init(new BulletProjectileAim(Target,1.5f,Target.transform.position,Vector3.down*5,pos+front*10,1,0.8f), new BulletDataCommon(Target, new Damage_Once(), 2.2f)).Shoot();
        }
    }
    public class Skill3 : SkillCD
    {
        public Skill3(Target t) : base(t, Tool.SpriteManager.SkillPackageC[3])
        {
            Name = "守卫者";
            Description = "召唤一圈子弹围绕自身，耗魔75";
            Tag = "平a";
            TimeNeeded = 0.2f;
            cost = 75;
            CD = 20f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1, 0);
            for(int i = 0;i<4;i++)
                GetBullet(7).Init(new BulletOrbit(Target,10f,2,360,i*90,0.4f), new BulletDataCommon(Target, new Damage_Time(0.5f), 1.6f)).Shoot();
        }
    }
    public class Skill4 : SkillNonCD
    {
        private static readonly List<float> offset = new List<float>()
        {
            4.1f,
            5.0f,
            4.5f,
            4.2f,
            6.4f,
            6.8f,
            5.4f,
            5.9f,
            5.4f,
            7.2f,
            5.7f,
            7.5f,
            4.0f,
            5.2f,
            6.8f,
            7.9f,
            7.3f,
            4.8f,
            5.2f,
            5.0f,
            7.7f,
            4.6f,
            6.2f,
            5.1f,
            5.6f,
            4.2f,
            4.1f,
            6.7f,
            5.5f,
            6.2f,
            6.8f,
            7.2f,
            4.3f,
            5.3f,
            6.9f,
            7.4f,
            6.7f,
            4.4f,
            5.4f,
            6.2f,
        };
        public Skill4(Target t) : base(t, Tool.SpriteManager.SkillPackageC[4])
        {
            Name = "子弹风暴";
            Description = "前方地面不断喷发出子弹，耗魔150";
            Tag = "平a";
            TimeNeeded = 0.6f;
            cost = 150;
        }
        protected override void OnUse()
        {
            var front = Target.FaceRight ? 1:-1;
            var pos = Target.transform.position;
            for (int i = 0; i < offset.Count; i++)
            {
                int j = i;
                AddEvent(j * 0.05f, new TimeLineData(Target,front,pos),(d) =>
                {
                    Vector3 start = new Vector3(d.index,0) + d.pos * offset[j] + Vector3.down * 2;
                    GetBullet(7).Init(new BulletFromTo(d.Target, 0.5f, start,start + Vector3.up*10,0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 0.3f)).Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillCD
    {
        public Skill5(Target t) : base(t, Tool.SpriteManager.SkillPackageC[5])
        {
            Name = "诅咒";
            Description = "向周围10m内的敌人施加诅咒";
            Tag = "平a";
            TimeNeeded = 2f;
            cost = 0;
            CD = 25f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            GetBullet(4).Init(new BulletStaticScaleChange(Target,0.5f,10), new BulletDataSlight(Target, new Damage_VFXOnly(), 0.8f)).Shoot();
            foreach (var i in Target.GetEnemyInRange(10, false))
            {
                i.ApplyEffect(new BadLuck(Target, i, 20, 10f));
            }
        }
    }
}