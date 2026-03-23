using Ens.Request.Client;
using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkCorrespondent : EnsBehaviour
{
    private void Awake()
    {
        Tool.NetworkCorrespondent = this;

        //客户端断开连接时(准备或战斗)删除技能信息，物体，阵营图标
        EnsInstance.OnClientExit += (id) =>
        {
            DeleteData(id);
            Debug.Log("id为" + id + "的玩家断开连接");
            if (Tool.SceneController.NonSkillPlayers.ContainsKey(id)) Destroy(Tool.SceneController.NonSkillPlayers[id]);
            Tool.SceneController.DestroyTargetByOwner(id);
        };
        //离开房间重置场景
        EnsInstance.OnExitRoom += RestartGame;
        //连接失败重置场景
        EnsInstance.OnConnectionRejected += RestartGame;
        //断开连接重置场景
        EnsInstance.OnServerDisconnect += RestartGame;
        //连接异常重置场景
        EnsInstance.OnTimeOut(RestartGame + (System.Action)(()=>Debug.Log("请求超时")));
        EnsInstance.OnError(RestartGame+(System.Action)(() => Debug.Log("请求出错")));

        //连接后向主机发送新增玩家信息
        EnsInstance.OnJoinRoom += () =>
        {
            SendData();
        };
        EnsInstance.OnCreateRoom +=()=>
        {
            Dictionary<string, string> info = new Dictionary<string, string>()
            {
                { "Name","房间名称"},
                { "State","匹配中"},
                { "IP",EnsInstance.PresentRoomId==EnsRoomManager.roomIdStart?Tool.GetIP():EnsInstance.PresentRoomId.ToString()},
            };
            SetInfo.SendRequest(info);
            SendData();
        };

        EnsInstance.OnAuthorityChanged += () => { Debug.Log("权限设置为："+EnsInstance.HasAuthority); };
    }

    



    #region//数据同步
    /// <summary>
    /// 调用后，自动同步信息、阵营、玩家物体\出战的boss
    /// </summary>
    public void SendData()//Client
    {
        Tool.SceneController.DestroyLevel();
        Tool.SceneController.DestroyPlayer();
        Tool.PageManager.PageActive(PageManager.PageType.Home, false);
        Tool.PageManager.PageActive(PageManager.PageType.Prepare,true);
        Tool.SceneController.CreateLevel(1);

        var player = new ServerDataContainer.PlayerDataContainer(
            EnsInstance.LocalClientId,
            PlayerInfo.Name
            );
        CallFuncRpc(RecvData, SendTo.RoomOwner, Delivery.Reliable, player.ToString());
        Tool.FightController.SelectedMode = Tool.FightController.SelectedMode;
    }

    //接收到新增客户端的信息
    [Rpc]
    private void RecvData(string data)//Server
    {
        var playerData = new ServerDataContainer.PlayerDataContainer(data);

        //防止启动Host时没有数据而出错
        if (ServerDataContainer.GetAllValues().Count != 0)
        {
            string AllData = ServerDataContainer.ReturnAll();
            CallFuncRpc(RecvAllData, SendTo.To(playerData.id), Delivery.OrderWise, AllData);
        }

        CallFuncRpc(RecvNewData, SendTo.Everyone, Delivery.OrderWise, data);
        Tool.FightController.SyncNameTo(playerData.id);
        Tool.FightController.SyncDesTo(playerData.id);

        StartCoroutine(RecreatePlayers(playerData.id,playerData));
    }
    [Rpc]
    private void RecvAllData(string data)
    {
        ServerDataContainer.LoadAll(data);
    }
    [Rpc]
    private void RecvNewData(string data)
    {
        var playerData = new ServerDataContainer.PlayerDataContainer(data);
        ServerDataContainer.Set(playerData);
    }
    public IEnumerator RecreatePlayers(short playerid,ServerDataContainer.PlayerDataContainer playerData)
    {
        yield return 3;
        foreach (var i in Tool.SceneController.NonSkillPlayers.Values)
        {
            EnsInstance.EnsSpawner.RespawnCheckServerRpc(SendTo.To(playerid),i.GetComponent<NonSkillPlayerCollection>(), i.id.ToString());
        }
        EnsInstance.EnsSpawner.CreateServerRpc(SendTo.Everyone,Tool.PrefabManager.NonSkillPlayerCollection.CollectionId,
            playerData.id.ToString());
    }

    

    public void DeleteData(short id)
    {
        ServerDataContainer.Remove(id);
    }
    #endregion


    public void SetScoreboardActiveRpc(bool active)
    {
        CallFuncRpc(SetScoreboardActiveLocal, SendTo.Everyone, Delivery.Reliable,active ? "1" : "0");
    }
    [Rpc]
    private void SetScoreboardActiveLocal(string data)
    {
        PlayModeController.Instance.SetScoreboardActive(data == "1");
    }
    public void SetScoreboardTextRpc(int x, int y, string data)
    {
        var sb=Tool.stringBuilder;
        sb.Append(x).Append('_').Append(y).Append('_').Append(data);
        CallFuncRpc(SetScoreboardTextLocal, SendTo.Everyone, Delivery.Reliable, sb.ToString());
    }
    [Rpc]
    private void SetScoreboardTextLocal(string data)
    {
        string[] s = data.Split('_');
        PlayModeController.Instance.SetScoreboardText(int.Parse(s[0]), int.Parse(s[1]), s[2]);
    }
    public void CreateLevelRpc(int type)
    {
        CallFuncRpc(CreateLevelLocal, SendTo.Everyone, Delivery.Reliable, type.ToString());
    }
    [Rpc]
    private void CreateLevelLocal(string data)
    {
        Tool.SceneController.CreateLevel(int.Parse(data));
    }
    public void DestroyLevelRpc()
    {
        CallFuncRpc(DestroyLevelLocal, SendTo.Everyone, Delivery.Reliable);
    }
    [Rpc]
    private void DestroyLevelLocal()
    {
        Tool.SceneController.DestroyLevel();
    }

    public void TargetKilledRpc(Target killer,Target killed)
    {
        if (killed == null)
        {
            Debug.LogError("被击杀者不能为空");
            return;
        }
        //info用/划分
        if(killer == null)
        {
            CallFuncRpc(TargetKilledLocal, SendTo.RoomOwner, Delivery.Reliable, killed.Info.ToString());
        }
        else
        {
            var sb=Tool.stringBuilder;
            sb.Append(killer.Info.ToString()).Append('_').Append(killed.Info.ToString());
            CallFuncRpc(TargetKilledLocal, SendTo.RoomOwner, Delivery.Reliable, sb.ToString());
        }
    }
    [Rpc]
    private void TargetKilledLocal(string data)
    {
        if (data.Contains('_'))
        {
            var s=data.Split('_');
            if (EnsInstance.HasAuthority) LevelCreator.CustomLevel.TargetKilled(new TargetIdentify(s[0]), new TargetIdentify(s[1]));
            else Debug.LogError("击败消息发送到了非权威客户端");
        }
        else
        {
            if (EnsInstance.HasAuthority) LevelCreator.CustomLevel.TargetKilled(new TargetIdentify(data));
            else Debug.LogError("击败消息发送到了非权威客户端");
        }
    }


    #region//开始战斗
    public void StartFight()//(id,camp)
    {
        if (Tool.FightController.TryLoadLevelLua())
        {
            CallFuncRpc(ControllerStartFight, SendTo.Everyone, Delivery.OrderWise);
        }
    }
    [Rpc]
    private void ControllerStartFight()
    {
        Tool.FightController.StartFight();
        Tool.PageManager.PageActive(PageManager.PageType.Prepare, false);
        Tool.PageManager.PageActive(PageManager.PageType.PlayMode, true);
    }
    public void BackToPrepare()
    {
        CallFuncRpc(RecvBackToPrepare, SendTo.Everyone,Delivery.Reliable);

        Invoke(nameof(LateBackToPrepare), 0.05f);
    }
    [Rpc]
    private void RecvBackToPrepare()
    {
        Tool.FightController.StopFightLocal();

        Tool.SceneController.DestroyAllTargetsLocal();
        Tool.SceneController.DestroyLevel();

        Tool.SceneController.CreateLevel(1);
        Tool.PageManager.PageActive(PageManager.PageType.PlayMode,false);
        Tool.PageManager.PageActive(PageManager.PageType.Prepare, true);
    }
    private void LateBackToPrepare()
    {
        foreach (var i in ServerDataContainer.GetAllValues())
        {
            EnsInstance.EnsSpawner.CreateServerRpc(SendTo.Everyone, Tool.PrefabManager.NonSkillPlayerCollection.CollectionId, i.id.ToString());
        }
    }
    #endregion



    private bool restarting = false;
    public void RestartGame()
    {
        if (restarting) return;
        restarting= true;

        Debug.LogWarning("重置了游戏");

        EnsInstance.Corr.ShutDown();
        Tool.FightController.StopFightLocal();

        ServerDataContainer.Reset();

        if(Tool.SceneController.Player!=null)Destroy(Tool.SceneController.Player);

        Tool.SceneController.DestroyLevel();
        Tool.SceneController.DestroyNonSkillPlayer();
        Tool.SceneController.DestroyAllTargetsLocal();

        Tool.PageManager.PageActive(PageManager.PageType.Prepare, false);
        Tool.PageManager.PageActive(PageManager.PageType.PlayMode, false);
        Tool.PageManager.PageActive(PageManager.PageType.Home, true);
        Tool.SceneController.CreateLevel(0);
        Tool.SceneController.CreateUnnetPlayer();

        restarting = false;
    }
}