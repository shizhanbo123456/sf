using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=900
    public class Follow : ShootActBase
    {
        public override short minId => 900;
        public override short maxId => 900;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletFollowSystem.RegistObject(b, info.radius, info.lifeTime);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}