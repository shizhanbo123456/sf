using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=400
    public class Follow : ShootActBase
    {
        public override ushort minId => 400;
        public override ushort maxId => 400;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletFollowSystem.RegistObject(b, info.radius, info.lifeTime);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}