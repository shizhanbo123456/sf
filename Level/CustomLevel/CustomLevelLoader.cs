using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class CustomLevelLoader
{
    public static Dictionary<string, string> LevelInfo;
    private static string path => Path.Combine(Application.streamingAssetsPath, "CustomLevel");
    public static async Task LoadAsync()
    {
        await Task.Delay(1);
    }
}