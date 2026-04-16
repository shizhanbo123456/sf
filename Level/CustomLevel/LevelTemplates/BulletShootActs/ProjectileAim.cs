using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class ProjectileAim : ShootActBase
    {
        public static ModSpace3 mod = new(15, 10, 20);//3000
        public override ushort minId => 32000;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2, out var v3);

            Execute(shooter, info, v1 + 1, v2 * 36, v3*0.5f+0.5f);
        }
        public void Execute(Target shooter, BulletInfo info, float speed,float angle,float hitTime)
        {
            UnityEngine.Vector3 vstart = BulletSystemCommon.AngleToVector(angle) * speed;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletProjectileAimSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position,
                vstart, t ? t.transform.position : shooter.transform.position, hitTime);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}