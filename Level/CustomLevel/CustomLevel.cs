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
    private static LuaFunction TargetKilledFunction;//TargetInfo
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

            int i = 0;
            while (i < lua.Length && i != '\n') i++;
            LevelPath =lua.Substring(0,i).Trim('-',' ').Split('-');

            FightStartFunction = luaEnv.Global.Get<LuaFunction>("FightStart");
            TargetKilledFunction = luaEnv.Global.Get<LuaFunction>("TargetKilled");
            UpdateFunction = luaEnv.Global.Get<LuaFunction>("Update");
            JudgeEndFunction = luaEnv.Global.Get<LuaFunction>("JudgeEnd");
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
        StartTime = Time.time;
        //创建关卡、角色、装载技能
        FightStartFunction.Action(0);
    }
    public static void TargetKilled(TargetInfo targetInfo)
    {
        TargetKilledFunction.Action(targetInfo);
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

        try
        {
            FightStartFunction?.Dispose();
            TargetKilledFunction?.Dispose();
            UpdateFunction?.Dispose();
            JudgeEndFunction?.Dispose();
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