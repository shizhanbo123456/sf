using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=14000-14799,初始角度=百位*45，初始速度=十位*2+2，命中时间=个位*0.3+1
    public class ProjectileAim : ShootActBase
    {
        public override short minId => 14000;
        public override short maxId => 14799;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int tid = id - minId;
            int timeFactor = tid % 10;
            tid /= 10;
            int speedFactor= tid % 10;
            tid /= 10;
            int angleFactor = tid % 10;
            UnityEngine.Vector3 vstart=BulletSystemCommon.AngleToVector(angleFactor * 45)*(speedFactor*2+2);
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletProjectileAimSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position,
                vstart,t?t.transform.position:shooter.transform.position,timeFactor*0.3f+1);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}