using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class Follow : ShootActBase
    {
        public override ushort minId => 2999;//1
        public override ushort maxId => minId;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            Execute(shooter, info);
        }
        public void Execute(Target shooter, BulletInfo info)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletFollowSystem.RegistObject(b, info.radius, info.lifeTime);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}