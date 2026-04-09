using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class WorldTextController : EnsBehaviour
{
    public enum TextColor
    {
        Orange, Green, Blue, Red, Purple, Pink
    }

    [SerializeField] private GameObject TextPrefab;
    [SerializeField] private TextColor Reference;
    [SerializeField]private List<Color> TextColors = new List<Color>();
    [Space]
    [SerializeField] private float existTime = 0.5f;
    private GameObjectPool pool;

    private List<Transform> Texts = new List<Transform>();
    private List<float> DestroyTimes = new List<float>();


    private int messageLeftForCurrentFrame = 0;


    private void Awake()
    {
        Tool.WorldTextController = this;
        pool = GameObjectPool.Create(TextPrefab,
            o=>
            { 
                o.SetActive(false); 
                o.transform.SetParent(transform);
            }, 
            o => 
            { 
                o.SetActive(true); 
            });
    }
    public void ShowMissRpc(short defenser)
    {
        if (messageLeftForCurrentFrame == 0) return;
        messageLeftForCurrentFrame--;
        var t = Tool.SceneController.GetTarget(defenser);
        if (t == null) return;
        ShowText("miss", TextColor.Blue, t.transform.position);
        CallFuncRpc(ShowMissLocal, SendTo.ExcludeSender, Delivery.Unreliable,defenser);
    }
    [Rpc]
    private void ShowMissLocal(short defenser)
    {
        var t = Tool.SceneController.GetTarget(defenser);
        if (t == null) return;
        if (!Visiable(t.transform.position)) return;
        ShowText("miss", TextColor.Blue, t.transform.position);
    }
    public void ShowDamageRpc(short defenser,int value)
    {
        if (messageLeftForCurrentFrame == 0) return;
        messageLeftForCurrentFrame--;
        var t = Tool.SceneController.GetTarget(defenser);
        if (t == null) return;
        ShowText(value.ToString(), TextColor.Orange, t.transform.position);
        CallFuncRpc(ShowDamageLocal,SendTo.ExcludeSender,Delivery.Unreliable,defenser,value);
    }
    [Rpc]
    private void ShowDamageLocal(short defenser, int value)
    {
        var t = Tool.SceneController.GetTarget(defenser);
        if (t == null) return;
        if (!Visiable(t.transform.position)) return;
        ShowText(value.ToString(), TextColor.Orange, t.transform.position);
    }
    public void ShowStrikeDamageRpc(short defenser, int value)
    {
        if (messageLeftForCurrentFrame == 0) return;
        messageLeftForCurrentFrame--;
        var t = Tool.SceneController.GetTarget(defenser);
        if (t == null) return;
        ShowText(value.ToString(), TextColor.Red, t.transform.position);
        CallFuncRpc(ShowStrikeDamageLocal, SendTo.ExcludeSender, Delivery.Unreliable, defenser, value);
    }
    [Rpc]
    private void ShowStrikeDamageLocal(short defenser, int value)
    {
        var t = Tool.SceneController.GetTarget(defenser);
        if (t == null) return;
        if (!Visiable(t.transform.position)) return;
        ShowText(value.ToString(), TextColor.Red, t.transform.position);
    }
    private void ShowText(string msg,TextColor color,Vector3 pos)
    {
        var obj = pool.Get();
        pos +=new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f));
        obj.transform.position = pos;
        obj.transform.SetParent(Tool.SceneController.SingleLevel.Canvas);

        var t = obj.transform.GetChild(1).GetComponent<Text>();
        t.text = msg;
        t.color = TextColors[(int)color];
        obj.GetComponent<Text>().text = msg;
        obj.transform.GetChild(0).GetComponent<Text>().text = msg;

        Texts.Add(obj.transform);
        DestroyTimes.Add(Time.time + existTime);
    }
    private bool Visiable(Vector3 pos)
    {
        var p=CameraInstance.instance.transform.position - pos;
        return Mathf.Abs(p.y)<7&&Mathf.Abs(p.x)<20;
    }
    private void Update()
    {
        for (int i = Texts.Count - 1; i >= 0; i--)
        {
            if (Time.time < DestroyTimes[i]) Texts[i].position += Vector3.up * Time.deltaTime;
            else
            {
                pool.Return(Texts[i].gameObject);
                Texts.RemoveAt(i);
                DestroyTimes.RemoveAt(i);
            }
        }
        messageLeftForCurrentFrame = 5;
    }
}
