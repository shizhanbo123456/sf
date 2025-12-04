using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

public static class CustomLevel
{
    private static LuaEnv luaEnv=> Tool.LuaManager.luaEnv;
    private static string LuaText;

    public static bool Initialized = false;

    public static string[] LevelPath;
    public static string LevelPathJoined=>Format.ArrayToString(LevelPath,'-').Trim('-');

    private static LuaFunction FightStartFunction;//0
    private static LuaFunction UpdateFunction;//float float
    private static LuaFunction JudgeEndFunction;//0->bool
    private static LuaFunction KillScoreFunction;//0->int
    private static LuaFunction TimeScoreFunction;//0->int
    private static LuaFunction ModeScoreFunction;//0->int
    private static LuaFunction ReleaseDataFunction;//0


    private static float StartTime = 0f;
    public static float FightTime => Time.time - StartTime;

    public static bool CreateCustomLevel(string lua)
    {
        LuaText = lua;

        try
        {
            luaEnv.DoString(LuaText);

            LevelPath = luaEnv.Global.Get<string>("LevelPath").Split('-');

            FightStartFunction = luaEnv.Global.Get<LuaFunction>("FightStart");
            UpdateFunction = luaEnv.Global.Get<LuaFunction>("Update");
            JudgeEndFunction = luaEnv.Global.Get<LuaFunction>("JudgeEnd");
            KillScoreFunction = luaEnv.Global.Get<LuaFunction>("KillScore");
            TimeScoreFunction = luaEnv.Global.Get<LuaFunction>("TimeScore");
            ModeScoreFunction = luaEnv.Global.Get<LuaFunction>("ModeScore");
            ReleaseDataFunction = luaEnv.Global.Get<LuaFunction>("ReleaseData");
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
    public static void FigureScore(out int killScore,out int timeScore,out int challengeScore)
    {
        killScore=KillScoreFunction.Func<int, int>(0);
        timeScore=TimeScoreFunction.Func<int, int>(0);
        challengeScore = ModeScoreFunction.Func<int, int>(0);
    }
    public static void ReleaseData()
    {
        try
        {
            ReleaseDataFunction.Action(0);
        }
        catch
        {
            Debug.LogError("关卡释放异常");
        }
    }
    public static void Dispose()
    {
        Initialized = false;

        FightStartFunction?.Dispose();
        UpdateFunction?.Dispose();
        JudgeEndFunction?.Dispose();
    }
}