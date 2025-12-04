using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public static string Name;
    public static int Id;

    public static void SetData(string data)
    {
        string[] s = data.Split(new char[] { '*' }, System.StringSplitOptions.RemoveEmptyEntries);
        try
        {
            Name = s[0];
            Id = int.Parse(s[1]);
        }
        catch
        {
            Debug.LogWarning("信息不全");
        }
    }
    public static string GetData()
    {
        string s = Name + "*" + Id.ToString();
        return s;
    }
    public static void ResetData()
    {
        Id = Random.Range(100000, 999999);
        Name = Id.ToString();
    }
}