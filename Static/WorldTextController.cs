using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTextController : MonoBehaviour
{
    public GameObject TextPrefab;
    private List<Transform> Texts = new List<Transform>();
    public enum TextColor
    {
        Orange,Green,Blue,Red,Purple,Pink
    }
    public TextColor Reference;
    public List<Color> TextColors = new List<Color>();
    private void Awake()
    {
        Tool.WorldTextController = this;
    }
    /// <summary>
    /// 会向全部玩家显示此文本
    /// </summary>
    public void ShowTextLocal(string text, Vector3 pos, TextColor color)
    {
        pos += new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f));
        var obj = Instantiate(TextPrefab,pos, Quaternion.identity, Tool.SceneController.Level.Canvas);
        var t = obj.transform.GetChild(1).GetComponent<Text>();
        t.text = text;
        t.color = TextColors[(int)color];
        obj.GetComponent<Text>().text=text;
        obj.transform.GetChild(0).GetComponent<Text>().text=text;
        Add(obj.transform);
        Destroy(obj, 0.5f);
    }
    private void Add(Transform t)//有null则插入，反之则删除
    {
        for(int i = 0; i < Texts.Count; i++)
        {
            if (Texts[i]==null)
            {
                Texts[i] = t;
                return;
            }
        }
        Texts.Add(t);
    }
    private void Update()
    {
        for(int i= Texts.Count - 1; i >= 0; i--)
        {
            if (Texts[i]!=null)Texts[i].position+=Vector3.up*Time.deltaTime;
        }
    }
}
