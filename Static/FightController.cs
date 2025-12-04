using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : EnsBehaviour
{
    public static int localPlayerId
    {
        get
        {
            if(EnsInstance.Corr.networkMode!=EnsCorrespondent.NetworkMode.None) return EnsInstance.LocalClientId;
            else
            {
                Debug.LogWarning("错误的时机获取id");
                return 0;
            }
        }
    }


    private RegistableVariable<string> modeList=new RegistableVariable<string>("00");
    public string ModeList
    {
        get
        {
            return modeList.Value;
        }
        set
        {
            if (!int.TryParse(value,out var _))
            {
                throw new Exception();
            }
            modeList.Value = value;
            SyncMode();
        }
    }
    public void SyncMode()
    {
        CallFuncRpc(nameof(SyncMode), SendTo.ExcludeSender, KeyLibrary.KeyFormatType.Nonsequential);
    }
    private void SyncMode(string data)
    {
        modeList.Value = data;
    }
    public void OnModeListChange(Action<string> a)
    {
        modeList.OnValueChanged += a;
    }


    public List<int> KillCount=new List<int>();
    public List<int> KilledCount=new List<int>();

    private Func<bool> JudgeEnd;


    [HideInInspector]public bool Fighting = false;
    [HideInInspector]public float FightTimeCount;

    private void Awake()
    {
        Tool.FightController = this;
        Fighting = false;
    }
    
    public void StartFight()//网络通讯器会创建玩家
    {
        Tool.SceneController.DestroyLevel();
        Tool.SceneController.DestroyNonSkillPlayer();
        Tool.PageManager.TurnPage(PageManager.PageType.PlayMode);
        Tool.SceneController.CreateLevel(Tool.SceneController.ModeToLevel(modeList.Value));
        KillCount = new List<int>() { 0,0,0,0};
        KilledCount = new List<int>() { 0,0,0,0};
        Fighting = true;
        timeCount = -10;
        JudgeEnd = CustomLevel.JudgeEnd;
        Tool.ScoreboardController.InitScoreboard();
    }
    private void EndFightRpc()
    {
        if (!Fighting) return;
        CallFuncRpc(nameof(EndFightLocal), SendTo.Everyone);
    }
    private void EndFightLocal()
    {
        StopFight();
        int killscore = 0;
        int timescore = 0;
        int challengescore = 0;
        try
        {
            CustomLevel.FigureScore(out killscore, out timescore, out challengescore);
        }
        catch
        {
            killscore = 0;
            timescore = 0;
            challengescore = 0;
        }
        Tool.PageManager.PlayModePage.settlement.Settle(killscore, timescore, challengescore);
    }
    public void StopFight()//立即停止，不结算，保留关卡
    {
        Fighting = false;
        Tool.ScoreboardController.CloseScoreBoard();
    }


    /// <summary>
    /// 对于无击杀，意外死亡，使用无效击杀阵营(9等)
    /// </summary>
    public void OnDeathRpc(int killedCamp,int killerCamp)
    {
        CallFuncRpc(nameof(OnDeathLocal), SendTo.Everyone, killedCamp + "_" + killerCamp,KeyLibrary.KeyFormatType.Nonsequential);
    }
    private void OnDeathLocal(string data)
    {
        string[] s = data.Split('_');
        int killedCamp = int.Parse(s[0]);
        int killerCamp = int.Parse(s[1]);
        if (killerCamp < KillCount.Count)
        {
            KillCount[killerCamp] += 1;
            Tool.ScoreboardController.SetText(0, killerCamp, KillCount[killerCamp].ToString());
        }
        KilledCount[killedCamp] += 1;
        Tool.ScoreboardController.SetText(0, killedCamp, KilledCount[killedCamp].ToString());
    }

    private float timeCount=0;
    private void Update()
    {
        if (!Fighting) return;
        FightTimeCount += Time.deltaTime;
        if (timeCount < 1)
        {
            timeCount += Time.deltaTime;
            return;
        }
        timeCount = 0;

        bool end = false;
        try
        {
            end = JudgeEnd.Invoke();
        }
        catch
        {
            end = false;
            Tool.Notice.ShowMesg("关卡结算异常");
        }
        if (end) EndFightRpc();
    }
    private void OnEnable()
    {
        FightTimeCount = 0;
    }
    private void OnDisable()
    {
        FightTimeCount = 0;
    }
}