using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public static MinimapCamera Instance;
    [SerializeField]private List<Camera>Cameras = new List<Camera>();
    public static float cameraScaleFactor;

    private void Awake()
    {
        Instance= this;
    }
    private void Update()
    {
        transform.position = CameraInstance.instance.transform.position;
        float scale = CameraInstance.cameraViewScale;
        foreach(var c in Cameras)
        {
            c.orthographicSize = scale * cameraScaleFactor;
        }
    }
}
