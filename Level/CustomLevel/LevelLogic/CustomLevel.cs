using UnityEngine;
using XLua;

namespace LevelCreator
{
    /// <summary>
    /// 只在房主客户端执行
    /// </summary>
    public static class CustomLevel
    {
        private static LuaEnv luaEnv;
        private static string LuaText;

        public static string[] ModePathParts;
        public static string ModePath;
        public static string ModeDescrpition;

        private static LuaFunction InitTemplateFunction;
        private static LuaFunction FightStartFunction;//0
        private static LuaFunction UpdateFunction;//float float
        private static LuaFunction JudgeEndFunction;//0->bool
        private static LuaFunction TargetKilledFunction;//TargetInfo killer,TargetInfo killed
        private static LuaFunction SelectFunction;//int selector,int selection
        private static LuaFunction EnterCheckPointFunction;//int targetId,int checkPointId
        private static LuaFunction SelectablePointClickedFunction;//int clientId,int checkPointId
        private static LuaFunction KillScoreFunction;//0->int
        private static LuaFunction TimeScoreFunction;//0->int
        private static LuaFunction ModeScoreFunction;//0->int
        private static LuaFunction ReleaseDataFunction;//0


        private static float StartTime = 0f;
        public static float FightTime => Fighting ? (Time.time - StartTime) : 0f;

        public static bool Initialized = false;
        public static bool Fighting = false;

        public static bool CreateCustomLevel(string lua)
        {
            if (luaEnv == null)
            {
                luaEnv = new LuaEnv();
                Tool.OnApplicationQuitEvent += () => luaEnv.Dispose();
            }

            LuaText = lua;

            try
            {
                luaEnv.DoString(LuaText);

                int i = 0;
                while (i < lua.Length && i != '\n') i++;
                ModePath = luaEnv.Global.Get<string>("ModeName");
                ModePathParts = ModePath.Trim('-', ' ').Split('-');
                ModeDescrpition = luaEnv.Global.Get<string>("ModeDescription");

                InitTemplateFunction = luaEnv.Global.Get<LuaFunction>("InitTemplate");
                FightStartFunction = luaEnv.Global.Get<LuaFunction>("FightStart");
                UpdateFunction = luaEnv.Global.Get<LuaFunction>("Update");
                JudgeEndFunction = luaEnv.Global.Get<LuaFunction>("JudgeEnd");
                TargetKilledFunction = luaEnv.Global.Get<LuaFunction>("TargetKilled");
                SelectFunction = luaEnv.Global.Get<LuaFunction>("Select");
                EnterCheckPointFunction = luaEnv.Global.Get<LuaFunction>("EnterCheckPoint");
                SelectablePointClickedFunction = luaEnv.Global.Get<LuaFunction>("SelectablePointClicked");
                KillScoreFunction = luaEnv.Global.Get<LuaFunction>("KillScore");
                TimeScoreFunction = luaEnv.Global.Get<LuaFunction>("TimeScore");
                ModeScoreFunction = luaEnv.Global.Get<LuaFunction>("ModeScore");
                ReleaseDataFunction = luaEnv.Global.Get<LuaFunction>("ReleaseData");
            }
            catch (System.Exception e)
            {
                Debug.LogError("[CustomLevel]加载关卡逻辑时出错： " + e.ToString());
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
        public static void OnInitTemplate()
        {
            try
            {
                InitTemplateFunction.Action(0);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void OnFightStart()
        {
            Fighting = true;
            StartTime = Time.time;
            //创建关卡、角色、装载技能
            try
            {
                FightStartFunction.Action(0);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void TargetKilled(TargetIdentify killed)
        {
            try
            {
                TargetKilledFunction.Action(new TargetIdentify() { camp=-1}, killed);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void TargetKilled(TargetIdentify killer, TargetIdentify killed)
        {
            try
            {
                TargetKilledFunction.Action(killer, killed);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void Select(int clientId,int index)
        {
            try
            {
                SelectFunction.Action(clientId,index);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void EnterCheckPoint(int targetId,int index)
        {
            try
            {
                EnterCheckPointFunction.Action(targetId,index);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void SelectablePointClicked(int clientId,int index)
        {
            try
            {
                SelectablePointClickedFunction.Action(clientId,index);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void Update()
        {
            try
            {
                UpdateFunction.Action(Time.time - StartTime, Time.deltaTime);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static bool JudgeEnd()
        {
            try
            {
                return JudgeEndFunction.Func<int, bool>(0);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
        public static void FigureScore(int clientId, out int killScore, out int timeScore, out int challengeScore)
        {
            try
            {
                killScore = KillScoreFunction.Func<int, int>(clientId);
                timeScore = TimeScoreFunction.Func<int, int>(clientId);
                challengeScore = ModeScoreFunction.Func<int, int>(clientId);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                killScore = 111;
                timeScore = 222;
                challengeScore = 333;
            }
        }
        public static void ReleaseData()
        {
            Fighting = false;
            try
            {
                ReleaseDataFunction.Action(0);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void Dispose()
        {
            Initialized = false;
            try
            {
                InitTemplateFunction?.Dispose();
                FightStartFunction?.Dispose();
                UpdateFunction?.Dispose();
                JudgeEndFunction?.Dispose();
                TargetKilledFunction?.Dispose();
                SelectFunction?.Dispose();
                EnterCheckPointFunction?.Dispose();
                SelectablePointClickedFunction?.Dispose();
                KillScoreFunction?.Dispose();
                TimeScoreFunction?.Dispose();
                ModeScoreFunction?.Dispose();
                ReleaseDataFunction?.Dispose();
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogException(e);
            }
        }
    }
}