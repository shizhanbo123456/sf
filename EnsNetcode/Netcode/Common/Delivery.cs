using System;

public class DeliverySource
{
    //Unreliable 0
    //Reliable 1-200
    //Orderwise 201-255

    private int reliableSource = 1;
    private byte Reliable
    {
        get
        {
            reliableSource++;
            if (reliableSource > 200) reliableSource = 1;
            return (byte)reliableSource;
        }
    }
    private int orderWiseSource = 201;
   private byte Orderwise
    {
        get
        {
            orderWiseSource++;
            if (orderWiseSource > 255) orderWiseSource = 201;
            return (byte)orderWiseSource;
        }
    }
    public byte GetIndex(Delivery delivery)
    {
        return delivery switch
        {
            Delivery.Unreliable => 0,
            Delivery.Reliable => Reliable,
            Delivery.OrderWise => Orderwise,
            _ => throw new Exception("Delivery随机数产生器检测到未知类型")
        };
    }
    public static bool DeliveryEqual(byte b,Delivery d)
    {
        switch (d)
        {
            case Delivery.Unreliable:return b == 0;
            case Delivery.Reliable:return b <=200&&b>=1;
            case Delivery.OrderWise:return b >= 201 && b <= 255;
        }
        return false;
    }
}
public enum Delivery
{
    Unreliable,
    Reliable,
    OrderWise
}