using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NonSkillPlayerData : EnsBehaviour
{
    public NonSkillPlayerController PlayerController;
    public SyncPosition SyncPosition;
    public TargetAnim Anim;

    [HideInInspector] public float Jixing;
    [HideInInspector] public float Tengkong;
    [HideInInspector] public float Liantiao;

    public int id;
    public bool isLocalPlayer;
    public bool Initialized;

    [Space]
    [Header("Visual")]
    public Text Recog;
    public Font Font;
    public Image Back;


    public void Init(string data)//由Spawner在生成时传入信息
    {
        id = int.Parse(data);
        isLocalPlayer = FightController.localPlayerId == id;

        ServerDataContainer.TryGet(id, out var p);
        var Name = p.name;
        if (Name!=string.Empty)
        {
            Recog.text = "P" + id + " " + Name;
            Recog.color = Color.white;
            GUIStyle style = new GUIStyle()
            {
                font = Font,
                fontSize = Recog.fontSize,
                fontStyle = Recog.fontStyle
            };
            Texture2D tex = new Texture2D(1024, 16);
            Vector2 size = style.CalcSize(new GUIContent(Recog.text));
            Destroy(tex);
            Back.transform.localScale = new Vector3((size.x + 30) / 100, 0.01f, 1);
        }
        else
        {
            Recog.transform.parent.gameObject.SetActive(false);
        }

        GetComponent<SyncPosition>().Init(this);

        var att=Tool.AttributesManager.GetDynamicAttribute(null);
        Jixing = att.Jixing;
        Tengkong= att.Tengkong;
        Liantiao= att.Liantiao;

        PlayerController.Init(id, this);
        Anim.Init(gameObject);
        if (isLocalPlayer)
        {
            SyncPosition.nomEnabled = true;
            Tool.SceneController.Player = gameObject;
            CameraInstance.instance.Init(transform);
        }
        else
        {
            SyncPosition.nomEnabled= false;
        }

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
