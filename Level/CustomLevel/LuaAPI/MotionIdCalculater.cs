using XLua;

[LuaCallCSharp]
public static class MotionIdCalculater
{
    //1 2 3 4 type 2 10
    public static int GetStaticId(float time, int stoicLevel)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 1000;
        return id;
    }
    public static int GetDirId(float time, int stoicLevel, int speedx, int speedy)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 2000;
        id += speedx * 100000 + speedy * 10000;
        return id;
    }
    public static int GetForceId(float time, int stoicLevel, int speedx, int speedy)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 3000;
        id += speedx * 100000 + speedy * 10000;
        return id;
    }
    public static int GetLerpId(float time, int stoicLevel, int startx, int starty, int endx, int endy)
    {
        int id = 0;
        id += ((int)(time * 10)) % 100;
        id += stoicLevel * 100;
        id += 4000;
        id += endx * 10000000 + endy * 1000000 + startx * 100000 + starty * 10000;
        return id;
    }
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