using AttributeSystem.Attributes;
using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnnetPlayerData : MonoBehaviour
{
    public UnnetPlayerController PlayerController;
    [HideInInspector]public TargetGraphic Graphic;

    [HideInInspector] public float Jixing;
    [HideInInspector] public float Tengkong;
    [HideInInspector] public float Liantiao;

    public void Awake()//由Spawner在生成时传入信息
    {
        Graphic=Instantiate(Tool.PrefabManager.GraphicCollection[0], transform).GetComponent<TargetGraphic>();
        Graphic.transform.position = Vector3.zero;

        var att =TargetAttributes.GetGameTimeAttributes(1);
        Jixing = att.Jixing.Value;
        Tengkong = att.Tengkong.Value;
        Liantiao = att.Liantiao.Value;
        att.Release();
        att = null;

        PlayerController.Init(this);
        Graphic.Init(gameObject);
        CameraInstance.instance.Init(transform);

        Graphic.SetName(TargetGraphic.NullName);
        Graphic.SetBarActive(false);

        Tool.SceneController.Player = gameObject;
    }
}
