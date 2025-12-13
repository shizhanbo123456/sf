using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// 只在房主客户端执行
/// </summary>
public static class CustomLevel
{
    private static LuaEnv luaEnv=> Tool.LuaManager.luaEnv;
    private static string LuaText;

    public static string[] ModePathParts;
    public static string ModePath;
    public static string ModeDescrpition;

    private static LuaFunction FightStartFunction;//0
    private static LuaFunction UpdateFunction;//float float
    private static LuaFunction JudgeEndFunction;//0->bool
    private static LuaFunction TargetKilledFunction;//TargetInfo killer,TargetInfo killed
    private static LuaFunction KillScoreFunction;//0->int
    private static LuaFunction TimeScoreFunction;//0->int
    private static LuaFunction ModeScoreFunction;//0->int
    private static LuaFunction ReleaseDataFunction;//0


    private static float StartTime = 0f;
    public static float FightTime => Fighting?(Time.time - StartTime):0f;

    public static bool Initialized = false;
    public static bool Fighting = false;

    public static bool CreateCustomLevel(string lua)
    {
        LuaText = lua;

        try
        {
            luaEnv.DoString(LuaText);

            int i = 0;
            while (i < lua.Length && i != '\n') i++;
            ModePath = luaEnv.Global.Get<string>("ModeName");
            ModePathParts = ModePath.Trim('-',' ').Split('-');
            ModeDescrpition= luaEnv.Global.Get<string>("ModeDescription");

            FightStartFunction = luaEnv.Global.Get<LuaFunction>("FightStart");
            UpdateFunction = luaEnv.Global.Get<LuaFunction>("Update");
            JudgeEndFunction = luaEnv.Global.Get<LuaFunction>("JudgeEnd");
            TargetKilledFunction = luaEnv.Global.Get<LuaFunction>("TargetKilled");
            KillScoreFunction = luaEnv.Global.Get<LuaFunction>("KillScore");
            TimeScoreFunction = luaEnv.Global.Get<LuaFunction>("TimeScore");
            ModeScoreFunction = luaEnv.Global.Get<LuaFunction>("ModeScore");
            ReleaseDataFunction = luaEnv.Global.Get<LuaFunction>("ReleaseData");
        }
        catch(System.Exception e)
        {
            Debug.LogError("加载关卡逻辑时出错： " + e.ToString());
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
        Fighting= true;
        StartTime = Time.time;
        //创建关卡、角色、装载技能
        FightStartFunction?.Action(0);
    }
    public static void TargetKilled(TargetInfo killed)
    {
        TargetKilledFunction?.Action(new TargetInfo(),killed);
    }
    public static void TargetKilled(TargetInfo killer,TargetInfo killed)
    {
        TargetKilledFunction?.Action(killer,killed);
    }
    public static void Update()
    {
        UpdateFunction?.Action(Time.time - StartTime,Time.deltaTime);
    }
    public static bool JudgeEnd()
    {
        return JudgeEndFunction != null ? JudgeEndFunction.Func<int, bool>(0) : false;
    }
    public static void FigureScore(out int killScore,out int timeScore,out int challengeScore)
    {
        killScore=KillScoreFunction!=null?KillScoreFunction.Func<int, int>(0):233;
        timeScore=TimeScoreFunction != null ? TimeScoreFunction.Func<int, int>(0) : 233;
        challengeScore = ModeScoreFunction != null ? ModeScoreFunction.Func<int, int>(0) : 233;
    }
    public static void ReleaseData()
    {
        Fighting = false;
        try
        {
            ReleaseDataFunction?.Action(0);
        }
        catch
        {
            Debug.LogError("关卡释放异常");
        }
    }
    public static void Dispose()
    {
        Initialized = false;
        try
        {
            FightStartFunction?.Dispose();
            UpdateFunction?.Dispose();
            JudgeEndFunction?.Dispose();
            TargetKilledFunction?.Dispose();
            KillScoreFunction?.Dispose();
            TimeScoreFunction?.Dispose();
            ModeScoreFunction?.Dispose();
            ReleaseDataFunction?.Dispose();
        }
        catch(System.InvalidOperationException e)
        {
            Debug.Log(e);
        }
    }
}