using Ens.Request.Client;
using System;
using System.Collections.Generic;

public class RoomListDedicateServer:Singleton<RoomListDedicateServer>
{
    private List<RoomListUnitInfo> roomInfoList = new List<RoomListUnitInfo>();
    public Action<List<RoomListUnitInfo>> onRoomInfoChanged;
    public Action OnClearRoomListRequired;
    public void OnEnter()
    {
        EnsInstance.OnServerConnect += Flash;
        GetRoomList.OnRecvReply += OnGetRoomList;
        GetInfo.OnRecvReply += OnGetRoomInfo;
        RoomInfoUnit.OnRoomInfoUnitClicked += _JoinRoom;

        StartRecv();
    }

    public void OnExit()
    {
        EnsInstance.OnServerConnect -= Flash;
        GetRoomList.OnRecvReply -= OnGetRoomList;
        GetInfo.OnRecvReply -= OnGetRoomInfo;
        RoomInfoUnit.OnRoomInfoUnitClicked -= _JoinRoom;
    }
    public void StartRecv()
    {
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.Client)
        {
            Flash();
        }
        else
        {
            roomInfoList.Clear();
            onRoomInfoChanged?.Invoke(roomInfoList);

            EnsInstance.Corr.IP = Tool.Instance.ServerIP;
            EnsInstance.Corr.StartClient();
        }
    }

    public void Flash()
    {
        if (EnsInstance.LocalClientId < 0) return;
        roomInfoList.Clear();
        onRoomInfoChanged?.Invoke(roomInfoList);

        GetRoomList.SendRequest(0, 999);
    }

    private void OnGetRoomList(int totalCount, List<int> roomList)
    {
        if (roomList.Count != 0)
        {
            GetInfo.SendRequest(roomList);
        }
        else
        {
            OnGetRoomInfo(new Dictionary<int, string>());
        }
    }

    private void OnGetRoomInfo(Dictionary<int, string> info)
    {
        // 解析房间信息并添加到列表
        foreach (var kvp in info)
        {
            var roomDict = Format.StringToDictionary(kvp.Value, t => t, t => t);
            if (roomDict.TryGetValue("Name", out string name) &&
                roomDict.TryGetValue("State", out string state))
            {
                roomInfoList.Add(new RoomListUnitInfo(name, kvp.Key.ToString(), state, "远程联机"));
                onRoomInfoChanged?.Invoke(roomInfoList);
            }
        }
    }

    public void _CreateRoom()
    {
        if (EnsInstance.LocalClientId < 0) return;
        CreateRoom.SendRequest();
    }
    public void _JoinRoom(string ip)
    {
        if (EnsInstance.LocalClientId < 0) return;
        JoinRoom.SendRequest(int.Parse(ip));
    }
}
/*
public abstract class RoomOperationLogic : MonoBehaviour
{
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Flash();
    public abstract void _CreateRoom();
    public abstract void _JoinRoom(string ip);
}*/