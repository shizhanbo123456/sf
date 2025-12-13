using Ens.Request.Client;
using ProtocolWrapper;
using System;
using System.Collections.Generic;

public class RoomListHost:Singleton<RoomListHost>
{
    private List<RoomListUnitInfo>roomInfoList=new List<RoomListUnitInfo>();
    public Action<List<RoomListUnitInfo>> onRoomInfoChanged;
    public void OnEnter()
    {
        Broadcast.StartRecv();
        RoomInfoUnit.OnRoomInfoUnitClicked += _JoinRoom;

    }
    public void OnExit()
    {
        Broadcast.EndRecv();
        RoomInfoUnit.OnRoomInfoUnitClicked -= _JoinRoom;
    }

    public void Flash()
    {
        roomInfoList.Clear();
        onRoomInfoChanged?.Invoke(roomInfoList);
        if (Broadcast.TryGetContents("RoomInfo", out var c))
        {
            foreach (var i in c)
            {
                var d = Format.StringToDictionary(i.Content, t => t, t => t);
                roomInfoList.Add(new RoomListUnitInfo(d["Name"], d["IP"], d["State"], "æ÷”ÚÕ¯∑øº‰"));
                onRoomInfoChanged?.Invoke(roomInfoList);
            }
        }
    }

    public void _CreateRoom()
    {
        EnsInstance.OnServerConnect += CreateRoomOnConnect;
        EnsInstance.Corr.StartHost();
    }
    public void CreateRoomOnConnect()
    {
        EnsInstance.OnServerConnect -= CreateRoomOnConnect;
        CreateRoom.SendRequest();
    }
    public void _JoinRoom(string ip)
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