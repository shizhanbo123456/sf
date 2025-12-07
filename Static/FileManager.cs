using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    private bool Initialized = false;
    private bool TemData = false;

    private const string VersionKey = "SF_Version";
    private const string DataKey = "SF_Data";

    public void Awake()
    {
        Tool.FileManager = this;
        Init();
        Transition.ExecuteWithLoading(async () =>
        {
            await CustomLevelLoader.LoadAsync();
            CustomLevelSelector.ProcessData();
            //foreach(var i in CustomLevelSelector.LevelInfo)Debug.Log(i.logic);
        });
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