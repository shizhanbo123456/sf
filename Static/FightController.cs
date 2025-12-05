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


    private Action<string> OnHeaderChanged;
    private CustomLevelText customLevel;
    public CustomLevelText SelectedMode
    {
        get => customLevel;
        set
        {
            customLevel = value;
            CallFuncRpc(nameof(SyncModeHeaderLocal), SendTo.ExcludeSender, customLevel.joinedPath, KeyLibrary.KeyFormatType.Nonsequential);
            OnHeaderChanged?.Invoke(customLevel.joinedPath);
        }
    }
    public void SyncModeHeaderTo(int player)
    {
        CallFuncRpc(nameof(SyncModeHeaderLocal),new List<int>() { player},customLevel.joinedPath,KeyLibrary.KeyFormatType.Nonsequential);
    }
    private void SyncModeHeaderLocal(string data)=> OnHeaderChanged?.Invoke(data);
    public void OnModeChange(Action<string> a)=> OnHeaderChanged += a;



    [HideInInspector]public bool Fighting = false;

    private void Awake()
    {
        Tool.FightController = this;
        Fighting = false;
    }
    
    public void StartFight()//网络通讯器会创建玩家
    {
        Tool.SceneController.DestroyLevel();
        Tool.SceneController.DestroyNonSkillPlayer();
        Fighting = true;
        judgeEndCdRecorder = -10;
    }
    private void SettleRpc()
    {
        if (!Fighting) return;
        CallFuncRpc(nameof(SettleLocal), SendTo.Everyone);
    }
    private void SettleLocal()
    {
        StopFightLocal();
        int killscore;
        int timescore;
        int challengescore;
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
    public void StopFightLocal()//立即停止，不结算，保留关卡
    {
        Fighting = false;
    }


    private float judgeEndCdRecorder=0;
    private void Update()
    {
        if (!Fighting) return;
        if (judgeEndCdRecorder < 1)
        {
            judgeEndCdRecorder += Time.deltaTime;
            return;
        }
        judgeEndCdRecorder = 0;

        bool end = false;
        try
        {
            end = CustomLevel.JudgeEnd();
        }
        catch
        {
            end = false;
            Tool.Notice.ShowMesg("关卡结算异常");
        }
        if (end) SettleRpc();
    }
}