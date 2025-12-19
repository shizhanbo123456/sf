using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnnetPlayerData : MonoBehaviour
{
    public UnnetPlayerController PlayerController;
    public TargetGraphic Anim;

    [HideInInspector] public float Jixing;
    [HideInInspector] public float Tengkong;
    [HideInInspector] public float Liantiao;

    public void Awake()//由Spawner在生成时传入信息
    {
        var att=TargetAttributes.GetGameTimeAttributes(1);
        Jixing = att.Jixing.Value;
        Tengkong = att.Tengkong.Value;
        Liantiao = att.Liantiao.Value;
        att.Release();
        att = null;

        PlayerController.Init(this);
        Anim.Init(gameObject);
        CameraInstance.instance.Init(transform);

        Anim.SetName(TargetGraphic.NullName);
        Anim.SetBarActive(false);

        Tool.SceneController.Player = gameObject;
    }
}
