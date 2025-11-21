using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class BulletDamageOnceSystem
{
    private static readonly Dictionary<Bullet, HashSet<Target>> _bulletDamagedTargets = new Dictionary<Bullet, HashSet<Target>>();

    private static readonly ObjectPool<HashSet<Target>> _hashSetPool = new ObjectPool<HashSet<Target>>(
        createFunc: () => new HashSet<Target>(),
        actionOnGet: (hashSet) => hashSet.Clear(),
        actionOnRelease: (hashSet) => hashSet.Clear()
    );
    public static void Regist(Bullet b)
    {
        b.ReleaseDamageableReference = OnBulletDestroyed;
        b.CanDamage = OnDamaged;
    }
    public static bool OnDamaged(Target target, Bullet bullet)
    {
        if (!_bulletDamagedTargets.TryGetValue(bullet, out var damagedTargets))
        {
            damagedTargets = _hashSetPool.Get();
            _bulletDamagedTargets.Add(bullet, damagedTargets);
            return true;
        }
        if (damagedTargets.Contains(target)) return false;
        return true;
    }
    private static void OnBulletDestroyed(Bullet bullet)
    {
        if (_bulletDamagedTargets.TryGetValue(bullet, out var damagedTargets))
        {
            _hashSetPool.Release(damagedTargets);
            _bulletDamagedTargets.Remove(bullet);
        }
    }
}
public static class BulletDamageTimeSystem
{
    private static readonly Dictionary<Bullet,float>_damageDt= new Dictionary<Bullet,float>();

    private static readonly Dictionary<Bullet, Dictionary<Target, float>> _bulletTargetDamageTimes = new Dictionary<Bullet, Dictionary<Target, float>>();

    private static readonly ObjectPool<Dictionary<Target, float>> _targetTimeDictPool = new ObjectPool<Dictionary<Target, float>>(
        createFunc: () => new Dictionary<Target, float>(),
        actionOnGet: (dict) => dict.Clear(),
        actionOnRelease: (dict) => dict.Clear()
    );

    /// <summary>
    /// 为子弹注册到时间伤害系统，并设置最小伤害间隔。
    /// </summary>
    /// <param name="b">要注册的子弹</param>
    /// <param name="dt">最小伤害间隔（秒）</param>
    public static void Regist(Bullet b, float dt)
    {
        _damageDt[b]=dt;
        b.CanDamage = CanDamageWithCooldown;
        b.ReleaseDamageableReference += OnBulletDestroyed;
    }

    /// <summary>
    /// 核心伤害检查逻辑，包含冷却时间判断。
    /// </summary>
    private static bool CanDamageWithCooldown(Target target, Bullet bullet)
    {
        if (_bulletTargetDamageTimes.TryGetValue(bullet, out var targetTimeDict))
        {
            if (targetTimeDict.TryGetValue(target, out float canDamageTime))
            {
                return Time.time > canDamageTime;
            }
            else
            {
                targetTimeDict.Add(target,_damageDt[bullet]);
                return true;
            }
        }
        else
        {
            targetTimeDict = _targetTimeDictPool.Get();
            _bulletTargetDamageTimes.Add(bullet, targetTimeDict);
            _bulletTargetDamageTimes[bullet][target] = Time.time + _damageDt[bullet];
            return true;
        }
    }
    private static void OnBulletDestroyed(Bullet bullet)
    {
        if (_bulletTargetDamageTimes.TryGetValue(bullet, out var targetTimeDict))
        {
            _targetTimeDictPool.Release(targetTimeDict);
            _bulletTargetDamageTimes.Remove(bullet);
        }
    }
}