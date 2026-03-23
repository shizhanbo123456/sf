using System;
using System.Text;
using XLua;

[LuaCallCSharp]
[Serializable]
public struct TargetIdentify
{
    private static readonly StringBuilder stringBuilder = new StringBuilder();
    public int camp;
    public int owner;
    public int level;
    public string name;
    public float spawnX;
    public float spawnY;
    public float size;
    public string label;
    public TargetIdentify(int camp, int owner, int level, string name, float spawnX, float spawnY, float size, string label)
    {
        this.camp = camp;
        this.owner = owner;
        this.level = level;
        this.name = name;
        this.spawnX = spawnX;
        this.spawnY = spawnY;
        this.size = size;
        this.label = label;
    }
    public TargetIdentify(string info)
    {
        string[] s = info.Split('/');
        camp = int.Parse(s[0]);
        owner = int.Parse(s[1]);
        level = int.Parse(s[2]);
        name = s[3];
        spawnX = float.Parse(s[4]);
        spawnY = float.Parse(s[5]);
        size = float.Parse(s[6]);
        label = s[7];
    }
    public override string ToString()
    {
        var sb = stringBuilder;
        sb.Clear();
        sb.Append(camp).Append('/');
        sb.Append(owner).Append('/');
        sb.Append(level).Append('/');
        sb.Append(name).Append('/');
        sb.Append(spawnX.ToString("F1")).Append('/');
        sb.Append(spawnY.ToString("F1")).Append('/');
        sb.Append(size.ToString("F1")).Append('/');
        sb.Append(label);
        return sb.ToString();
    }
}