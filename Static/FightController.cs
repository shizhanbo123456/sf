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


    public Action<string> OnModeNameChanged;
    public Action<string> OnDescriptionChanged;
    private CustomLevelText customLevel;//只存储在房主客户端
    public CustomLevelText SelectedMode
    {
        get => customLevel;
        set
        {
            customLevel = value;
            CallFuncRpc(nameof(SyncNameLocal), SendTo.ExcludeSender, customLevel.joinedPath, KeyLibrary.KeyFormatType.OrderWise);
            CallFuncRpc(nameof(SyncDesLocal), SendTo.ExcludeSender, customLevel.description, KeyLibrary.KeyFormatType.OrderWise);
            OnModeNameChanged?.Invoke(customLevel.joinedPath);
        }
    }
    public void SyncNameTo(int player)
    {
        CallFuncRpc(nameof(SyncNameLocal),new List<int>() { player},customLevel.joinedPath,KeyLibrary.KeyFormatType.OrderWise);
    }
    private void SyncNameLocal(string data)=> OnModeNameChanged?.Invoke(data);
    public void SyncDesTo(int player)
    {
        CallFuncRpc(nameof(SyncDesLocal), new List<int>() { player },customLevel.description,KeyLibrary.KeyFormatType.OrderWise);
    }
    private void SyncDesLocal(string data)=>OnDescriptionChanged?.Invoke(data);


    [HideInInspector]public bool Fighting = false;

    private void Awake()
    {
        Tool.FightController = this;
    }

    public bool TryLoadLevelLua()=> CustomLevel.CreateCustomLevel(customLevel.logic);

    public void StartFight()//网络通讯器会创建玩家
    {
        Tool.SceneController.DestroyLevel();
        Tool.SceneController.DestroyNonSkillPlayer();
        Fighting = true;
        judgeEndCdRecorder = -10;

        CustomLevel.OnFightStart();
    }
    private void SettleRpc()
    {
        if (!Fighting) return;
        foreach(var i in ServerDataContainer.GetAllKeys())
        {
            CustomLevel.FigureScore(i,out int killscore, out int timescore, out int challengescore);
            var sb=Tool.stringBuilder;
            sb.Append(killscore).Append('_').Append(timescore).Append('_').Append(challengescore);
            CallFuncRpc(nameof(SettleLocal),new List<int>() { i}, sb.ToString(), KeyLibrary.KeyFormatType.DisorderConfirm);
        }
    }
    private void SettleLocal(string data)
    {
        StopFightLocal();
        string[] s = data.Split('_',StringSplitOptions.RemoveEmptyEntries);
        PlayModeController.Instance.Settle(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
    }
    public void StopFightLocal()//立即停止，不结算，保留关卡
    {
        Fighting = false;
        CustomLevel.ReleaseData();
        CustomLevel.Dispose();
    }


    private float judgeEndCdRecorder=0;
    private void Update()
    {
        if (!Fighting) return;
        CustomLevel.Update();
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
