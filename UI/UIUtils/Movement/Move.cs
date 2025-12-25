using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector3 DeltaPosition;
    public float Rate=0.97f;
    public bool On;
    public bool StartOnSetOn = false;

    private Vector3 Origin;
    private Vector3 Target;
    private void Awake()
    {
        if (!StartOnSetOn)
        {
            Origin = transform.localPosition;
            Target = transform.localPosition + DeltaPosition;
        }
        else 
        {
            Origin = transform.localPosition - DeltaPosition;
            Target = transform.localPosition;
        }
    }
    void Update()
    {
        if (On) transform.localPosition = Target * (1-Rate) + transform.localPosition * Rate;
        else transform.localPosition = Origin*(1-Rate)+transform.localPosition *Rate;
    }
    public void SetOn(bool on)
    {
        On= on;
    }
    public void SetPosition(bool on)
    {
        On = on;
        if (On) transform.localPosition = Target;
        else transform.localPosition = Origin;
    }
}
