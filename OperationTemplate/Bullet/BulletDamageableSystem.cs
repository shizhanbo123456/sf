using LevelCreator.TargetTemplate;
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
        b.ReleaseDamageableReference += OnBulletDestroyed;
        b.CanDamage += OnDamaged;
    }
    public static bool OnDamaged(Target target, Bullet bullet)
    {
        if (_bulletDamagedTargets.TryGetValue(bullet, out var damagedTargets))
        {
            if (damagedTargets.Contains(target))
            {
                Debug.Log(false);
                return false;
            }
            else
            {
                Debug.Log(true);
                damagedTargets.Add(target);
                return true;
            }
        }
        else
        {
            Debug.Log(true);
            damagedTargets = _hashSetPool.Get();
            _bulletDamagedTargets.Add(bullet, damagedTargets);
            _bulletDamagedTargets[bullet].Add(target);
            return true;
        }
    }
    private static void OnBulletDestroyed(Bullet bullet)
    {
        if (_bulletDamagedTargets.TryGetValue(bullet, out var damagedTargets))
        {
            _hashSetPool.Release(damagedTargets);
            _bulletDamagedTargets.Remove(bullet);
        }
        bullet.ReleaseDamageableReference -= OnBulletDestroyed;
        bullet.CanDamage -= OnDamaged;
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

    public static void Regist(Bullet b, float dt=0.1f)
    {
        _damageDt.Add(b,dt);
        b.CanDamage += CanDamageWithCooldown;
        b.ReleaseDamageableReference += OnBulletDestroyed;
    }

    private static bool CanDamageWithCooldown(Target target, Bullet bullet)
    {
        if (_bulletTargetDamageTimes.TryGetValue(bullet, out var targetTimeDict))
        {
            if (targetTimeDict.TryGetValue(target, out float canDamageTime))
            {
                if(Time.time > canDamageTime)
                {
                    targetTimeDict[target] = Time.time+_damageDt[bullet];
                    return true;
                }
                return false;
            }
            else
            {
                targetTimeDict.Add(target,Time.time+_damageDt[bullet]);
                return true;
            }
        }
        else
        {
            targetTimeDict = _targetTimeDictPool.Get();
            _bulletTargetDamageTimes.Add(bullet, targetTimeDict);
            _bulletTargetDamageTimes[bullet].Add(target,Time.time + _damageDt[bullet]);
            return true;
        }
    }
    private static void OnBulletDestroyed(Bullet bullet)
    {
        _damageDt.Remove(bullet);
        if (_bulletTargetDamageTimes.TryGetValue(bullet, out var targetTimeDict))
        {
            _targetTimeDictPool.Release(targetTimeDict);
            _bulletTargetDamageTimes.Remove(bullet);
        }
        bullet.CanDamage -= CanDamageWithCooldown;
        bullet.ReleaseDamageableReference -= OnBulletDestroyed;
    }
}