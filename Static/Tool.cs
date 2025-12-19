using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Tool:MonoBehaviour
{
    public static Tool Instance;
    public static Settings Settings;
    public static PrefabManager PrefabManager;
    public static SpriteManager SpriteManager;
    public static FightController FightController;
    public static PageManager PageManager;
    public static SceneController SceneController;//生成/摧毁 玩家/场景
    public static WorldTextController WorldTextController;
    public static NetworkCorrespondent NetworkCorrespondent;
    public static FileManager FileManager;
    public static Notice Notice;
    public static SubInput SubInput;
    public static BulletManager BulletManager;
    public static LuaManager LuaManager;
    [Space]
    public GameObject WindowsUI;
    public GameObject AndroidUI;
    [Space]
    public string ServerIP = "127.0.0.1";
    public enum TargetPlatform
    {
        Windows,Android
    }
    public TargetPlatform Platform;

    private static StringBuilder stringbuilder=new StringBuilder();
    public static StringBuilder stringBuilder
    {
        get { return stringbuilder.Clear(); }
    }




    private void Awake()
    {
        Instance = this;
        Instantiate(WindowsUI, Vector3.zero, Quaternion.identity);
        if (Platform == TargetPlatform.Windows)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        EnsInstance.OnConnectionRejected += () =>
        {
            Notice.ShowMesg("连接失败");
        };
    }



    public static string GetIP()
    {
        /*
        string ipv4 = "";
        try
        {
            if (Instance.Platform == TargetPlatform.Windows)
            {
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    NetworkInterfaceType type1 = NetworkInterfaceType.Wireless80211;
                    NetworkInterfaceType type2 = NetworkInterfaceType.Ethernet;
                    if ((item.NetworkInterfaceType == type1 || item.NetworkInterfaceType == type2) && item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (var ip in item.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                ipv4 = ip.Address.ToString();
                        }
                    }
                }
            }
            else
            {
                ipv4 = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName())
                    .AddressList.First(f => f.ToString()[1] > '0').ToString();
            }
        }
        catch
        {
            ipv4 = "0.0.0.0";
        }
        return ipv4;*/
        List<string> candidateIPs = new List<string>(); // 候选IP列表（私有网段）
        List<string> hotspotPriorityIPs = new List<string>(); // 热点优先网段IP

        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            // 仅处理活跃的、非回环的网络接口
            if (networkInterface.OperationalStatus != OperationalStatus.Up ||
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                continue;

            // 遍历接口的所有单播IPv4地址
            foreach (UnicastIPAddressInformation unicastIP in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicastIP.Address.AddressFamily != AddressFamily.InterNetwork ||
                    IPAddress.IsLoopback(unicastIP.Address))
                    continue;

                string ip = unicastIP.Address.ToString();
                // 仅保留私有网段IP（局域网有效）
                if (IsPrivateIP(unicastIP.Address))
                {
                    candidateIPs.Add(ip);
                    // 标记热点常见网段IP（优先选择）
                    if (IsHotspotCommonSubnet(unicastIP.Address))
                    {
                        hotspotPriorityIPs.Add(ip);
                    }
                }
            }
        }

        // 优先返回热点常见网段IP（如果有）
        if (hotspotPriorityIPs.Count > 0)
        {
            return hotspotPriorityIPs[0]; // 取第一个热点网段IP
        }

        // 若无热点网段IP，返回其他私有网段IP（如果有）
        if (candidateIPs.Count > 0)
        {
            return candidateIPs[0]; // 取第一个有效私有IP
        }

        // 无可用局域网IP
        return "127.0.0.1";
    }
    private static bool IsPrivateIP(IPAddress ip)
    {
        byte[] bytes = ip.GetAddressBytes();
        return (bytes[0] == 10) ||                                   // 10.x.x.x
               (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) || // 172.16.x.x ~ 172.31.x.x
               (bytes[0] == 192 && bytes[1] == 168);                  // 192.168.x.x
    }
    private static bool IsHotspotCommonSubnet(IPAddress ip)
    {
        byte[] bytes = ip.GetAddressBytes();
        // Android热点常见网段：192.168.43.x（网关通常是192.168.43.1）
        // iOS热点常见网段：172.20.x.x（如172.20.10.x）
        return (bytes[0] == 192 && bytes[1] == 168 && bytes[2] == 43) || // Android热点
               (bytes[0] == 172 && bytes[1] == 20);                     // iOS热点常见网段
    }
}
