using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public abstract class ShootActBase
    {
        public abstract ushort minId { get; }
        public abstract ushort maxId { get; }
        public abstract void Act(Target shooter, BulletInfo info,ushort id);
        /// <summary>
        /// 0-2:爆炸不可变色，3:魔法核,4:能量球,5:能量球(吸收),6:能量球(放射)<br></br>
        /// 7:距离,8:光点,9:魔法阵,10:雪球,11:爆炸,12:火球，13-15:雾,16:烟火<br></br>
        /// </summary>
        protected static Bullet GetBullet(int index)
        {
            return Tool.BulletManager.GetBullet(index);
        }
    }
}