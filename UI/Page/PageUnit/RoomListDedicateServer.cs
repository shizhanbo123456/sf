using Ens.Request.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListDedicateServer : RoomOperationLogic
{
    private RoomListLayout roomListLayout;
    private RoomListLayout RoomListLayout
    {
        get
        {
            if (roomListLayout == null) roomListLayout = GetComponent<RoomListLayout>();
            return roomListLayout;
        }
    }


    public override void OnEnter()
    {
        EnsInstance.OnServerConnect += Flash;
        GetRoomList.OnRecvReply += OnGetRoomList;
        GetInfo.OnRecvReply += OnGetRoomInfo;
        RoomInfoUnit.OnRoomInfoUnitClicked += _JoinRoom;

        StartRecv();
    }

    public override void OnExit()
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
            RoomListLayout.ClearRoomList();
            EnsInstance.Corr.IP = Tool.Instance.ServerIP;
            EnsInstance.Corr.StartClient();
        }
    }

    public override void Flash()
    {
        if (EnsInstance.LocalClientId < 0) return;
        RoomListLayout.ClearRoomList();
        GetRoomList.SendRequest(0,999);
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
                RoomListLayout.AddRoomList(name, kvp.Key.ToString(), state, "远程联机");
            }
        }
    }

    public override void _CreateRoom()
    {
        if (EnsInstance.LocalClientId < 0) return;
        CreateRoom.SendRequest();
    }
    public override void _JoinRoom(string ip)
    {
        if (EnsInstance.LocalClientId < 0) return;
        JoinRoom.SendRequest(int.Parse(ip));
    }
}