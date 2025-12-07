using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private CustomLevelText customLevel;//只存储在房主客户端
    public CustomLevelText SelectedMode
    {
        get => customLevel;
        set
        {
            customLevel = value;
            CallFuncRpc(nameof(SyncModeHeaderLocal), SendTo.ExcludeSender, customLevel.joinedPath, KeyLibrary.KeyFormatType.DisorderConfirm);
            OnHeaderChanged?.Invoke(customLevel.joinedPath);
        }
    }
    public void SyncModeHeaderTo(int player)
    {
        CallFuncRpc(nameof(SyncModeHeaderLocal),new List<int>() { player},customLevel.joinedPath,KeyLibrary.KeyFormatType.DisorderConfirm);
    }
    private void SyncModeHeaderLocal(string data)=> OnHeaderChanged?.Invoke(data);
    public void OnModeChange(Action<string> a)=> OnHeaderChanged += a;



    [HideInInspector]public bool Fighting = false;

    private void Awake()
    {
        Tool.FightController = this;
        Fighting = false;
    }
    private static int replySum;
    private static int replyCount;

    private static int partCount;
    private static List<string>Parts=new List<string>();
    public async Task<bool> SyncLevel()
    {
        replySum = 0;

        string logic = customLevel.logic;
        if (string.IsNullOrEmpty(logic))
        {
            Debug.LogError("空的关卡逻辑");
            return false;
        }
        try
        {
            CustomLevel.CreateCustomLevel(logic);
        }
        catch(Exception e)
        {
            Debug.LogError("加载了错误的逻辑 " + e.ToString());
            return false;
        }
        int nextSubstringStart = 0;
        List<string>parts= new List<string>();
        while (nextSubstringStart < logic.Length)
        {
            parts.Add(logic.Substring(nextSubstringStart, Math.Min(1000, logic.Length - nextSubstringStart)));
            nextSubstringStart +=1000;
        }
        CallFuncRpc(nameof(SendReply), SendTo.ExcludeSender, parts.Count.ToString(),KeyLibrary.KeyFormatType.DisorderConfirm);
        await Task.Delay(1000);
        if (replySum == 0) return false;

        int nextSyncIndex = 0;
        replyCount = 0;
        while (nextSyncIndex < parts.Count)
        {
            CallFuncRpc(nameof(SendRecvConfirm), SendTo.ExcludeSender, parts[nextSyncIndex],KeyLibrary.KeyFormatType.OrderWise);
            nextSyncIndex++;
            await Task.Delay(100);
        }
        float nextBreakTime= Time.time + 5f;
        while (replyCount < replySum)
        {
            await Task.Delay(300);
            if (nextBreakTime < Time.time)
            {
                Debug.LogError("同步出错");
                return false;
            }
        }
        return true;
    }
    private void SendReply(string partcount)//其它客户端处被调用
    {
        partCount=int.Parse(partcount);
        Parts.Clear();
        CallFuncRpc(nameof(RecvReply), SendTo.RoomOwner, KeyLibrary.KeyFormatType.DisorderConfirm);
    }
    private void RecvReply()//房主处被调用
    {
        replySum += 1;
    }
    private void SendRecvConfirm(string part)//其它客户端处被调用
    {
        Transition.Show();
        CallFuncRpc(nameof(RecvConfirm), SendTo.RoomOwner, KeyLibrary.KeyFormatType.DisorderConfirm);
        Parts.Add(part);
        if (Parts.Count == partCount)
        {
            Transition.Hide();
            var sb=Tool.stringBuilder;
            sb.Clear();
            foreach(var i in Parts)sb.Append(i);
            Debug.Log("接收到关卡逻辑  "+sb.ToString());
            CustomLevel.CreateCustomLevel(sb.ToString());
        }
    }
    private void RecvConfirm()//房主处被调用
    {
        replyCount += 1;
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
