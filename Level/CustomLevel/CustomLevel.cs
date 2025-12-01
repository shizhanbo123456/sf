using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

public static class CustomLevel
{
    private static LuaEnv luaEnv=> Tool.LuaManager.luaEnv;
    private static string LuaText;

    public static bool Initialized = false;

    public static List<string> LevelPath;
    private static float StartTime = 0;

    private static LuaFunction ProcessCampDataFunction;
    private static LuaFunction FightStartFunction;
    private static LuaFunction UpdateFunction;
    private static LuaFunction JudgeEndFunction;

    public static bool CreateCustomLevel(string lua)
    {
        LuaText = lua;

        try
        {
            luaEnv.DoString(LuaText);
            ProcessCampDataFunction = luaEnv.Global.Get<LuaFunction>("ProcessCampData");
            FightStartFunction = luaEnv.Global.Get<LuaFunction>("FightStart");
            UpdateFunction = luaEnv.Global.Get<LuaFunction>("Update");
            JudgeEndFunction = luaEnv.Global.Get<LuaFunction>("JudgeEnd");
        }
        catch
        {
            Dispose();
            return false;
        }
        Initialized = true;
        return true;
    }
    public static string GetLuaText()
    {
        return LuaText;
    }
    public static bool ProcessCampData(Dictionary<int,int>campData,out string msg)
    {
        msg=ProcessCampDataFunction.Func<int[], string>(campData.Values.ToArray());
        return msg == string.Empty;
    }
    public static void OnFightStart()
    {
        StartTime = Time.time;
        //创建关卡、角色、装载技能
        FightStartFunction.Action(0);
    }
    public static void Update()
    {
        UpdateFunction.Action(Time.time - StartTime,Time.deltaTime);
    }
    public static bool JudgeEnd()
    {
        return JudgeEndFunction.Func<int,bool>(0);
    }

    public static void Dispose()
    {
        Initialized = false;

        ProcessCampDataFunction?.Dispose();
        FightStartFunction?.Dispose();
        UpdateFunction?.Dispose();
        JudgeEndFunction?.Dispose();
    }
}