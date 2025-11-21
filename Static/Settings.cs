using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private void Awake()
    {
        Tool.Settings = this;

        PlayerLayer = LayerMask.NameToLayer("Player");
        FallingPlayerLayer = LayerMask.NameToLayer("FallingPlayer");
        TargetLayer = LayerMask.NameToLayer("Target");
        FallingTargetLayer = LayerMask.NameToLayer("FallingTarget");
    }
    [Header("Layers")]
    public LayerMask Ground;
    public LayerMask Player;
    [HideInInspector] public int PlayerLayer;
    [HideInInspector] public int FallingPlayerLayer;
    [HideInInspector] public int TargetLayer;
    [HideInInspector] public int FallingTargetLayer;
}
