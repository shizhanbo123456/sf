using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    public static CameraInstance instance;
    private void Awake()
    {
        instance = this;
    }

    private Transform CameraRoot;
    private int CameraShakeIndex;

    public void Init(Transform root)
    {
        CameraRoot=root;
        transform.position = CameraRoot.position;
        enabled= true;
    }
    private void Update()
    {
        if (CameraRoot == null)
        {
            enabled = false;
            CameraShakeIndex = 0;
            return;
        }
        if (CameraShakeIndex > 0)
        {
            transform.position =CameraRoot.position+ new Vector3(0, 0.2f * ((CameraShakeIndex % 2 == 0) ? 1 : -1), 0);
            CameraShakeIndex -= 1;
        }
        else if (CameraShakeIndex == 0) transform.position=CameraRoot.position;
    }
    public void ShakeCamera(int value = 2)
    {
        CameraShakeIndex = value;
    }
}
