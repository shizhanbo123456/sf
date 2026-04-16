using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class MotionIdCalculator//辅助根据需要计算出对应的编码Id
{
    //水平方向速度为负表示向后，为正表示向前，竖直方向速度为负表示向下，为正表示向上
    //stoicLevel为0、1、2。为0时可以被任何攻击打断，为1时只可被强力攻击打断，为2时不可被任何攻击打断
    //速度因子mod=9时，=4时速度为0，小于4时速度为负，大于4时速度为正，离4越远速度绝对值越大
    //速度因子mod=5时，=2时速度为0，小于2时速度为负，大于2时速度为正，离2越远速度绝对值越大

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
    //匀速直线运动，速度因子范围[0,8]
    public static ushort GetDirId(float time, int stoicLevel, int speedx, int speedy)//81
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 1+speedV2Mod.Encode(speedx,speedy));//1-81
        return id;
    }
    //匀加速直线运动，速度因子范围[0,8]
    public static ushort GetForceId(float time, int stoicLevel, int speedx, int speedy)//81
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 82 + speedV2Mod.Encode(speedx, speedy));//82-162
        return id;
    }
    //速度由插值计算得出，需要变速或曲线移动可使用，速度因子范围[0,4]
    public static ushort GetLerpId(float time, int stoicLevel, int startx, int starty, int endx, int endy)//625
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 163 + speedV4Mod.Encode(startx,starty,endx,endy));//163-787
        return id;
    }
    //只有在被添加位移效果的一刻，速度被调整为目标值。此类位移效果的生效时间建议设置得较短，例如0.1s，速度因子范围[0,8]
    public static ushort GetPluseId(float time, int stoicLevel, int speedx, int speedy)//81
    {
        if (stoicLevel != 0 && stoicLevel != 1 && stoicLevel != 2) throw new System.Exception("霸体等级超出范围");
        int timeFactor = (int)(Mathf.Clamp(time, 0.201f, 5.001f) * 5)-1;
        ushort id = (ushort)mod.Encode(timeFactor, stoicLevel, 788 + speedV2Mod.Encode(speedx, speedy));//788-868
        return id;
    }
}