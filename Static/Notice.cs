using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notice : MonoBehaviour
{
    private GameObjectPool pool;
    private List<CanvasGroup> Units=new List<CanvasGroup>();
    private List<float> SpawnTime = new List<float>();
    [SerializeField] private GameObject Obj;
    private Transform Canvas;
    private void Awake()
    {
        Tool.Notice = this;
        pool= GameObjectPool.Create(Obj);
    }
    private void Start()
    {
        Canvas = Tool.PageManager.DynamicCanvas.transform;
        ShowMesg("µ±Ç°°æ±¾ : "+Application.version);
    }

    public void ShowMesg(string message)
    {
        var n=pool.Get();
        n.transform.localScale = Vector3.one * (Screen.height / 1080f);
        n.transform.SetParent(Canvas);
        n.transform.position = new Vector3(0, -1000, 0);
        var cg=n.GetComponent<CanvasGroup>();
        if(!cg)cg=n.AddComponent<CanvasGroup>();
        Units.Add(cg);
        Units[Units.Count-1].GetComponent<Text>().text = message;
        SpawnTime.Add(0);
    }
    private void Update()
    {
        for(int i= Units.Count - 1; i >= 0; i--)
        {
            SpawnTime[i] += Time.deltaTime;
            if (SpawnTime[i] > 2.3)
            {
                SpawnTime.RemoveAt(i);
                pool.Return(Units[i].gameObject);
                Units.RemoveAt(i);
            }
            else
            {
                Units[i].transform.localPosition = new Vector3(0, GetY(SpawnTime[i]));
                Units[i].alpha = GetA(SpawnTime[i]);
            }
        }
    }
    private float GetY(float t)
    {
        if (t < 0.3) return Screen.height*(0.17f+t*0.1f);
        else if(t<1.3)    return Screen.height * 0.2f;
        else
        {
            return Screen.height * (0.2f + (t-1.3f) / 5);
        }
    }
    private float GetA(float t)
    {
        if (t < 0.3f)
        {
            return t / 0.3f;
        }
        else if (t < 1.3f)
        {
            return 1;
        }
        else
        {
            return 2.3f - t;
        }
    }
}
