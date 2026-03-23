using System;
using Utils;

public class DeliverySource
{
    //Unreliable 0
    //Reliable 1-100 resp=101-200 +100
    //Orderwise 201-227 resp=228-254 +27
    public static readonly byte Unreliable = 0;

    private int reliableSource = 1;
    private int orderWiseSource = 201;
    private DeliverySource() { }
    private static readonly ObjectPool<DeliverySource> pool=new ObjectPool<DeliverySource>(
        () => new DeliverySource(), 
        s => { s.reliableSource = 1;s.orderWiseSource = 201; }
        );
    public static DeliverySource Get()
    {
        var p=pool.Get();
        p.reliableSource = 1;
        p.orderWiseSource = 201;
        return p;
    }
    public static void Return(DeliverySource d)
    {
        pool.Return(d);
    }
    private byte Reliable
    {
        get
        {
            reliableSource++;
            if (reliableSource > 100) reliableSource = 1;
            return (byte)reliableSource;
        }
    }
    private byte Orderwise
    {
        get
        {
            orderWiseSource++;
            if (orderWiseSource > 227) orderWiseSource = 201;
            return (byte)orderWiseSource;
        }
    }
    public byte DeliveryToByte(Delivery delivery)
    {
        return delivery switch
        {
            Delivery.Unreliable => Unreliable,
            Delivery.Reliable => Reliable,
            Delivery.OrderWise => Orderwise,
            _ => throw new Exception("Delivery随机数产生器检测到未知类型")
        };
    }
    public byte Top(Delivery delivery)
    {
        if (delivery == Delivery.Unreliable) return Unreliable;
        if(delivery == Delivery.Reliable)
        {
            int a=reliableSource+1;
            if (a > 100) a = 1;
            return (byte)a;
        }
        else
        {
            int b=orderWiseSource+1;
            if (b > 227) b = 201;
            return (byte)b;
        }
    }
    public static Delivery ByteToDelivery(byte b)
    {
        if (b == 0) return Delivery.Unreliable;
        else if (b >= 1 && b <= 200) return Delivery.Reliable;
        else return Delivery.OrderWise;
    }
    public static bool IsResponse(byte b)
    {
        return (b >= 101 && b <= 200) || (b >= 228 && b <= 254);
    }
    public static byte MessageToResponse(byte b)
    {
        if (b >= 1 && b <= 100) return (byte)(b + 100);
        else if (b >= 201 && b <= 227) return (byte)(b + 27);
        else throw new Exception("Delivery随机数产生器检测到未知类型");
    }
    public static byte ResponseToMessage(byte b)
    {
        if (b >= 101 && b <= 200) return (byte)(b - 100);
        else if (b >= 228 && b <= 254) return (byte)(b - 27);
        else throw new Exception("Delivery随机数产生器检测到未知类型");
    }
}
public enum Delivery
{
    Unreliable,
    Reliable,
    OrderWise
}