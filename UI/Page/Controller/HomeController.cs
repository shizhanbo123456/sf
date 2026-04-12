using System;
using System.Collections.Generic;

public class HomeController:Singleton<HomeController>
{
    public int PlayerId => PlayerInfo.Id;
    public string PlayerName => PlayerInfo.Name;
    public bool UseHost = false;//侦听连接的模式
    public bool RoomListActive = false;
    public List<RoomListUnitInfo> infoList=new();

    public void Repaint()
    {
        Tool.PageManager.PageRepaint(PageManager.PageType.Home);
    }
    public void SetPlayerName(string value)
    {
        PlayerInfo.Name = value;
        FileManager.WriteData();
        Repaint();
    }
    public void DedicateServerMatch()
    {
        UseHost = false;
        Tool.DedicateServerMode = true;
        RoomListActive = true;
        RoomListDedicateServer.Instance.OnEnter();
        RoomListDedicateServer.Instance.onRoomInfoChanged += RecordAndRepaint;
        Repaint();
    }
    public void HostMatch()
    {
        UseHost = true;
        Tool.DedicateServerMode = false;
        RoomListActive = true;
        RoomListHost.Instance.OnEnter();
        RoomListHost.Instance.onRoomInfoChanged += RecordAndRepaint;
        Repaint();
    }
    public void ExitMatch()
    {
        RoomListActive = false;
        if (UseHost)
        {
            RoomListHost.Instance.OnExit();
            RoomListHost.Instance.onRoomInfoChanged -= RecordAndRepaint;
        }
        else
        {
            RoomListDedicateServer.Instance.OnExit();
            RoomListDedicateServer.Instance.onRoomInfoChanged -= RecordAndRepaint;
        }
        Repaint();
    }
    private void RecordAndRepaint(List<RoomListUnitInfo> info)
    {
        infoList=info;
        Repaint();
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
}