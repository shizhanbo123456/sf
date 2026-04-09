using System.Collections.Generic;
using LevelCreator.TargetTemplate;

namespace LevelCreator.Executer
{
    public struct BulletShootExecuter
    {
        private static List<LevelCreator.BulletShootTemplate.ShootActBase> ShootActs = new()
        {
            new LevelCreator.BulletShootTemplate.Aim(),
            new LevelCreator.BulletShootTemplate.Angle(),
            new LevelCreator.BulletShootTemplate.AngleNonFacing(),
            new LevelCreator.BulletShootTemplate.AngleNonFacing2(),
            new LevelCreator.BulletShootTemplate.Follow(),
            new LevelCreator.BulletShootTemplate.FromTo(),
            new LevelCreator.BulletShootTemplate.Orbit(),
            new LevelCreator.BulletShootTemplate.OrbitWorld(),
            new LevelCreator.BulletShootTemplate.Projectile(),
            new LevelCreator.BulletShootTemplate.ProjectileAim(),
            new LevelCreator.BulletShootTemplate.Static(),
            new LevelCreator.BulletShootTemplate.Static2(),
            new LevelCreator.BulletShootTemplate.StaticScaleChange(),
        };
        public static void ExecuteBulletShootAct(Target shooter, BulletInfo info, ushort id)
        {
            foreach (var i in ShootActs)
            {
                if (id >= i.minId && id <= i.maxId)
                {
                    i.Act(shooter, info, id);
                    return;
                }
            }
        }
    }
}