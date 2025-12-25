using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomScaler : MonoBehaviour
{
    public Vector2 TargetSize = new Vector2(1920, 1080);

    private void Start()
    {
        transform.parent.GetComponent<CanvasScaler>().enabled = false;
    }
    private void Update()
    {
        transform.localScale=new Vector3(Screen.width/TargetSize.x,Screen.height/TargetSize.y,1);
        transform.localPosition=new Vector3(Screen.width/-2,Screen.height/-2,0);
    }
}
