using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private void Awake()
    {
        Tool.BackgroundController = this;
    }

    public List<Transform> Grapic;
    public List<SpriteRenderer> TintTargets = new List<SpriteRenderer>();
    public Color Tint;
    public float Light = 0.2f;
    public float Fog = 0.1f;

    private static GameObject MainCharacter
    {
        get
        {
            return Tool.SceneController.Player;
        }
    }

    private Vector3 LastFramePosition;
    private Vector2 v;
    [Space]
    [SerializeField] private Vector2 Speed = new Vector2(10, 10);
    private Vector2 TargetOffset;
    private Vector2 PresentOffset;
    [SerializeField] private float Smooth = 0.99f;
    [Space]
    [SerializeField] private float MaxOffset = 10;
    [SerializeField] private float k=1;
    [SerializeField] private float b=1;

    [Space]
    [Header("ExtraWork")]
    [SerializeField] private Transform Vine;
    [SerializeField] private Transform BG3;

    public void UpdateColor(Color c)
    {
        foreach (var i in TintTargets) i.color = c;
        if (Tool.PageManager)
        {
            float ave = c.r + c.g + c.b;
            ave = 1 - ave / 3 - Light;
            Tool.PageManager.BgEffect.color = new Color(c.r * Fog, c.g * Fog, c.b * Fog, ave);
        }
    }

    private void Update()
    {
        if (MainCharacter == null) return;
        
        v=MainCharacter.transform.position-LastFramePosition;
        LastFramePosition =MainCharacter.transform.position;
        TargetOffset=new Vector2(Mathf.Clamp(-v.x*Speed.x, -MaxOffset, MaxOffset), Mathf.Clamp(v.y*Speed.y, -MaxOffset, MaxOffset));
        PresentOffset=PresentOffset*Smooth+TargetOffset*(1-Smooth);
        for(int i = 0; i < Grapic.Count; i++)
        {
            Grapic[i].localPosition = new Vector3(PresentOffset.x * (k*i+b), PresentOffset.y * (k * i + b), Grapic[i].localPosition.z);
        }


        Vine.localPosition=BG3.localPosition;
    }
}
