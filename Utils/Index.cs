public class Index
{
    private static int id = 0;
    private int ID;
    public Index()
    {
        ID = id;
        id++;
    }
    public static bool operator ==(Index a, Index b)
    {
        return a.ID == b.ID;
    }
    public static bool operator !=(Index a, Index b)
    {
        return a.ID != b.ID;
    }
    public override bool Equals(object obj)
    {
        if (obj is not Index) return false;
        return (obj as Index).ID == ID;
    }
    public override int GetHashCode()
    {
        return ID;
    }
}