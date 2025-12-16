using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerDataContainer
{
    public static Action OnInfoChanged;
    public struct PlayerDataContainer
    {
        public int id;
        public string name;
        public PlayerDataContainer(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        public PlayerDataContainer(string data)
        {
            var s = data.Split('+', System.StringSplitOptions.RemoveEmptyEntries);
            id = int.Parse(s[0]);
            name = s[1];
        }
        public override string ToString()
        {
            return $"{id}+{name}";
        }
    }

    private static SortedDictionary<int, PlayerDataContainer> PlayerData = new SortedDictionary<int, PlayerDataContainer>();

    public static int[] GetAllKeys()
    {
        return PlayerData.Keys.ToArray();
    }

    public static void Set(PlayerDataContainer playerData)
    {
        if (PlayerData.ContainsKey(playerData.id))
        {
            PlayerData[playerData.id] = playerData;
        }
        else
        {
            PlayerData.Add(playerData.id, playerData);
        }
        OnInfoChanged?.Invoke();
    }
    public static void Remove(int id)
    {
        if (PlayerData.ContainsKey(id)) PlayerData.Remove(id);
        OnInfoChanged?.Invoke();
    }
    public static bool TryGet(int id,out PlayerDataContainer playerDataContainer)
    {
        return PlayerData.TryGetValue(id, out playerDataContainer);
    }
    public static SortedDictionary<int,PlayerDataContainer>.ValueCollection GetAllValues()
    {
        return PlayerData.Values;
    }



    public static void LoadAll(string data)
    {
        string[] s = data.Split('=', StringSplitOptions.RemoveEmptyEntries);
        foreach (var i in s) Set(new PlayerDataContainer(i));
    }
    public static string ReturnAll()
    {
        if (PlayerData.Count == 0) return string.Empty;
        StringBuilder sb = new StringBuilder();
        bool first = true;
        foreach (PlayerDataContainer playerData in PlayerData.Values)
        {
            if (!first) sb.Append('=');
            else first = false;
            sb.Append(playerData.ToString());
        }
        return sb.ToString();
    }



    public static void Reset()
    {
        PlayerData.Clear();
        OnInfoChanged?.Invoke();
    }
    public static string ReplaceInvalidChars(string data)
    {
        return data.Replace('+', '1').Replace('=', '1');
    }
}