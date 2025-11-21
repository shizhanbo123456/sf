using System;
using System.Collections.Generic;
using System.Text;

public class ServerDataContainer
{
    public static Action OnInfoChanged;
    public struct PlayerDataContainer
    {
        public int id;
        public string name;
        public int vocation;
        public int camp;
        public PlayerDataContainer(int id, string name, int vocation, int camp)
        {
            this.id = id;
            this.name = name;
            this.vocation = vocation;
            this.camp = camp;
        }
        public PlayerDataContainer(string data)
        {
            var s = data.Split('+', System.StringSplitOptions.RemoveEmptyEntries);
            id = int.Parse(s[0]);
            name = s[1];
            vocation = int.Parse(s[2]);
            camp = int.Parse(s[3]);
        }
        public override string ToString()
        {
            return $"{id}+{name}+{vocation}+{camp}";
        }
    }

    private static SortedDictionary<int, PlayerDataContainer> PlayerData = new SortedDictionary<int, PlayerDataContainer>();

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