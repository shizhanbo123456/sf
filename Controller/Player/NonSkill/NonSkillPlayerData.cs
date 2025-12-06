using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonSkillPlayerData : EnsBehaviour
{
    private NonSkillPlayerController PlayerController;
    public TargetGraphic Anim;

    [HideInInspector] public float Jixing;
    [HideInInspector] public float Tengkong;
    [HideInInspector] public float Liantiao;

    [HideInInspector]public int id;
    [HideInInspector]public bool isLocalPlayer;

    public void Init(string data)//由Spawner在生成时传入信息
    {
        id = int.Parse(data);
        isLocalPlayer = FightController.localPlayerId == id;

        ServerDataContainer.TryGet(id, out var p);
        var Name = p.name;
        Anim.SetName($"P{id}-{Name}", Color.white);

        var att=Tool.AttributesManager.GetDynamicAttribute(null);
        Jixing = att.Jixing;
        Tengkong= att.Tengkong;
        Liantiao= att.Liantiao;

        if (isLocalPlayer)
        {
            PlayerController=gameObject.AddComponent<NonSkillPlayerController>();
            PlayerController.Init(id, this);
        }

        Tool.SceneController.Player = gameObject;
        CameraInstance.instance.Init(transform);

        Anim.Init(gameObject);
        Anim.SetBarActive(false);

        transform.position = Tool.SceneController.Level.GetPos(0.5f,0.9f);

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
