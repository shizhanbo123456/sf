using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class Aim:ShootActBase
    {
        public static ModSpace3 mod = new(4,5,5);
        public override ushort minId => 0;
        public override ushort maxId => (ushort)(minId + mod.TotalSize-1);
        public override void Act(Target shooter, BulletInfo info,ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1,out var v2,out var v3);

            Execute(shooter, info,v1*4+4,v2*5-10,v3*5-10);
        }
        public void Execute(Target shooter, BulletInfo info, float speed,float xOffset,float yOffset)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy(15,true);
            var p = t == null ? shooter.transform.position + shooter.Front*20 : t.transform.position;
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletAimSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position, speed, p);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}