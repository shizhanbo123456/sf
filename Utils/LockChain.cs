using System.Collections.Generic;
using System.Diagnostics;

public class LockChain
{
    private bool locked=false;
    public bool Locked
    {
        get
        {
            return locked;
        }
        set
        {
            if (LockChains.Count > 0) throw new System.Exception("只能设置叶子节点的锁");
            if (locked != value)
            {
                locked = value;
                if(parentChain!=null)parentChain.UpdateLock();
            }
        }
    }
    public bool LockedInHierechy
    {
        get
        {
            if(parentChain!=null)return parentChain.LockedInHierechy;
            return locked;
        }
    }
    private bool inUse=true;

    private LockChain parentChain;
    private List<LockChain> LockChains = new List<LockChain>();
    /*
    private static int _index = 0;
    public int index = 0;
    public int rootindex()
    {
        if(parentChain==null)return index;
        return parentChain.rootindex();
    }*/
    public LockChain() 
    {
        //index=_index++; 
    }
    private LockChain(LockChain chain)
    {
        //index = _index++;
        parentChain = chain;
    }
    private void UpdateLock()
    {
        locked= false;
        for(int index = LockChains.Count - 1;index >= 0; index--)
        {
            LockChain i=LockChains[index];
            if (!i.inUse)
            {
                LockChains.RemoveAt(index);
                continue;
            }
            if (i.locked)
            {
                locked= true;
                break;
            }
        }
    }
    public void Discard()
    {
        inUse = false;
        if (parentChain != null) parentChain.UpdateLock();
    }
    public LockChain GetChain()
    {
        var c = new LockChain(this);
        LockChains.Add(c);
        return c;
    }
}
