using AttributeSystem.Attributes;

public class DedicateSyncAttributes
{
    public RegistableVariable<(int,int)> Shengming=RegistableVariable<(int,int)>.Get();

    public int Gongji;
    public int Mingzhong;
    public int Baoji;
    public int Jiashang;

    public DedicateSyncAttributes(int level, float healthScale = 1)
    {
        var att=TargetAttributes.GetGameTimeAttributes(level,healthScale);
        Shengming.Value = (att.Shengming.Value, att.Shengming.Value);
        Gongji = att.Gongji.Value;
        Mingzhong = att.Mingzhong.Value;
        Baoji = att.Baoji.Value;
        Jiashang = att.Jiashang.Value;
    }
}