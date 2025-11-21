using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePage : BasePage
{
    public RoomListLayout RoomList;
    public RoomOperationLogic RoomOperationLogic;
    [Header("Ãû³ÆÐÞ¸Ä")]
    public InputField InputField;
    public Text Name;
    public Text Id;
    private bool Initialized;




    private void Update()
    {
        if (!Initialized)
        {
            if (PlayerInfo.Name != string.Empty)
            {
                Initialized = true;
                Name.text = PlayerInfo.Name;
                InputField.text= Name.text;
                Id.text = PlayerInfo.Id.ToString();
                enabled = false;
            }
        }
    }
    public void FinishEnter()
    {
        string t = InputField.text;
        if (t.Length > 8) t = t.Substring(0, 8);
        else if (t.Length < 2) return;
        PlayerInfo.Name = t;
        Name.text = t;
        InputField.gameObject.SetActive(false);
        Tool.FileManager.WriteData();
    }

    public void SelectVocation()
    {
        Tool.PageManager.TurnPage(PageManager.PageType.VocationSelection);
    }


    public override void Exit()
    {
        ExitMatch();
    }
    public void DedicateServerMatch()
    {
        RoomOperationLogic=RoomList.gameObject.AddComponent<RoomListDedicateServer>();
        RoomList.gameObject.SetActive(true);
        RoomOperationLogic.OnEnter();
    }
    public void HostMatch()
    {
        RoomOperationLogic=RoomList.gameObject.AddComponent<RoomListHost>();
        RoomList.gameObject.SetActive(true);
        RoomOperationLogic.OnEnter();
    }
    public void ExitMatch()
    {
        RoomList.gameObject.SetActive(false);
        RoomOperationLogic.OnExit();
        Destroy(RoomOperationLogic);
    }
    public void Flash()
    {
        RoomOperationLogic.Flash();
    }
    public void CreateRoom()
    {
        RoomOperationLogic._CreateRoom();
    }
}
