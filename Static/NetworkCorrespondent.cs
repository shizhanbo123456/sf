using Ens.Request.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCorrespondent : EnsBehaviour
{
    private void Awake()
    {
        Tool.NetworkCorrespondent = this;

        //去除在游玩阶段的连接
        EnsInstance.OnClientEnter += (id) =>
        {
            if(EnsInstance.HasAuthority && 
            Tool.PageManager.PresentPageType==PageManager.PageType.PlayMode)
            {
                CallFuncRpc(nameof(RestartGame), new List<int>() { id });
            }
        };
        //客户端断开连接时(准备或战斗)删除技能信息，物体，阵营图标
        EnsInstance.OnClientExit += (id) =>
        {
            DeleteData(id);
            Debug.Log("id为" + id + "的玩家断开连接");
            if (Tool.SceneController.NonSkillPlayers.ContainsKey(id)) Tool.SceneController.NonSkillPlayers[id].DestroyLocal();
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
                { "IP",EnsInstance.PresentRoomId==1000?Tool.GetIP():EnsInstance.PresentRoomId.ToString()},
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
        Tool.PageManager.TurnPage(PageManager.PageType.Prepare);
        Tool.SceneController.CreateLevel(1);

        var player = new ServerDataContainer.PlayerDataContainer(
            EnsInstance.LocalClientId,
            PlayerInfo.Name
            );
        CallFuncRpc(nameof(RecvData), SendTo.RoomOwner, player.ToString(),KeyLibrary.KeyFormatType.DisorderConfirm);
        Tool.FightController.SelectedMode = Tool.FightController.SelectedMode;
    }

    //接收到新增客户端的信息
    private void RecvData(string data)//Server
    {
        var playerData = new ServerDataContainer.PlayerDataContainer(data);

        //防止启动Host时没有数据而出错
        if (ServerDataContainer.GetAllValues().Count != 0)
        {
            string AllData = ServerDataContainer.ReturnAll();
            CallFuncRpc(nameof(RecvAllData), new List<int>() { playerData.id }, AllData, KeyLibrary.KeyFormatType.OrderWise);
        }

        CallFuncRpc(nameof(RecvNewData), SendTo.Everyone, data, KeyLibrary.KeyFormatType.OrderWise);
        Tool.FightController.SyncModeHeaderTo(playerData.id);

        StartCoroutine(RecreatePlayers(playerData));
    }
    private void RecvAllData(string data)
    {
        ServerDataContainer.LoadAll(data);
    }
    private void RecvNewData(string data)
    {
        var playerData = new ServerDataContainer.PlayerDataContainer(data);
        ServerDataContainer.Set(playerData);
    }
    public IEnumerator RecreatePlayers(ServerDataContainer.PlayerDataContainer playerData)
    {
        yield return 3;
        foreach (var i in Tool.SceneController.NonSkillPlayers.Values)
        {
            EnsInstance.EnsSpawner.RespawnCheckServerRpc(i.GetComponent<NonSkillPlayerCollection>(), i.id.ToString(),KeyLibrary.KeyFormatType.OrderWise);
        }
        EnsInstance.EnsSpawner.CreateServerRpc(Tool.PrefabManager.NonSkillPlayerCollection.CollectionId, SendTo.Everyone,
            playerData.id.ToString(),KeyLibrary.KeyFormatType.OrderWise);
    }

    

    public void DeleteData(int id)
    {
        ServerDataContainer.Remove(id);
    }
    #endregion


    public void SetScoreboardActiveRpc(bool active)
    {
        CallFuncRpc(nameof(SetScoreboardActiveLocal), SendTo.Everyone, active ? "1" : "0", KeyLibrary.KeyFormatType.DisorderConfirm);
    }
    private void SetScoreboardActiveLocal(string data)
    {
        var b = data == "1";
        Tool.PageManager.PlayModePage.Scoreboard.gameObject.SetActive(b);
    }
    public void SetScoreboardTextRpc(int x, int y, string data)
    {
        if (!Tool.PageManager.PlayModePage.Scoreboard.gameObject.activeSelf) return;
        var sb=Tool.stringBuilder;
        sb.Clear();
        sb.Append(x).Append('_').Append(y).Append('_').Append(data);
        CallFuncRpc(nameof(SetScoreboardTextLocal), SendTo.Everyone, sb.ToString(), KeyLibrary.KeyFormatType.DisorderConfirm);
    }
    private void SetScoreboardTextLocal(string data)
    {
        string[] s = data.Split('_');
        Tool.PageManager.PlayModePage.Scoreboard.SetText(int.Parse(s[0]), int.Parse(s[1]), s[2]);
    }


    #region//开始战斗
    public void StartFight()//(id,camp)
    {
        Transition.ExecuteWithLoading(Tool.FightController.SyncLevel, (b) =>
        {
            if (b) CallFuncRpc(nameof(ControllerStartFight), SendTo.Everyone, KeyLibrary.KeyFormatType.OrderWise);
            else RestartGame();
        });
    }
    private void ControllerStartFight()
    {
        Tool.FightController.StartFight();
        Tool.PageManager.TurnPage(PageManager.PageType.PlayMode);
    }
    public void BackToPrepare()
    {
        CallFuncRpc(nameof(RecvBackToPrepare), SendTo.Everyone);

        Invoke(nameof(LateBackToPrepare), 0.05f);
    }
    private void RecvBackToPrepare()
    {
        Tool.FightController.StopFightLocal();

        Tool.SceneController.DestroyAllTargetsLocal();
        Tool.SceneController.DestroyLevel();

        Tool.SceneController.CreateLevel(1);
        Tool.PageManager.TurnPage(PageManager.PageType.Prepare);
    }
    private void LateBackToPrepare()
    {
        foreach (var i in ServerDataContainer.GetAllValues())
        {
            EnsInstance.EnsSpawner.CreateServerRpc(Tool.PrefabManager.NonSkillPlayerCollection.CollectionId, SendTo.Everyone, i.id.ToString(),KeyLibrary.KeyFormatType.DisorderConfirm);
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


        Tool.SceneController.DestroyLevel();
        Tool.SceneController.DestroyNonSkillPlayer();
        Tool.SceneController.DestroyAllTargetsLocal();

        Tool.PageManager.TurnPage(PageManager.PageType.Home);
        Tool.SceneController.CreateLevel(0);
        Tool.SceneController.CreateUnnetPlayer();

        restarting = false;
    }
}