namespace LevelCreator.Internal
{
    public class Package2
    {
        public static void Load()
        {
            //小子弹
            ExecutionBuilder.CreateBullet(400, 2, 0.3f, 1f, 0.05f);
            ExecutionBuilder.CreateBullet(401, 2, 0.3f, 1f, 0.15f);
            ExecutionBuilder.CreateBullet(402, 2, 0.3f, 1f, 0.3f);
            //大子弹
            ExecutionBuilder.CreateBullet(403, 2, 0.8f, 1.5f, 0.05f);
            ExecutionBuilder.CreateBullet(404, 2, 0.8f, 1.5f, 0.15f);
            ExecutionBuilder.CreateBullet(405, 2, 0.8f, 1.5f, 0.3f);
            //爆炸
            ExecutionBuilder.CreateBullet(406, 0, 2f, 0.2f, 0.4f);
            ExecutionBuilder.CreateBullet(407, 0, 3f, 0.2f, 0.6f);
            ExecutionBuilder.CreateBullet(408, 0, 4f, 0.2f, 0.8f);
            //刀光
            ExecutionBuilder.CreateBullet(409, 3, 1.2f, 3f, 0.3f);
            ExecutionBuilder.CreateBullet(410, 3, 1.8f, 4f, 0.5f);
            ExecutionBuilder.CreateBullet(411, 3, 2.4f, 5f, 0.7f);
            //狼
            ExecutionBuilder.CreateBullet(412, 4, 1f, 5f, 0.2f);
            ExecutionBuilder.CreateBullet(413, 4, 2f, 5f, 0.3f);
            ExecutionBuilder.CreateBullet(414, 4, 3f, 5f, 0.4f);
            //骷髅(附加减速)
            ExecutionBuilder.CreateBullet(415, 5, 1f, 5f, 0.2f, effect: 400);
            ExecutionBuilder.CreateBullet(416, 5, 2f, 5f, 0.3f, effect: 400);
            ExecutionBuilder.CreateBullet(417, 5, 3f, 5f, 0.4f, effect: 400);

            //buff400:减速
            ExecutionBuilder.InitEffect(400);
            ExecutionBuilder.AddEffect(3, 1f, 5f);
            ExecutionBuilder.UploadEffect();

            //Operation400：瞄准最近的敌人发射三发散弹
            ExecutionBuilder.InitSkillOperation(400);
            ExecutionBuilder.ShootBullet(0,400, (ushort)ShootIdCalculator.AngleNonFacing2(1, 0));
            ExecutionBuilder.ShootBullet(0,400, (ushort)ShootIdCalculator.AngleNonFacing2(1, 5));
            ExecutionBuilder.ShootBullet(0,400, (ushort)ShootIdCalculator.AngleNonFacing2(1, 175));
            ExecutionBuilder.UploadOperation();

            //Operation401：原地造成小爆炸并跃起
            ExecutionBuilder.InitSkillOperation(401);
            ExecutionBuilder.DoMotion(0, MotionIdCalculator.GetPluseId(0.2f, 0, 4, 6));
            ExecutionBuilder.ShootBullet(0,406,(ushort)ShootIdCalculator.Static(22,22));
            ExecutionBuilder.UploadOperation();

            //Operation402：发射4发子弹环绕玩家旋转
            ExecutionBuilder.InitSkillOperation(402);
            ExecutionBuilder.ShootBullet(0,401, (ushort)ShootIdCalculator.Orbit(1,6,0));
            ExecutionBuilder.ShootBullet(0,401, (ushort)ShootIdCalculator.Orbit(1,6,10));
            ExecutionBuilder.ShootBullet(0,401, (ushort)ShootIdCalculator.Orbit(1,6,20));
            ExecutionBuilder.ShootBullet(0,401, (ushort)ShootIdCalculator.Orbit(1,6,30));
            ExecutionBuilder.UploadOperation();

            //Operation403：在玩家两侧发射多发垂直子弹
            ExecutionBuilder.InitSkillOperation(403);
            ExecutionBuilder.ShootBullet(0, 403, (ushort)ShootIdCalculator.FromTo(4, 4, 4, 6));
            ExecutionBuilder.ShootBullet(0, 403, (ushort)ShootIdCalculator.FromTo(6, 4, 6, 6));
            ExecutionBuilder.ShootBullet(200, 403, (ushort)ShootIdCalculator.FromTo(4, 4, 4, 6));
            ExecutionBuilder.ShootBullet(200, 403, (ushort)ShootIdCalculator.FromTo(6, 4, 6, 6));
            ExecutionBuilder.ShootBullet(400, 403, (ushort)ShootIdCalculator.FromTo(4, 4, 4, 6));
            ExecutionBuilder.ShootBullet(400, 403, (ushort)ShootIdCalculator.FromTo(6, 4, 6, 6));
            ExecutionBuilder.ShootBullet(600, 403, (ushort)ShootIdCalculator.FromTo(4, 4, 4, 6));
            ExecutionBuilder.ShootBullet(600, 403, (ushort)ShootIdCalculator.FromTo(6, 4, 6, 6));
            ExecutionBuilder.ShootBullet(800, 403, (ushort)ShootIdCalculator.FromTo(4, 4, 4, 6));
            ExecutionBuilder.ShootBullet(800, 403, (ushort)ShootIdCalculator.FromTo(6, 4, 6, 6));
            ExecutionBuilder.UploadOperation();

            //Operation404：向前发射五发刀光
            ExecutionBuilder.InitSkillOperation(404);
            ExecutionBuilder.ShootBullet(0, 411, (ushort)ShootIdCalculator.Angle(1,0));
            ExecutionBuilder.ShootBullet(500, 410, (ushort)ShootIdCalculator.Angle(1, 10));
            ExecutionBuilder.ShootBullet(500, 410, (ushort)ShootIdCalculator.Angle(1, 170));
            ExecutionBuilder.ShootBullet(1000, 409, (ushort)ShootIdCalculator.Angle(1, 20));
            ExecutionBuilder.ShootBullet(1000, 409, (ushort)ShootIdCalculator.Angle(1, 160));
            ExecutionBuilder.UploadOperation();

            //Operation405：发射大量子弹并追加骷髅头
            ExecutionBuilder.InitSkillOperation(405);
            ExecutionBuilder.ShootBullet(0, 400, (ushort)ShootIdCalculator.Angle(1, 5));
            ExecutionBuilder.ShootBullet(100, 400, (ushort)ShootIdCalculator.Angle(1, 3));
            ExecutionBuilder.ShootBullet(200, 400, (ushort)ShootIdCalculator.Angle(1, 1));
            ExecutionBuilder.ShootBullet(300, 400, (ushort)ShootIdCalculator.Angle(1, 179));
            ExecutionBuilder.ShootBullet(400, 400, (ushort)ShootIdCalculator.Angle(1, 177));
            ExecutionBuilder.ShootBullet(500, 400, (ushort)ShootIdCalculator.Angle(1, 175));
            ExecutionBuilder.ShootBullet(600, 400, (ushort)ShootIdCalculator.Angle(1, 177));
            ExecutionBuilder.ShootBullet(700, 400, (ushort)ShootIdCalculator.Angle(1, 179));
            ExecutionBuilder.ShootBullet(800, 400, (ushort)ShootIdCalculator.Angle(1, 1));
            ExecutionBuilder.ShootBullet(900, 400, (ushort)ShootIdCalculator.Angle(1, 3));
            ExecutionBuilder.ShootBullet(1000, 400, (ushort)ShootIdCalculator.Angle(1, 5));
            ExecutionBuilder.ShootBullet(1300, 416, (ushort)ShootIdCalculator.Angle(1, 0));
            ExecutionBuilder.UploadOperation();
        }
    }
}