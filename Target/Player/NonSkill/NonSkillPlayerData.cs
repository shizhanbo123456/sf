using AttributeSystem.Attributes;
using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NonSkillPlayerData : EnsBehaviour
{
    private NonSkillPlayerController PlayerController;
    [HideInInspector]public TargetGraphic Graphic;

    [HideInInspector] public float Jixing;
    [HideInInspector] public float Tengkong;
    [HideInInspector] public float Liantiao;

    [HideInInspector]public int id;
    [HideInInspector]public bool isLocalPlayer;

    public void Init(string data)//由Spawner在生成时传入信息
    {
        Graphic=Instantiate(Tool.PrefabManager.GraphicCollection[0],transform).GetComponent<TargetGraphic>();
        Graphic.transform.localPosition = Vector3.zero;

        id = int.Parse(data);
        isLocalPlayer = FightController.localPlayerId == id;

        ServerDataContainer.TryGet((short)id, out var p);
        var Name = p.name;
        Graphic.SetName($"P{id}-{Name}", Color.white);

        var att=TargetAttributes.GetGameTimeAttributes(1);
        Jixing = att.Jixing.Value;
        Tengkong= att.Tengkong.Value;
        Liantiao= att.Liantiao.Value;
        att.Release();
        att = null;

        if (isLocalPlayer)
        {
            PlayerController=gameObject.AddComponent<NonSkillPlayerController>();
            PlayerController.Init(id, this);
            Tool.SceneController.Player = gameObject;
            CameraInstance.instance.Init(transform);
        }


        Graphic.Init(gameObject);
        Graphic.SetBarActive(false);

        transform.position = Vector3.zero;

        Tool.SceneController.NonSkillPlayers.Add(id, this);
    }
    private void OnDestroy()
    {
        if (Tool.SceneController.NonSkillPlayers.TryGetValue(id, out var data))
        {
            Tool.SceneController.NonSkillPlayers.Remove(id);
        }
    }
}
