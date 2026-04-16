using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class Projectile : ShootActBase
    {
        public static ModSpace2 mod = new(25, 20);//500
        public override ushort minId => 31000;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2);

            Execute(shooter, info, v1*4-48, v2 * 2);
        }
        public void Execute(Target shooter, BulletInfo info, float offsetX,float offsetY)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletProjectileSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position, new UnityEngine.Vector3(offsetX,offsetY));
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}