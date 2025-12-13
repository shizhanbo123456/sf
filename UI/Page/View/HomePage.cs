using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePage : BasePage
{
    private static HomeController controller => HomeController.Instance;
    [Header("名称修改")]
    public InputField InputField;
    public Text Name;
    public Text Id;
    [Header("房间信息")]
    [SerializeField]private GameObject RoomList;
    [SerializeField] private Transform Root; // 房间单元的父节点
    private List<RoomInfoUnit> Units = new List<RoomInfoUnit>();
    private int activeCount = 0; // 当前激活的房间单元数量


    public override void Repaint()
    {
        InputField.text = controller.PlayerName;
        Name.text = controller.PlayerName;
        Id.text = controller.PlayerId.ToString();

        RoomList.SetActive(controller.RoomListActive);
        Relayout();
    }
    private void OnEnable()
    {
        Repaint();
    }
    private void OnDisable()
    {
        ExitMatch();
    }
    public void FinishEnter()
    {
        string t = InputField.text;
        if (t.Length > 8|| t.Length < 2)
        {
            Tool.Notice.ShowMesg("名字长度需要在2-8个字符内");
            Repaint();
            return;
        }
        controller.SetPlayerName(t);
    }
    public void DedicateServerMatch() => controller.DedicateServerMatch();
    public void HostMatch()
    {
        controller.HostMatch();

        //Invoke(nameof(Flash), 1);
    }
    public void ExitMatch()=>controller.ExitMatch();
    public void Flash()=>controller.Flash();
    public void CreateRoom()=>controller.CreateRoom();
    private void Relayout()
    {
        ClearRoomList();
        foreach (var i in controller.infoList) AddRoomList(i.name, i.id, i.state, i.type);
    }
    private void ClearRoomList()
    {
        foreach (var unit in Units) unit.gameObject.SetActive(false);
        activeCount = 0;
    }
    private void AddRoomList(string name, string id, string state, string type)
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