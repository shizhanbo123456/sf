using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=16011-16099，水平偏移=十位*5-25，垂直偏移=个位*5-25
    public class Static : ShootActBase
    {
        public override short minId => 16011;
        public override short maxId => 16099;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int tid = id - minId;
            int v = tid % 10;
            tid /= 10;
            int h = tid % 10;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletStaticSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position+new UnityEngine.Vector3(h*5-25,v*5-25));
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    //id=16111-16199，水平偏移=十位*5-25，垂直偏移=个位*5-25
    public class Static2 : ShootActBase
    {
        public override short minId => 16111;
        public override short maxId => 16199;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int tid = id - minId;
            int v = tid % 10;
            tid /= 10;
            int h = tid % 10;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            UnityEngine.Vector3 pos = t ? t.transform.position : shooter.transform.position;
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletStaticSystem.RegistObject(b, info.radius, info.lifeTime,pos + new UnityEngine.Vector3(h * 5 - 25, v * 5 - 25));
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}