using Ens.Request.Client;
using ProtocolWrapper;
using System.Collections.Generic;
using UnityEngine;

public class RoomListHost : RoomOperationLogic
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
        Broadcast.StartRecv();
        Invoke(nameof(Flash), 1);
        RoomInfoUnit.OnRoomInfoUnitClicked += _JoinRoom;

    }
    public override void OnExit()
    {
        Broadcast.EndRecv();
        RoomInfoUnit.OnRoomInfoUnitClicked -= _JoinRoom;
    }

    public override void Flash()
    {
        RoomListLayout.ClearRoomList();
        if(Broadcast.TryGetContents("RoomInfo",out var c))
        {
            foreach(var i in c)
            {
                var d=Format.StringToDictionary(i.Content,t=>t,t=>t);
                RoomListLayout.AddRoomList(d["Name"], d["IP"], d["State"], "æ÷”ÚÕ¯∑øº‰");
            }
        }
    }

    public override void _CreateRoom()
    {
        EnsInstance.OnServerConnect += CreateRoomOnConnect;
        EnsInstance.Corr.StartHost();
    }
    public void CreateRoomOnConnect()
    {
        EnsInstance.OnServerConnect -= CreateRoomOnConnect;
        CreateRoom.SendRequest();
    }
    public override void _JoinRoom(string ip)
    {
        EnsInstance.OnServerConnect += JoinRoomOnConnect;
        EnsInstance.Corr.IP = ip;
        EnsInstance.Corr.StartClient();
    }
    public void JoinRoomOnConnect()
    {
        EnsInstance.OnServerConnect -= JoinRoomOnConnect;
        JoinRoom.SendRequest(1000);
    }
}