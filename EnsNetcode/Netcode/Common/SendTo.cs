using System;

public struct SendTo
{
    public static SendTo Everyone=>new SendTo(-1);
    public static SendTo ExcludeSender=>new SendTo(-2);
    public static SendTo RoomOwner=>new SendTo(-3);
    public static SendTo To(short id) 
    {
        if (id < 0) throw new Exception("id不能小于0");
        return new SendTo(id);
    }
    public static SendTo To(byte b1,byte b2)
    {
        var b=BytesPool.GetBuffer(2);
        b[0] = b1;
        b[1] = b2;
        int index = 0;
        var s=ShortSerializer.Deserialize(b, ref index);
        BytesPool.ReturnBuffer(b);
        return new SendTo(s);
    }

    private short target;
    public readonly short Target => target;
    private SendTo(short target)
    {
        this.target=target;
    }
}