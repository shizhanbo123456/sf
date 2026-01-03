using System;

public struct SendTo
{
    public static SendTo Everyone=>new SendTo(-1);
    public static SendTo ExcludeSender=>new SendTo(-2);
    public static SendTo RoomOwner=>new SendTo(-3);
    public static SendTo Server=>new SendTo(-4);
    public static SendTo To(short id) => new SendTo(id);
    public static SendTo To(byte b1,byte b2)
    {
        var b=BytesPool.GetBuffer(2);
        b[0] = b1;
        b[1] = b2;
        int index = 0;
        var s=ShortSerializer.Deserialize(b, ref index,2);
        BytesPool.ReturnBuffer(b);
        return new SendTo(s);
    }

    private short target;
    public readonly short Target => target;
    private SendTo(short target)
    {
        this.target=target;
    }
    public static bool operator ==(SendTo a,SendTo b)
    {
        return a.target == b.target;
    }
    public static bool operator !=(SendTo a, SendTo b)
    {
        return a.target != b.target;
    }
    public override bool Equals(object obj)
    {
        return obj is SendTo to &&
               target == to.target;
    }
    public override int GetHashCode()
    {
        return target;
    }
}