using Ens.Request.Client;
using ProtocolWrapper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparePage : BasePage
{
    [Header("HostOnly")]
    [SerializeField] private InputField RoomNameInput;
    [SerializeField] private GameObject SelectModeButton;
    [Space]
    public Text RoomId;
    [SerializeField] private Text RoomName;
    [SerializeField] private Text Mode;
    public ButtonGroup CampSelected;
    [Space]
    [SerializeField] private GameObject SettingsPanel;


    public override void RegistEvent()
    {
        Tool.FightController.OnModeChange(s =>
        {
            Tool.PageManager.PreparePage.Mode.text = s;
        });
        EnsInstance.OnAuthorityChanged += () =>
        {
            var b=EnsInstance.HasAuthority;
            RoomNameInput.gameObject.SetActive(b);
            SelectModeButton.SetActive(b);
        };
        EnsInstance.OnGetInfo += OnGetInfo;
        EnsInstance.SetInfoSucceed += ()=>SetBroadcastInfo();
        //ServerDataContainer.OnInfoChanged += RelayoutUnit;
    }
    private void OnGetInfo(Dictionary<int,string> info)
    {
        if(info.TryGetValue(EnsInstance.PresentRoomId,out var value))
        {
            var i=Format.StringToDictionary(value, t => t, t => t);
            RoomId.text = i.ContainsKey("IP") ? i["IP"] : "未知";
            RoomName.text = i.ContainsKey("Name") ? i["Name"] : "游戏房间";
            RoomNameInput.text = RoomName.text;
        }
    }
    public static bool SetBroadcastInfo()
    {
        var room = EnsRoom.Instance;
        if (room == null) return false;
        var ip = Tool.GetIP();
        if (ip == string.Empty)
        {
            Tool.Notice.ShowMesg("当前网络环境不支持局域网联机");
            return false;
        }
        Dictionary<string, string> info = new Dictionary<string, string>();
        if (room.Info.ContainsKey("Name"))
        {
            info.Add("Name", room.Info["Name"]);
        }
        else
        {
            info.Add("Name", "游戏房间");
        }
        if (room.Info.ContainsKey("State"))
        {
            info.Add("State", room.Info["State"]);
        }
        else
        {
            room.Info.Add("State", "房间初始化中");
        }
        if (room.Info.ContainsKey("IP"))
        {
            info.Add("IP", room.Info["IP"]);
        }
        else
        {
            room.Info.Add("IP", "(隐藏)");
        }
        Broadcast.AddInfo("RoomInfo", Format.DictionaryToString(info));
        return true;
    }
    public override void Enter()
    {
        SettingsPanel.SetActive(false);
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.Host)
        {
            RoomId.text = Tool.GetIP();
            Tool.Notice.ShowMesg("房间可以搜索到");
            EnsInstance.Corr.SetServerListening(true);
            Broadcast.StartBroadcast();
        }
        else if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.Client)
        {
            RoomId.text = EnsInstance.PresentRoomId.ToString();
        }
    }

    public override void Exit()
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
        SettingsPanel.SetActive(true);
    }
    public void ExitSettingsPanel()
    {
        SettingsPanel.SetActive(false);
        if (EnsInstance.HasAuthority)
        {
            if (RoomNameInput.text.Length < 1 || RoomNameInput.text.Length > 8)
            {
                RoomNameInput.text = "游戏房间";
                Tool.Notice.ShowMesg("房间名长度应在1-8之间");
            }
            SetInfo.SendRequest(new Dictionary<string, string>() { { "Name",RoomNameInput.text} });
        }
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