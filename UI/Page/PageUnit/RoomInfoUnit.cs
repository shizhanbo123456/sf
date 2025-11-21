using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoUnit : MonoBehaviour
{
    public Text roomName;
    public Text RoomId;
    public Text RoomState;
    public Text RoomType;

    public static Action<string> OnRoomInfoUnitClicked;
    public void OnClicked()
    {
        OnRoomInfoUnitClicked?.Invoke(RoomId.text);
    }
}
