using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListLayout : MonoBehaviour
{
    [SerializeField] private List<RoomInfoUnit> Units = new List<RoomInfoUnit>();
    [SerializeField] private Transform Root; // 房间单元的父节点
    private int activeCount = 0; // 当前激活的房间单元数量



    // 清空当前显示的房间列表
    public void ClearRoomList()
    {
        foreach (var unit in Units) unit.gameObject.SetActive(false);
        activeCount = 0;
    }

    // 添加房间信息到列表
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
    public RoomListUnitInfo(string name,string id,string state,string type)
    {
        this.name = name;
        this.id = id;
        this.state = state;
        this.type = type;
    }
}