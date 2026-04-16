using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class Angle : ShootActBase
    {
        public static ModSpace2 mod = new(180,5);//900
        public override ushort minId => 100;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1,out var v2);

            Execute(shooter, info, v2*4+4,v1*2);
        }
        public void Execute(Target shooter, BulletInfo info, float speed,float angle)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletAngleSystem.RegistObject(b, info.radius, info.lifeTime, speed,angle);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}