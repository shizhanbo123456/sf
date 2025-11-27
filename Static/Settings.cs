using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private void Awake()
    {
        Tool.Settings = this;

        TargetLayer = LayerMask.NameToLayer("Target");
        FallingTargetLayer = LayerMask.NameToLayer("FallingTarget");
    }
    [Header("Layers")]
    public LayerMask Ground;
    [HideInInspector] public int TargetLayer;
    [HideInInspector] public int FallingTargetLayer;
}
