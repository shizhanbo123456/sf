using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    [SerializeField] private bool Initialized = false;
    private bool TemData = false;

    private string VersionKey = "SF_Version";
    private string DataKey = "SF_Data";

    public void Awake()
    {
        Tool.FileManager = this;
        Init();
    }
    public void Init()
    {
        if (Initialized) return;
        try
        {
            if (Exists())
            {
                ReadData();
            }
            else
            {
                PlayerInfo.ResetData();
                WriteData();
            }
            Initialized = true;
        }
        catch(System.Exception)
        {
            TemData = true;
            PlayerInfo.ResetData();
            PlayerInfo.Name += "T";
            Initialized = true;
        }
    }
    private void ReadData()
    {
        if (TemData) return;
        var s = PlayerPrefs.GetString(DataKey);
        PlayerInfo.SetData(s);
    }
    private bool Exists()
    {
        if (TemData) return false;
        return PlayerPrefs.HasKey(VersionKey) && PlayerPrefs.GetString(VersionKey)[0] ==Application.version[0];
    }
    public void WriteData()
    {
        if (TemData) return;
        PlayerPrefs.SetString(DataKey , PlayerInfo.GetData());
    }
    private void OnApplicationQuit()
    {
        WriteData();
    }
}