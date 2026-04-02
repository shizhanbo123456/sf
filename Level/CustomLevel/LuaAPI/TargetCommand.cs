using LevelCreator.TargetTemplate;
using System.Collections.Generic;
using System.Linq;

[XLua.LuaCallCSharp]
public static class TargetCommand
{
    private static Dictionary<int, Target> Selected=new();
    public enum ComparsinRule
    {
        Larger,Smaller,LargerOrEqual,SmallerOrEqual,Equal,NotEqual
    }
    public static void ClearTargetBuffer()
    {

    }
    public static void MaximunTargetBuffer()
    {

    }
    public static void SelectByLabel(string label)//label必须“,”分隔
    {

    }
    public static void SelectByHealth(ComparsinRule rule,int value)
    {

    }
    public static void SelectByTargetData(int type)
    {

    }
    public static void SelectByTargetController(int type)
    {

    }
    public static void SelectBySkillController(int type)
    {

    }
    public static void SelectByEffectController(int type)
    {

    }
    public static void SelectByPos(float centerX,float centerY,float radius)
    {

    }
    public static void SelectByCamp(int camp)
    {

    }
    public static void SelectByLevel(ComparsinRule rule, int value)
    {

    }

    public static int[] GetObjectId()
    {
        return Selected.Keys.ToArray();
    }


    public static void Spawn(short id)
    {

    }
    public enum TeleportFaceOption
    {
        Right,Left,DontChange,Change,TowardNearestPartner,TowardNearestEnemy
    }
    public static void Teleport(float x,float y,bool face)
    {

    }
    public static void Effect(short id)
    {

    }
    public static void Operation(short id)
    {

    }
    public static void Damage()
    {
        
    }
    public static void Kill()
    {

    }
}