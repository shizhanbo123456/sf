using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileManager
{
    private static bool Initialized = false;
    private static bool TemData = false;

    private const string VersionKey = "SF_Version";
    private const string DataKey = "SF_Data";

    public static void Init()
    {
        if (Initialized) return; 
        TransitionController.Instance.ExecuteWithLoading(async () =>
        {
            TransitionController.Instance.SetLabel("正在加载关卡逻辑");
            await CustomLevelLoader.LoadAsync();
            CustomLevelSelector.ProcessData();

            await TemplateLoader.LoadAsync();
        });
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
    private static void ReadData()
    {
        if (TemData) return;
        var s = PlayerPrefs.GetString(DataKey);
        PlayerInfo.SetData(s);
    }
    private static bool Exists()
    {
        if (TemData) return false;
        return PlayerPrefs.HasKey(VersionKey) && PlayerPrefs.GetString(VersionKey)[0] ==Application.version[0];
    }
    public static void WriteData()
    {
        if (TemData) return;
        PlayerPrefs.SetString(DataKey , PlayerInfo.GetData());
    }
}