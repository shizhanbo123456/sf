using System;

public class PlayerInfoViewModel:Singleton<PlayerInfoViewModel>
{
    public string PlayerName
    {
        get => PlayerInfo.Name;
        set
        {
            PlayerInfo.Name = value;
            Tool.FileManager.WriteData();
            OnPlayerNameChanged?.Invoke(PlayerName);
        }
    }
    public Action<string> OnPlayerNameChanged;
    public int PlayerId
    {
        get => PlayerInfo.Id;
        set
        {
            PlayerInfo.Id = value;
            Tool.FileManager.WriteData();
            OnPlayerIdChanged.Invoke(PlayerId);
        }
    }
    public Action<int> OnPlayerIdChanged;
    public int Vocation
    {
        get => PlayerInfo.Vocation;
        set
        {
            PlayerInfo.Vocation = value;
            Tool.FileManager.WriteData();
            OnVocationChanged.Invoke(Vocation);
        }
    }
    public Action<int> OnVocationChanged;
}