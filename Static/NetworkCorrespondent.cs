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
            Debug.Log("加入了房间" + EnsInstance.PresentRoomId);
            SendData();
        };
        EnsInstance.OnCreateRoom +=()=>
        {
            Debug.Log("创建了房间" + EnsInstance.PresentRoomId);
            SendData();
            try
            {
                string ip = Tool.DedicateServerMode ? EnsInstance.PresentRoomId.ToString() : Tool.GetIP();
                Dictionary<string, string> info = new Dictionary<string, string>()
                {
                    { "Name","房间名称"},
                    { "State","匹配中"},
                    { "IP",ip},
                };
                SetInfo.SendRequest(info);
            }
            catch(System.Exception e)
            {
                Debug.LogError("设置房间信息请求失败: " + e.Message);
            }
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
        Tool.SceneController.CreateLevel(SceneController.LevelType.Prepare);

        var player = new ServerDataContainer.PlayerDataContainer(
            EnsInstance.LocalClientId,
            PlayerInfo.Name
            );
        Debug.Log("发送了玩家数据");
        CallFuncRpc(RecvData, SendTo.RoomOwner, Delivery.Reliable, player.ToString());
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


    public void ShowScoreboardRpc(string columnHeader1,string columnHeader2,string rowHeader1,string rowHeader2,string rowHeader3)
    {
        var sb = Tool.stringBuilder;
        sb.Clear();
        sb.Append(columnHeader1).Append('/').Append(columnHeader2).Append("/");
        sb.Append(rowHeader1).Append('/').Append(rowHeader2).Append('/').Append(rowHeader3);
        CallFuncRpc(ShowScoreboardLocal, SendTo.Everyone, Delivery.Reliable,sb.ToString());
    }
    [Rpc]
    private void ShowScoreboardLocal(string content)
    {
        var s=content.Split('/',System.StringSplitOptions.RemoveEmptyEntries);
        string[] columns = new string[] { s[0], s[1] };
        string[] rows = new string[] { s[2], s[3], s[4] };
        PlayModeController.Instance.ShowScoreboard(columns,rows);
    }
    public void HideScoreboardRpc()
    {
        CallFuncRpc(HideScoreboardLocal, SendTo.Everyone, Delivery.Reliable);
    }
    [Rpc]
    private void HideScoreboardLocal()
    {
        PlayModeController.Instance.HideScoreboard();
    }
    public void SetScoreboardTextRpc(int x, int y, string data)
    {
        CallFuncRpc(SetScoreboardTextLocal, SendTo.Everyone, Delivery.Reliable,x,y,data);
    }
    [Rpc]
    private void SetScoreboardTextLocal(int x,int y,string data)
    {
        PlayModeController.Instance.SetScoreboardText(x,y,data);
    }
    public void ShowTitleRpc(string msg)
    {
        CallFuncRpc(ShowTitleLocal, SendTo.Everyone, Delivery.Reliable,msg);
    }
    [Rpc]
    private void ShowTitleLocal(string msg)
    {
        PlayModeController.Instance.ShowTitle(msg);
    }
    public void ShowSubtitleRpc(string msg)
    {
        CallFuncRpc(ShowSubtitleLocal, SendTo.Everyone, Delivery.Reliable, msg);
    }
    [Rpc]
    private void ShowSubtitleLocal(string msg)
    {
        PlayModeController.Instance.ShowSubtitle(msg);
    }
    public void CreateLevelRpc(ushort id,float minimapScale)
    {
        CallFuncRpc(CreateLevelLocal, SendTo.Everyone, Delivery.Reliable, id,minimapScale);
    }
    [Rpc]
    private void CreateLevelLocal(ushort id,float minimapScale)
    {
        Tool.SceneController.CreateLevel(SceneController.LevelType.Fight);
        Tool.SceneController.Level.Init(Tool.LevelCreatorManager.GetLandscapeInfo(id));
        MinimapCamera.cameraScaleFactor=minimapScale;
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
    public void ShowSelectionRpc(string label,string[] messages)
    {
        string message = Format.ListToString(messages, t => t);
        CallFuncRpc(ShowSelectionLocal, SendTo.Everyone, Delivery.Reliable, label, message);
    }
    [Rpc]
    private void ShowSelectionLocal(string label,string message)
    {
        var messages=Format.StringToArray(message,t=>t);
        PlayModeController.Instance.ShowSelection(label, messages);
    }
    public void HideSelectionRpc()
    {
        CallFuncRpc(HideSelectionLocal, SendTo.Everyone, Delivery.Reliable);
    }
    [Rpc]
    private void HideSelectionLocal()
    {
        PlayModeController.Instance.HideSelection();
    }
    public void SelectRpc(int index)
    {
        CallFuncRpc(SelectLocal,SendTo.RoomOwner,Delivery.Reliable, EnsInstance.LocalClientId,index);
    }
    [Rpc]
    private void SelectLocal(short clientId,int index)
    {
        LevelCreator.CustomLevel.Select(clientId,index);
    }
    public void SelectablePointClickedRpc(int index)
    {
        CallFuncRpc(SelectablePointClickedLocal, SendTo.RoomOwner, Delivery.Reliable,EnsInstance.LocalClientId,index);
    }
    [Rpc]
    private void SelectablePointClickedLocal(short clientId, int index)
    {
        LevelCreator.CustomLevel.Select(clientId, index);
    }


    public void TeleportCommandRpc(Dictionary<int,Target> targets, float x, float y)
    {
        CallFuncRpc(TeleportCommandLocal, SendTo.Everyone, Delivery.Strive, Format.ListToString(targets, t =>t.Key.ToString()), x, y);
    }
    private void TeleportCommandLocal(string msg, float x, float y)
    {
        var id = Format.StringToArray(msg, int.Parse);
        foreach (var i in id)
        {
            if (Tool.SceneController.FlattenTargets.TryGetValue(i, out var target))
                target.transform.position = new UnityEngine.Vector3(x, y);
        }
    }
    public void AddEffectCommandRpc(Dictionary<int, Target> targets, ushort id)
    {
        CallFuncRpc(AddEffectCommandLocal, SendTo.Everyone, Delivery.Strive, Format.ListToString(targets, t => t.Key.ToString()), id);
    }
    private void AddEffectCommandLocal(string msg, ushort id)
    {
        var idArray = Format.StringToArray(msg, int.Parse);
        foreach (var i in idArray)
        {
            if (Tool.SceneController.FlattenTargets.TryGetValue(i, out var target))
            {
                if (!target.UpdateLocally) continue;
                target.ApplyEffect(new AttributeSystem.Effect.EffectCollection(Target.LuaEffectId, Tool.LevelCreatorManager.GetEffectInfo(id).effects?.ToArray()));
            }
        }
    }
    public void DoOperationCommandRpc(Dictionary<int, Target> targets, ushort id)
    {
        CallFuncRpc(DoOperationCommandLocal, SendTo.Everyone, Delivery.Strive, Format.ListToString(targets, t => t.Key.ToString()), id);
    }
    private void DoOperationCommandLocal(string msg, ushort id)
    {
        var idArray = Format.StringToArray(msg, int.Parse);
        foreach (var i in idArray)
        {
            if (Tool.SceneController.FlattenTargets.TryGetValue(i, out var target))
            {
                LevelCreator.Executer.OperationExecuter.Execute(id, 0, target);
            }
        }
    }
    public void DamageCommandRpc(Dictionary<int, Target> targets, int value)
    {
        CallFuncRpc(DamageCommandLocal, SendTo.Everyone, Delivery.Strive, Format.ListToString(targets, t => t.Key.ToString()), value);
    }
    private void DamageCommandLocal(string msg, int value)
    {
        var idArray = Format.StringToArray(msg, int.Parse);
        foreach (var i in idArray)
        {
            if (Tool.SceneController.FlattenTargets.TryGetValue(i, out var target))
            {
                if (!target.UpdateLocally) continue;
                target.Damaged(null, value);
            }
        }
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
            CallFuncRpc(ControllerInitFight, SendTo.Everyone, Delivery.Reliable);
            Tool.LevelCreatorManager.SyncInfo(() =>
            {
                CallFuncRpc(ControllerStartFight, SendTo.Everyone, Delivery.Reliable);
            });
        }
        else
        {
            Tool.Notice.ShowMesg("关卡逻辑加载失败");
        }
    }
    [Rpc]
    private void ControllerInitFight()
    {
        TransitionController.Instance.ExecuteSignalOnly(true, "正在同步信息");
        Tool.FightController.InitFight();
    }
    [Rpc]
    private void ControllerStartFight()
    {
        TransitionController.Instance.ExecuteSignalOnly(false,string.Empty);
        Tool.FightController.StartFight();
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

        Tool.SceneController.CreateLevel(SceneController.LevelType.Prepare);
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
        Tool.SceneController.CreateLevel(SceneController.LevelType.Home);
        Tool.SceneController.CreateUnnetPlayer();

        restarting = false;
    }
}