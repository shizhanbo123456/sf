using System;
using System.Text;
using XLua;

[Serializable]
[LuaCallCSharp]
public struct TargetIdentify
{
    private static readonly StringBuilder stringBuilder = new StringBuilder();
    public int camp;
    public int owner;
    public int level;
    public string name;
    public float size;
    public float spawnX;
    public float spawnY;
    public string label;
    public TargetIdentify(int camp, int owner, int level, string name, float size,float spawnX,float spawnY, string label)
    {
        this.camp = camp;
        this.owner = owner;
        this.level = level;
        this.name = name;
        this.size = size;
        this.spawnX= spawnX;
        this.spawnY= spawnY;
        this.label = label;
    }
    public TargetIdentify(string info)
    {
        string[] s = info.Split('/');
        camp = int.Parse(s[0]);
        owner = int.Parse(s[1]);
        level = int.Parse(s[2]);
        name = s[3];
        size = float.Parse(s[4]);
        spawnX = float.Parse(s[5]);
        spawnY = float.Parse(s[6]);
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
        sb.Append(size.ToString("F1")).Append('/');
        sb.Append(spawnX.ToString("F1")).Append('/');
        sb.Append(spawnY.ToString("F1")).Append('/');
        sb.Append(label);
        return sb.ToString();
    }
}