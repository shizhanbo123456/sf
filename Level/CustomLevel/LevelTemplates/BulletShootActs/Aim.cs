using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class Aim:ShootActBase
    {
        public static ModSpace1 mod = new(10);
        public override ushort minId => 0;
        public override ushort maxId => (ushort)(minId + mod.TotalSize-1);
        public override void Act(Target shooter, BulletInfo info,ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1);

            Execute(shooter, info,v1*6+3);
        }
        public void Execute(Target shooter, BulletInfo info, float speed)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var p = t == null ? shooter.transform.position + shooter.Front : t.transform.position;
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletAimSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position, speed, p);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}