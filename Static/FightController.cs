using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FightController : EnsBehaviour
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
    private CustomLevelInfo customLevel;//只存储在房主客户端
    public CustomLevelInfo SelectedMode
    {
        get => customLevel;
        set
        {
            customLevel = value;
            CallFuncRpc(SyncNameLocal, SendTo.ExcludeSender, Delivery.OrderWise, customLevel.joinedPath);
            CallFuncRpc(SyncDesLocal, SendTo.ExcludeSender, Delivery.OrderWise, customLevel.description);
            OnModeNameChanged?.Invoke(customLevel.joinedPath);
            OnDescriptionChanged?.Invoke(customLevel.description);
        }
    }
    public void SyncNameTo(short player)
    {
        CallFuncRpc(SyncNameLocal, SendTo.To(player), Delivery.OrderWise, customLevel.joinedPath);
    }
    [Rpc]
    private void SyncNameLocal(string data)=> OnModeNameChanged?.Invoke(data);
    public void SyncDesTo(short player)
    {
        CallFuncRpc(SyncDesLocal, SendTo.To(player), Delivery.OrderWise, customLevel.description);
    }
    [Rpc]
    private void SyncDesLocal(string data)=>OnDescriptionChanged?.Invoke(data);


    [HideInInspector]public bool Fighting = false;

    private void Awake()
    {
        Tool.FightController = this;
    }

    public bool TryLoadLevelLua()=> LevelCreator.CustomLevel.CreateCustomLevel(customLevel.logic);

    public void InitFight()
    {
        Tool.PageManager.PageActive(PageManager.PageType.Prepare, false);
        Tool.SceneController.DestroyLevel();
        Tool.SceneController.DestroyNonSkillPlayer();
    }
    public void StartFight()
    {
        Tool.PageManager.PageActive(PageManager.PageType.PlayMode, true);
        Fighting = true;
        judgeEndCdRecorder = -10;

        if (EnsInstance.HasAuthority) LevelCreator.CustomLevel.OnFightStart();
    }
    private void SettleRpc()
    {
        if (!Fighting) return;
        if (EnsInstance.HasAuthority)
            foreach (var i in ServerDataContainer.GetAllKeys())
            {
                LevelCreator.CustomLevel.FigureScore(i, out int killscore, out int timescore, out int challengescore);
                CallFuncRpc(SettleLocal, SendTo.To(i), Delivery.Reliable, killscore,timescore,challengescore);
            }
    }
    [Rpc]
    private void SettleLocal(int killscore, int timescore,int challengescore)
    {
        StopFightLocal();
        PlayModeController.Instance.Settle(killscore,timescore,challengescore);
    }
    public void StopFightLocal()//立即停止，不结算，保留关卡
    {
        Fighting = false;
        if (EnsInstance.HasAuthority)
        {
            LevelCreator.CustomLevel.ReleaseData();
            LevelCreator.CustomLevel.Dispose();
        }
    }


    private float judgeEndCdRecorder=0;
    private void Update()
    {
        if (!Fighting) return;
        if (EnsInstance.HasAuthority)
        {
            LevelCreator.CustomLevel.Update();
            if (judgeEndCdRecorder < 1)
            {
                judgeEndCdRecorder += Time.deltaTime;
                return;
            }
            judgeEndCdRecorder = 0;

            bool end;
            try
            {
                end = LevelCreator.CustomLevel.JudgeEnd();
            }
            catch
            {
                end = false;
                Tool.Notice.ShowMesg("关卡结算异常");
            }
            if (end) SettleRpc();
        }
    }
}
