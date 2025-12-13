using Ens.Request.Client;
using ProtocolWrapper;
using System.Collections.Generic;

public class PrepareController : Singleton<PrepareController>
{
    public bool SettingsPanelActive=false;
    public bool ModeSelectionPanelActive=false;
    public bool HasAuthority = false;
    public string ModeName;
    public Dictionary<string, string> RoomInfo;

    public PrepareController()
    {
        Tool.FightController.OnModeNameChanged+=s =>
        {
            ModeName = s;
            Repaint();
        };
        EnsInstance.OnAuthorityChanged += () =>
        {
            HasAuthority = EnsInstance.HasAuthority;
            Repaint();
        };
        EnsInstance.OnGetInfo += info =>
        {
            if (info.TryGetValue(EnsInstance.PresentRoomId, out var value))
            {
                RoomInfo = Format.StringToDictionary(value, t => t, t => t);
                Repaint();
            }
        };
        EnsInstance.SetInfoSucceed += () =>
        {
            var room = EnsRoom.Instance;
            if (room == null) return;
            var ip = Tool.GetIP();
            if (ip == string.Empty)
            {
                Tool.Notice.ShowMesg("当前网络环境不支持局域网联机");
                return;
            }
            Dictionary<string, string> info = new Dictionary<string, string>
            {
                { "Name", room.Info.ContainsKey("Name") ? room.Info["Name"] : "游戏房间" },
                { "State", room.Info.ContainsKey("State") ? room.Info["State"] : "房间初始化中" },
                { "IP", room.Info.ContainsKey("IP") ? room.Info["IP"] : "未知" }
            };
            Broadcast.AddInfo("RoomInfo", Format.DictionaryToString(info));
        };
    }
    public void Repaint()
    {
        Tool.PageManager.PageRepaint(PageManager.PageType.Prepare);
    }
    public void Enter()
    {
        SettingsPanelActive = false;
        ModeSelectionPanelActive = false;
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.Host)
        {
            Tool.Notice.ShowMesg("房间可以搜索到");
            EnsInstance.Corr.SetServerListening(true);
            Broadcast.StartBroadcast();
        }
        Repaint();
    }

    public void Exit()
    {
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.Host)
        {
            EnsInstance.Corr.SetServerListening(false);
            Broadcast.EndBroadcast();
        }
    }
    public void EnterSettingsPanel()
    {
        GetInfo.SendRequest(new List<int>() { EnsInstance.PresentRoomId });
        SettingsPanelActive = true;
        Repaint();
    }
    public void ExitSettingsPanel(string roomName)
    {
        SettingsPanelActive = false;
        if (EnsInstance.HasAuthority)
        {
            SetInfo.SendRequest(new Dictionary<string, string>() { { "Name", roomName } });
        }
        Repaint();
    }
    public void StartFight()
    {
        Tool.NetworkCorrespondent.StartFight();
    }
    public void ExitRoom()
    {
        Tool.NetworkCorrespondent.RestartGame();
    }
}