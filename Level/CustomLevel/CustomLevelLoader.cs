using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public struct CustomLevelText
{
    public int[] path;
    public string joinedPath;
    public string logic;
}
public static class CustomLevelSelector
{
    public static List<CustomLevelText> LevelInfo;
    public static Dictionary<int, string> IntToPart;
    public static Dictionary<string,int> PartToInt;
    private static int nextPartIndex;

    public static void ProcessData()
    {
        foreach(var s in CustomLevelLoader.LevelInfo)
        {
            var t=GetPath(s);
            string[] pathParts = t.Split('-');
            foreach (var part in pathParts)
            {
                if (!PartToInt.ContainsKey(part))
                {
                    AddPathPart(part);
                }
            }
            int[] path = new int[pathParts.Length];
            for(int i = 0; i < pathParts.Length; i++)
            {
                path[i]=PartToInt[pathParts[i]];
            }
            LevelInfo.Add(new CustomLevelText() { path= path,joinedPath=t,logic=s });
        }
    }
    private static string GetPath(string src)
    {
        string header = null;
        for(int i = 0; i < src.Length; i++)
        {
            if (i == '\n')
            {
                header = src.Substring(0, i);
                break;
            }
        }
        if (string.IsNullOrEmpty(header)) throw new Exception("未找到关卡模式信息");
        return header.Trim(new char[]{ '-',' '});
    }
    private static int AddPathPart(string part)
    {
        int i = nextPartIndex++;
        IntToPart.Add(i, part);
        PartToInt.Add(part,i);
        return i;
    }

    public static List<int> GetNextSelectionListIndex(List<int> selectedPath)//传入列表内容为part的id
    {
        List<int>matchedSelectionIndex= new List<int>();
        for (int i = 0; i < LevelInfo.Count; i++)
        {
            if (MatchSelection(LevelInfo[i].path,selectedPath))matchedSelectionIndex.Add(i);
        }
        if (matchedSelectionIndex.Count == 1 && LevelInfo[matchedSelectionIndex[0]].path.Length == selectedPath.Count) return null;
        else
        {
            foreach(var i in matchedSelectionIndex)
            {
                if (LevelInfo[i].path.Length == selectedPath.Count)
                    throw new Exception("部分节点以其它模式的非叶节点为叶节点");
            }
        }
        return matchedSelectionIndex;
    }
    private static bool MatchSelection(int[] fullPath,List<int>selectedPath)
    {
        if (fullPath.Length == 0) throw new Exception("路径加载存在错误");
        if(fullPath.Length<selectedPath.Count)return false;
        if(selectedPath.Count==0) return true;
        for(int i = 0; i < selectedPath.Count; i++)
        {
            if(selectedPath[i] != fullPath[i])return false;
        }
        return true;
    }

    public static CustomLevelText GetCustomLevelText(List<int>list)
    {
        foreach(var i in LevelInfo)
        {
            if (i.path.Length != list.Count) continue;
            if(MatchSelection(i.path,list)) return i;
        }
        throw new Exception("未找到选择的模式");
    }
}
public static class CustomLevelLoader
{
    public static List<string> LevelInfo;
    private static string path => Path.Combine(Application.streamingAssetsPath, "CustomLevel");
    public static async Task LoadAsync()
    {
        await Task.Delay(1);
    }
}