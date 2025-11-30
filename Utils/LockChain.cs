using System.Collections.Generic;
using UnityEngine.Pool;

public class LockChain
{
    private bool locked=false;
    public bool InUse { get; private set; } = true;
    public bool IsLeaveLock { get; private set; } = false;
    public bool Locked
    {
        get
        {
            return locked;
        }
        set
        {
            if (!InUse) throw new System.Exception("锁已被弃用，引用已失效");
            if (!IsLeaveLock) throw new System.Exception("只能设置叶子节点的锁");
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
            if (!InUse) throw new System.Exception("锁已被弃用，引用已失效");
            if (parentChain!=null)return parentChain.LockedInHierechy;
            return locked;
        }
    }

    private LockChain parentChain;
    private List<LockChain> LockChains = new List<LockChain>();

    private LockChain() { }
    private void UpdateLock()
    {
        locked= false;
        for(int index = LockChains.Count - 1;index >= 0; index--)
        {
            if (!LockChains[index].InUse)
            {
                LockChains.RemoveAt(index);
                continue;
            }
            if (LockChains[index].locked)
            {
                locked= true;
                break;
            }
        }
        if (parentChain != null) parentChain.UpdateLock();
    }
    public void Discard()
    {
        if (!InUse) throw new System.Exception("锁已被弃用，引用已失效");
        InUse = false;
        if (parentChain != null)
        {
            parentChain.UpdateLock();
            parentChain = null;
        }
        foreach(var i in LockChains)
        {
            i.parentChain = null;
            i.Discard();
        }
        pool.Release(this);
    }
    public LockChain GetChain(bool isLeaveLock=true)
    {
        if (!InUse) throw new System.Exception("锁已被弃用，引用已失效");
        if (IsLeaveLock&&isLeaveLock) throw new System.Exception("不能从叶子锁引出子锁");
        var c = pool.Get();
        c.parentChain = this;
        LockChains.Add(c);
        c.IsLeaveLock = isLeaveLock;
        return c;
    }

    private static ObjectPool<LockChain> pool = new(
        () => new LockChain(),
        actionOnGet: c => { c.locked = false; c.parentChain=null; c.IsLeaveLock = false;c.InUse = true;c.LockChains.Clear(); });
    public static LockChain CreateLock()=> pool.Get();
}
