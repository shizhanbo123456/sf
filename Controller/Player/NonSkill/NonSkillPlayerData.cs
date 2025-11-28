using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonSkillPlayerData : EnsBehaviour
{
    private NonSkillPlayerController PlayerController;
    public TargetGraphic Anim;
    public TargetName targetName;

    [HideInInspector] public float Jixing;
    [HideInInspector] public float Tengkong;
    [HideInInspector] public float Liantiao;

    public int id;
    public bool isLocalPlayer;
    public bool Initialized;
    


    public void Init(string data)//由Spawner在生成时传入信息
    {
        id = int.Parse(data);
        isLocalPlayer = FightController.localPlayerId == id;

        ServerDataContainer.TryGet(id, out var p);
        var Name = p.name;
        if (Name!=string.Empty)
        {
            targetName.text = $"P{id}-{Name}";
            targetName.color= Color.white;
        }
        else
        {
            targetName.gameObject.SetActive(false);
        }

        var att=Tool.AttributesManager.GetDynamicAttribute(null);
        Jixing = att.Jixing;
        Tengkong= att.Tengkong;
        Liantiao= att.Liantiao;

        if (isLocalPlayer)
        {
            PlayerController=gameObject.AddComponent<NonSkillPlayerController>();
            PlayerController.Init(id, this);
        }
        Anim.Init(gameObject);

        Tool.SceneController.Player = gameObject;
        CameraInstance.instance.Init(transform);

        transform.position = Tool.SceneController.Level.GetSpawnPlace();

        Initialized = true;

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
