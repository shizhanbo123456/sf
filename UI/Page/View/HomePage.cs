using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePage : BasePage
{
    public RoomListLayout RoomList;
    private bool UseHost=false;//侦听连接的模式
    [Header("名称修改")]
    public InputField InputField;
    public Text Name;
    public Text Id;



    public override void Init()
    {
        Name.text = PlayerInfo.Name;
        InputField.text = Name.text;
        Id.text = PlayerInfo.Id.ToString();
    }
    private void OnEnable()
    {
        PlayerInfoViewModel.Instance.OnPlayerNameChanged += OnPlayerNameChanged;
    }
    private void OnDisable()
    {
        ExitMatch();
        PlayerInfoViewModel.Instance.OnPlayerNameChanged -= OnPlayerNameChanged;
    }
    private void OnDestroy()
    {
        OnDisable();
    }
    public void FinishEnter()
    {
        string t = InputField.text;
        if (t.Length > 8) t = t.Substring(0, 8);
        else if (t.Length < 2) return;
        PlayerInfo.Name = t;
        
        Tool.FileManager.WriteData();
    }
    private void OnPlayerNameChanged(string t)
    {
        Name.text = t;
        InputField.gameObject.SetActive(false);
    }

    public void DedicateServerMatch()
    {
        UseHost = false;
        RoomList.gameObject.SetActive(true);
        RoomListDedicateServer.Instance.OnEnter();
        RoomListHost.Instance.onRoomInfoChanged += Relayout;
    }
    public void HostMatch()
    {
        UseHost=true;
        RoomList.gameObject.SetActive(true);
        RoomListHost.Instance.OnEnter();
        RoomListDedicateServer.Instance.onRoomInfoChanged += Relayout;

        Invoke(nameof(Flash), 1);
    }
    public void ExitMatch()
    {
        RoomList.gameObject.SetActive(false);
        if (UseHost)
        {
            RoomListHost.Instance.OnExit();
            RoomListHost.Instance.onRoomInfoChanged -= Relayout;
        }
        else
        {
            RoomListDedicateServer.Instance.OnExit();
            RoomListDedicateServer.Instance.onRoomInfoChanged-=Relayout;
        }
    }
    public void Flash()
    {
        if (UseHost) RoomListHost.Instance.Flash();
        else RoomListDedicateServer.Instance.Flash();
    }
    public void CreateRoom()
    {
        if (UseHost) RoomListHost.Instance._CreateRoom();
        else RoomListDedicateServer.Instance._CreateRoom();
    }
    private void Relayout(List<RoomListUnitInfo> infoList)
    {
        RoomList.ClearRoomList();
        foreach (var i in infoList) RoomList.AddRoomList(i.name, i.id, i.state, i.type);
    }
}
