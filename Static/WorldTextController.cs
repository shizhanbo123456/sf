using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class WorldTextController : EnsBehaviour
{
    private struct TextInfo
    {
        public string text;
        public Vector2 pos;
        public TextColor color;
        public TextInfo(string text,Vector2 pos,TextColor color)
        {
            this.text = text;
            this.pos = pos;
            this.color = color;
        }
        public override string ToString()
        {
            var sb= Tool.stringBuilder;
            sb.Append(text).Append('_').
                Append(pos.x.ToString("F1")).Append('_').
                Append(pos.y.ToString("F2")).Append('_').
                Append((int)color);
            return sb.ToString();
        }
        public TextInfo(string data)
        {
            var s = data.Split('_');
            text= s[0];
            pos = new Vector2(float.Parse(s[1]), float.Parse(s[2]));
            color=(TextColor)int.Parse(s[3]);
        }
    }
    public enum TextColor
    {
        Orange, Green, Blue, Red, Purple, Pink
    }
    private List<TextInfo>TextInfos = new List<TextInfo>(100);

    [SerializeField] private GameObject TextPrefab;
    [SerializeField] private TextColor Reference;
    [SerializeField]private List<Color> TextColors = new List<Color>();
    [Space]
    [SerializeField] private float existTime = 0.5f;
    [SerializeField] private int countPerFrame = 5;
    private GameObjectPool pool;

    private List<Transform> Texts = new List<Transform>();
    private List<float> DestroyTimes = new List<float>();


    private void Awake()
    {
        Tool.WorldTextController = this;
        pool = GameObjectPool.Create(TextPrefab,o=>o.SetActive(false),o=>o.SetActive(true));
    }
    public void ShowTextRpc(string text,Vector2 pos,TextColor color)
    {
        TextInfos.Add(new TextInfo(text, pos, color));
    }
    public override void ManagedUpdate()
    {
        int i = countPerFrame;
        for(i=Mathf.Min(TextInfos.Count-1,i); i >= 0; i--)
        {
            var s = TextInfos[i].ToString();
            CallFuncRpc(ShowTextLocal, SendTo.ExcludeSender,Delivery.Unreliable,s);
            ShowTextLocal(s);
            TextInfos.RemoveAt(i);
        }
    }
    [Rpc]
    public void ShowTextLocal(string data)
    {
        var info=new TextInfo(data);
        var pos = (Vector3)info.pos+new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f));
        var obj = pool.Get(); 
        obj.transform.position = pos;
        obj.transform.SetParent(Tool.SceneController.SingleLevel.Canvas);

        var t = obj.transform.GetChild(1).GetComponent<Text>();
        t.text = info.text;
        t.color = TextColors[(int)info.color];
        obj.GetComponent<Text>().text=info.text;
        obj.transform.GetChild(0).GetComponent<Text>().text=info.text;

        Texts.Add(obj.transform);
        DestroyTimes.Add(Time.time + existTime);
    }
    private void Update()
    {
        for(int i= Texts.Count - 1; i >= 0; i--)
        {
            if (Time.time < DestroyTimes[i])Texts[i].position+=Vector3.up*Time.deltaTime;
            else
            {
                pool.Return(Texts[i].gameObject);
                Texts.RemoveAt(i);
                DestroyTimes.RemoveAt(i);
            }
        }
    }
}
