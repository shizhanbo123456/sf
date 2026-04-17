using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class AngleNonFacing : ShootActBase
    {
        public static ModSpace2 mod = new(180, 5);//900
        public override ushort minId => 1000;
        public override ushort maxId => (ushort)(minId + mod.TotalSize);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2);

            Execute(shooter, info, v2 * 4 + 4, v1 * 2);
        }
        public void Execute(Target shooter, BulletInfo info, float speed, float angle)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            if (!shooter.FaceRight) angle = 180 - angle;
            var b = GetBullet(info.graphicType);
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletAngleNonFacingSystem.RegistObject(b, info.radius, info.lifeTime, speed, angle);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class AngleNonFacing2 : ShootActBase
    {
        public static ModSpace2 mod = new(180, 5);//900
        public override ushort minId => 2000;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2);

            Execute(shooter, info, v2 * 4 + 4, v1 * 2);
        }
        public void Execute(Target shooter, BulletInfo info, float speed, float angle)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy(15,true);
            if (t)
            {
                angle+= BulletSystemCommon.VectorToAngle(t.transform.position - shooter.transform.position);
            }
            else angle = 180 - angle;
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletAngleNonFacingSystem.RegistObject(b, info.radius, info.lifeTime,speed, angle);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}