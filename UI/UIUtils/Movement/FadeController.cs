using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    private CanvasGroup canvasgroup;
    private bool On = true;
    private float alpha;
    public Vector3 HidePlace;
    private Vector3 OriginPlace;
    public float RatePerSec = 1f;

    private bool Initialized = false;

    public void Init()
    {
        Initialized= true;
        canvasgroup= GetComponent<CanvasGroup>();
        if (canvasgroup==null) canvasgroup = gameObject.AddComponent<CanvasGroup>();
        OriginPlace = transform.localPosition;
        HidePlace *= Screen.width/1920;
        HidePlace += OriginPlace;
    }
    private void Update()
    {
        if (!Initialized) Init();
        if (On && alpha <= 1f)
        {
            alpha += RatePerSec * Time.deltaTime;
            transform.localPosition = HidePlace * (1 - alpha) + OriginPlace * alpha;
            canvasgroup.alpha = alpha;
            if (alpha >= 1f)
                transform.localPosition = OriginPlace;
        }
        else if (!On && alpha > 0f)
        {
            alpha -= RatePerSec * Time.deltaTime;
            transform.localPosition = HidePlace * (1 - alpha) + OriginPlace * alpha;
            canvasgroup.alpha = alpha;
        }
        else if (!On && alpha <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
    public void SetOn(bool on)//立即变化
    {
        if (!Initialized) Init();
        if(canvasgroup==null)canvasgroup=GetComponent<CanvasGroup>();
        if (canvasgroup == null) canvasgroup = gameObject.AddComponent<CanvasGroup>();
        On = on;
        alpha = On ? 1f : 0f;
        if (On) gameObject.SetActive(true);
        else canvasgroup.alpha = alpha;
    }
    public void SetFade(bool on)//缓慢变化
    {
        if (!Initialized) Init();
        On = on;
        canvasgroup.alpha = alpha;
        if (On) gameObject.SetActive(true);
    }
    public void ReOn()
    {
        if (!Initialized) Init();
        SetOn(false);
        SetFade(true);
        canvasgroup.alpha = 0;
    }
}
