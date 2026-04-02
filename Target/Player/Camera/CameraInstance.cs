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

    [SerializeField]private Transform Player;
    private int CameraShakeIndex;

    public const float cameraViewScale = 5f;

    public void Init(Transform root)
    {
        Player=root;
        transform.position = Player.position;
        enabled= true;
    }
    private void Update()
    {
        if (Player == null)
        {
            enabled = false;
            CameraShakeIndex = 0;
            return;
        }
        if (CameraShakeIndex > 0)
        {
            transform.position =Player.position+ new Vector3(0, 0.2f * ((CameraShakeIndex % 2 == 0) ? 1 : -1), 0);
            CameraShakeIndex -= 1;
        }
        else if (CameraShakeIndex == 0) transform.position=Player.position;
    }
    public void ShakeCamera(int value = 2)
    {
        CameraShakeIndex = value;
    }
}
