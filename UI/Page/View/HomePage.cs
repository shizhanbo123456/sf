using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePage : BasePage
{
    private bool UseHost=false;//侦听连接的模式
    [Header("名称修改")]
    public InputField InputField;
    public Text Name;
    public Text Id;
    [Header("房间信息")]
    [SerializeField]private GameObject RoomList;
    [SerializeField] private Transform Root; // 房间单元的父节点
    private List<RoomInfoUnit> Units = new List<RoomInfoUnit>();
    private int activeCount = 0; // 当前激活的房间单元数量



    public override void Init()
    {
        Name.text = PlayerInfo.Name;
        InputField.text = Name.text;
        Id.text = PlayerInfo.Id.ToString();

        RoomList.SetActive(false);
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
        RoomList.SetActive(true);
        RoomListDedicateServer.Instance.OnEnter();
        RoomListHost.Instance.onRoomInfoChanged += Relayout;
    }
    public void HostMatch()
    {
        UseHost=true;
        RoomList.SetActive(true);
        RoomListHost.Instance.OnEnter();
        RoomListDedicateServer.Instance.onRoomInfoChanged += Relayout;

        Invoke(nameof(Flash), 1);
    }
    public void ExitMatch()
    {
        RoomList.SetActive(false);
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
        ClearRoomList();
        foreach (var i in infoList) AddRoomList(i.name, i.id, i.state, i.type);
    }

    public void ClearRoomList()
    {
        foreach (var unit in Units) unit.gameObject.SetActive(false);
        activeCount = 0;
    }
    public void AddRoomList(string name, string id, string state, string type)
    {
        activeCount++;
        // 若现有单元不足，动态创建新单元
        if (Units.Count < activeCount)
        {
            var unit = Instantiate(Tool.PrefabManager.RoomInfoUnit, Root).GetComponent<RoomInfoUnit>();
            Units.Add(unit);
        }
        else
        {
            Units[activeCount - 1].gameObject.SetActive(true); // 复用已有单元
        }
        // 赋值房间信息
        var currentUnit = Units[activeCount - 1];
        currentUnit.roomName.text = name;
        currentUnit.RoomId.text = id;
        currentUnit.RoomState.text = state;
        currentUnit.RoomType.text = type;
    }
}

public struct RoomListUnitInfo
{
    public string name;
    public string id;
    public string state;
    public string type;
    public RoomListUnitInfo(string name, string id, string state, string type)
    {
        this.name = name;
        this.id = id;
        this.state = state;
        this.type = type;
    }
}