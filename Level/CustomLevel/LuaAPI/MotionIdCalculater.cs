using XLua;

[LuaCallCSharp]
public static class MotionIdCalculater
{
    //speed=speedFactor*6-30
    //下文中的speedx,speedy等均使用此公式计算得到的speed

    //静止不动。如果释放技能期间不能移动，可为玩家添加此类位移效果
    public static int GetStaticId(float time, int stoicLevel)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 1000;
        return id;
    }
    //匀速直线运动
    public static int GetDirId(float time, int stoicLevel, int speedx, int speedy)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 2000;
        id += speedx * 100000 + speedy * 10000;
        return id;
    }
    //匀加速直线运动
    public static int GetForceId(float time, int stoicLevel, int speedx, int speedy)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 3000;
        id += speedx * 100000 + speedy * 10000;
        return id;
    }
    //速度由插值计算得出，需要变速或曲线移动可使用
    public static int GetLerpId(float time, int stoicLevel, int startx, int starty, int endx, int endy)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 4000;
        id += endx * 10000000 + endy * 1000000 + startx * 100000 + starty * 10000;
        return id;
    }
    //只有在被添加位移效果的一刻，速度被调整为目标值。此类位移效果的生效时间建议设置得较短，例如0.1s
    public static int GetPluseId(float time, int stoicLevel, int speedx, int speedy)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 5000;
        id += speedx * 100000 + speedy * 10000;
        return id;
    }
}