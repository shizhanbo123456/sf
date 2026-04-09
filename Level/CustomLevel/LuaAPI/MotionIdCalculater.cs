using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class MotionIdCalculater
{
    //speed=speedFactor*6-24
    //下文中的speedx,speedy等均使用此公式计算得到的speed

    public static ModSpace3 mod = new(25, 3, 869);

    public static ModSpace2 speedV2Mod = new(9, 9);
    public static ModSpace4 speedV4Mod = new(5,5,5,5);

    //静止不动。如果释放技能期间不能移动，可为玩家添加此类位移效果
    public static ushort GetStaticId(float time, int stoicLevel)//1*3*25
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time,0.201f,5.001f)*5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel,0);//0
        return id;
    }
    //匀速直线运动
    public static ushort GetDirId(float time, int stoicLevel, int speedx, int speedy)//81
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 1+speedV2Mod.Encode(speedx,speedy));//1-81
        return id;
    }
    //匀加速直线运动
    public static ushort GetForceId(float time, int stoicLevel, int speedx, int speedy)//81
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 82 + speedV2Mod.Encode(speedx, speedy));//82-162
        return id;
    }
    //速度由插值计算得出，需要变速或曲线移动可使用
    public static ushort GetLerpId(float time, int stoicLevel, int startx, int starty, int endx, int endy)//625
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 163 + speedV4Mod.Encode(startx,starty,endx,endy));//163-787
        return id;
    }
    //只有在被添加位移效果的一刻，速度被调整为目标值。此类位移效果的生效时间建议设置得较短，例如0.1s
    public static ushort GetPluseId(float time, int stoicLevel, int speedx, int speedy)//81
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 788 + speedV2Mod.Encode(speedx, speedy));//788-868
        return id;
    }
}