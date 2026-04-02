using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public abstract class ShootActBase
    {
        public abstract short minId { get; }
        public abstract short maxId { get; }
        public abstract void Act(Target shooter, BulletInfo info,short id);
        /// <summary>
        /// 0:爆炸 1:火焰 2:普通 3:刀光 4:狼形 5:骷髅
        /// </summary>
        protected static Bullet GetBullet(int index)
        {
            return Tool.BulletManager.GetBullet(index);
        }
    }
}