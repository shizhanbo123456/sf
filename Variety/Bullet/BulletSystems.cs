using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public static class BulletSystemCommon
{
    public static Target CurrentShooter;
    public static Vector3 AngleToVector(float angle)
    {
        angle*=Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    public static Vector3 RadToVector(float rad)
    {
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
public static class BulletStaticSystem
{
    private static readonly Dictionary<Bullet,float>Collection=new Dictionary<Bullet,float>();
    private static readonly List<Bullet>ToRemove = new List<Bullet>();
    public static void RegistObject(Bullet obj, float radius,float lifeTime, Vector3 pos)
    {
        obj.transform.position = pos;
        obj.transform.localScale *=radius;
        Collection.Add(obj, Time.time + lifeTime);
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach(var i in Collection) if (Time.time > i.Value) ToRemove.Add(i.Key);
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletStaticScaleChangeSystem
{
    private struct ScaleChangeInfo
    {
        public ScaleChangeInfo(float scaleFactor,float start,float end,float lifetime)
        {
            this.scaleFactor = scaleFactor;
            startScale = start;
            endScale = end;
            spawnTime= Time.time;
            lifeTime = lifetime;
        }
        public float scaleFactor;
        public float startScale;
        public float endScale;
        public float spawnTime;
        public float lifeTime;
    }
    private static readonly Dictionary<Bullet, ScaleChangeInfo> Collection = new Dictionary<Bullet, ScaleChangeInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();
    public static void RegistObject(Bullet obj, float startradius,float endradius, float lifeTime)
    {
        obj.transform.position = BulletSystemCommon.CurrentShooter.transform.position;
        Collection.Add(obj, new ScaleChangeInfo(obj.transform.localScale.x,startradius,endradius,lifeTime));
        obj.transform.localScale *= startradius;
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    public static void RegistObject(Bullet obj, float startradius, float endradius, float lifeTime,Vector3 pos)
    {
        obj.transform.position = pos;
        Collection.Add(obj, new ScaleChangeInfo(obj.transform.localScale.x, startradius, endradius, lifeTime));
        obj.transform.localScale *= startradius;
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach (var i in Collection)
        {
            if (Time.time > i.Value.lifeTime+i.Value.spawnTime) ToRemove.Add(i.Key);
            else
            {
                i.Key.transform.localScale= Mathf.Lerp(i.Value.startScale,i.Value.endScale,(Time.time-i.Value.spawnTime)/i.Value.lifeTime)*Vector3.one;
                i.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletFollowSystem
{
    private struct FollowInfo
    {
        public FollowInfo(float destroyTime,int followcamp,int followid)
        {
            this.destroyTime = destroyTime;
            followCamp=followcamp;
            followId=followid;
        }
        public float destroyTime;
        public int followCamp;
        public int followId;
    }
    private static readonly Dictionary<Bullet, FollowInfo> Collection = new Dictionary<Bullet, FollowInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();
    private static Target t = null;
    public static void RegistObject(Bullet obj, float radius, float lifeTime,Target followTarget)
    {
        obj.transform.position = BulletSystemCommon.CurrentShooter.transform.position;
        obj.transform.localScale *= radius;
        Collection.Add(obj, new FollowInfo(Time.time+lifeTime,followTarget.Camp,followTarget.ObjectId));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach (var i in Collection)
        {
            if (Time.time > i.Value.destroyTime) ToRemove.Add(i.Key);
            else
            {
                t=Tool.SceneController.GetTarget(i.Value.followCamp, i.Value.followId);
                if (t == null)
                {
                    ToRemove.Add(i.Key);
                    continue;
                }
                i.Key.transform.position = t.transform.position;
                i.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletAngleSystem
{
    private struct AngleInfo
    {
        public AngleInfo(float spawntime,float lifetime,Vector3 startpos,float speed,float angle,bool faceright)
        {
            spawnTime = spawntime;
            lifeTime= lifetime;
            startPos = startpos;
            this.speed = speed;
            this.angle = angle;
            faceRight = faceright;
        }
        public float spawnTime;
        public float lifeTime;
        public Vector3 startPos;
        public float speed;
        public float angle;
        public bool faceRight;
    }
    private static readonly Dictionary<Bullet, AngleInfo> Collection = new Dictionary<Bullet, AngleInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();
    public static void RegistObject(Bullet obj, float radius, float lifeTime, float speed, float angle)
    {
        obj.transform.position = BulletSystemCommon.CurrentShooter.transform.position;
        obj.transform.localScale *= radius;
        Collection.Add(obj, new AngleInfo(Time.time, lifeTime,BulletSystemCommon.CurrentShooter.transform.position ,
            speed, angle, BulletSystemCommon.CurrentShooter.FaceRight));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach (var i in Collection)
        {
            if (Time.time > i.Value.spawnTime+i.Value.lifeTime) ToRemove.Add(i.Key);
            else
            {
                i.Key.transform.position = (Time.time - i.Value.spawnTime) * i.Value.speed * 
                    BulletSystemCommon.AngleToVector(i.Value.faceRight ? i.Value.angle : 180 - i.Value.angle) + i.Value.startPos;
                i.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletAngleNonFacingSystem
{
    private struct AngleNonFacingInfo
    {
        public float spawnTime;
        public float destroyTime;
        public float speed;
        public float rad;
        public float radius;
        public Vector3 posStart;
        public int ownerCamp;
        public int ownerId;

        public AngleNonFacingInfo(float destroyTime, float speed, float rad, float radius)
        {
            spawnTime = Time.time;
            this.destroyTime = destroyTime;
            this.speed = speed;
            this.rad = rad;
            this.radius = radius;
            posStart = BulletSystemCommon.CurrentShooter.transform.position;
            ownerCamp = BulletSystemCommon.CurrentShooter.Camp;
            ownerId = BulletSystemCommon.CurrentShooter.ObjectId;
        }
    }

    private static readonly Dictionary<Bullet, AngleNonFacingInfo> Collection = new Dictionary<Bullet, AngleNonFacingInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();
    private static Target t = null;

    public static void RegistObject(Bullet obj, float radius, float lifetime, float speed, float angle)
    {
        obj.transform.localScale *= radius;
        obj.transform.position = BulletSystemCommon.CurrentShooter.transform.position;
        Collection.Add(obj, new AngleNonFacingInfo(Time.time + lifetime,speed,angle * Mathf.Deg2Rad,radius));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }

    public static void Update()
    {
        foreach (var pair in Collection)
        {
            if (Time.time > pair.Value.destroyTime) ToRemove.Add(pair.Key);
            else
            {
                t = Tool.SceneController.GetTarget(pair.Value.ownerCamp, pair.Value.ownerId);
                if (t == null)
                {
                    ToRemove.Add(pair.Key);
                    continue;
                }
                pair.Key.transform.position = pair.Value.posStart + (Time.time - pair.Value.spawnTime) *
                    pair.Value.speed * BulletSystemCommon.RadToVector(pair.Value.rad);
                pair.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}

public static class BulletDirSystem
{
    private struct DirInfo
    {
        public float spawnTime;
        public float destroyTime;
        public float speed;
        public Vector2 dir;
        public float radius;
        public Vector3 startPosition;
        public bool faceRight;

        public DirInfo(float destroyTime, float speed, Vector2 dir, float radius)
        {
            spawnTime = Time.time;
            this.destroyTime = destroyTime;
            this.speed = speed;
            this.dir = dir;
            this.radius = radius;
            startPosition = BulletSystemCommon.CurrentShooter.transform.position;
            faceRight = BulletSystemCommon.CurrentShooter.FaceRight;
        }
    }

    private static readonly Dictionary<Bullet, DirInfo> Collection = new Dictionary<Bullet, DirInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();

    public static void RegistObject(Bullet obj, float radius, float lifetime, float speed, Vector2 dir)
    {
        obj.transform.localScale *= radius;
        obj.transform.position= BulletSystemCommon.CurrentShooter.transform.position;
        Collection.Add(obj, new DirInfo(Time.time + lifetime,speed,dir.normalized,radius));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }

    public static void Update()
    {
        foreach (var pair in Collection)
        {
            if (Time.time > pair.Value.destroyTime) ToRemove.Add(pair.Key);
            else
            {
                Vector3 localPos = (Time.time - pair.Value.spawnTime) * pair.Value.speed * pair.Value.dir;
                Vector3 worldPos = pair.Value.startPosition + (pair.Value.faceRight ? localPos :new Vector3(-localPos.x,localPos.y));
                pair.Key.transform.position = worldPos;
                pair.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}

public static class BulletAimSystem
{
    private struct AimInfo
    {
        public float spawnTime;
        public float destroyTime;
        public float speed;
        public Vector3 dir;
        public Vector3 startPos;

        public AimInfo(float destroyTime, float speed, Vector3 dir,Vector3 startPos)
        {
            spawnTime = Time.time;
            this.destroyTime = destroyTime;
            this.speed = speed;
            this.dir = dir;
            this.startPos = startPos;
        }
    }

    private static readonly Dictionary<Bullet, AimInfo> Collection = new Dictionary<Bullet, AimInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();

    public static void RegistObject(Bullet obj, float radius, float lifetime, Vector3 worldStartPos, float speed, Vector3 aimAt)
    {
        obj.transform.localScale *= radius;
        obj.transform.position=worldStartPos;
        Collection.Add(obj, new AimInfo(
            Time.time + lifetime,
            speed,
            (aimAt - worldStartPos).normalized,
            worldStartPos
        ));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach (var pair in Collection)
        {
            if (Time.time > pair.Value.destroyTime) ToRemove.Add(pair.Key);
            else
            {
                pair.Key.transform.position = pair.Value.startPos + (Time.time - pair.Value.spawnTime) * pair.Value.speed * pair.Value.dir;
                pair.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletOrbitSystem
{
    private struct OrbitInfo
    {
        public float destroyTime;
        public float lifetime;
        public float radOffset;
        public float radSpeed;
        public float radius;
        public Vector3 offset;
        public int ownerCamp;
        public int ownerId;
        public bool faceRight;

        public OrbitInfo(float destroyTime, float radoffset, float radspeed, float radius,Vector3 offset, float lifetime)
        {
            this.destroyTime = destroyTime;
            this.lifetime = lifetime;
            radOffset = radoffset;
            radSpeed = radspeed;
            this.radius = radius;
            this.offset = offset;
            ownerCamp = BulletSystemCommon.CurrentShooter.Camp;
            ownerId = BulletSystemCommon.CurrentShooter.ObjectId;
            faceRight = BulletSystemCommon.CurrentShooter.FaceRight;
        }
    }

    private static readonly Dictionary<Bullet, OrbitInfo> Collection = new Dictionary<Bullet, OrbitInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();
    private static Target t;
    public static void RegistObject(Bullet obj, float scale, float lifetime, float radius,
        float degreePerSec, float angleOffset)
    {
        RegistObject(obj, scale, lifetime, radius, degreePerSec, angleOffset, Vector3.zero);
    }
    public static void RegistObject(Bullet obj, float scale, float lifetime, float radius, 
        float degreePerSec,float angleOffset,Vector3 offset)
    {
        obj.transform.localScale *= scale; 
        Vector3 localPos = BulletSystemCommon.AngleToVector(angleOffset)*radius + offset;
        Vector3 worldPos = BulletSystemCommon.CurrentShooter.transform.position +
            (BulletSystemCommon.CurrentShooter.FaceRight? localPos : new Vector3(-localPos.x, localPos.y));
        obj.transform.position = worldPos;
        Collection.Add(obj, new OrbitInfo(Time.time + lifetime, angleOffset * Mathf.Deg2Rad,
            degreePerSec * Mathf.Deg2Rad, radius, offset,lifetime));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }

    public static void Update()
    {
        foreach (var i in Collection)
        {
            if (Time.time > i.Value.destroyTime) ToRemove.Add(i.Key);
            else
            {
                t = Tool.SceneController.GetTarget(i.Value.ownerCamp, i.Value.ownerId);
                if (t == null)
                {
                    ToRemove.Add(i.Key);
                    continue;
                }
                float spawnTime = Time.time - (i.Value.destroyTime - i.Value.lifetime);
                Vector3 localPos = BulletSystemCommon.RadToVector(i.Value.radOffset + spawnTime * i.Value.radSpeed)
                    * i.Value.radius + i.Value.offset;
                Vector3 worldPos = t.transform.position +
                                  (i.Value.faceRight ? localPos : new Vector3(-localPos.x,localPos.y));
                i.Key.transform.position = worldPos;
                i.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletOrbitWorldSystem
{
    private struct OrbitInfo
    {
        public float destroyTime;
        public float spawntime;
        public float radOffset;
        public float radSpeed;
        public float radius;
        public Vector3 pos;

        public OrbitInfo(float destroyTime, float radoffset, float radspeed, float radius,Vector3 pos)
        {
            this.destroyTime = destroyTime;
            spawntime = Time.time;
            radOffset = radoffset;
            radSpeed = radspeed;
            this.radius = radius;
            this.pos = pos;
        }
    }

    private static readonly Dictionary<Bullet, OrbitInfo> Collection = new Dictionary<Bullet, OrbitInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();
    public static void RegistObject(Bullet obj, float scale, float lifetime, float radius,
        float degreePerSec,float angleOffset, Vector3 pos)
    {
        obj.transform.localScale *= scale;
        Vector3 localPos = BulletSystemCommon.AngleToVector(angleOffset)* radius;
        obj.transform.position = pos + localPos;
        Collection.Add(obj, new OrbitInfo(Time.time + lifetime, angleOffset * Mathf.Deg2Rad,
            degreePerSec * Mathf.Deg2Rad, radius, pos));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }

    public static void Update()
    {
        foreach (var i in Collection)
        {
            if (Time.time > i.Value.destroyTime) ToRemove.Add(i.Key);
            else
            {
                Vector3 localPos = BulletSystemCommon.RadToVector(i.Value.radOffset + i.Value.spawntime * i.Value.radSpeed)* i.Value.radius;
                i.Key.transform.position = i.Value.pos + localPos;
                i.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletDirAwaitSystem
{
    private struct AimAwaitInfo
    {
        public float startMoveTime;
        public float destroyTime;
        public float speed;
        public Vector3 dir;
        public Vector3 startPos;

        public AimAwaitInfo(float destroyTime,float startmovetime, float speed, Vector3 dir, Vector3 startPos)
        {
            startMoveTime = startmovetime;
            this.destroyTime = destroyTime;
            this.speed = speed;
            this.dir = dir;
            this.startPos = startPos;
        }
    }

    private static readonly Dictionary<Bullet, AimAwaitInfo> Collection = new Dictionary<Bullet, AimAwaitInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();

    public static void RegistObject(Bullet obj, float radius, float lifetime, float waitfor,Vector3 worldStartPos, float speed, Vector3 dir)
    {
        obj.transform.localScale *= radius;
        obj.transform.position = worldStartPos;
        Collection.Add(obj, new AimAwaitInfo(
            Time.time + lifetime,
            Time.time + waitfor,
            speed,
            dir.normalized,
            worldStartPos
        ));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach (var pair in Collection)
        {
            if (Time.time > pair.Value.destroyTime) ToRemove.Add(pair.Key);
            else
            {
                if (Time.time > pair.Value.startMoveTime)
                    pair.Key.transform.position = pair.Value.startPos + (Time.time - pair.Value.startMoveTime) * pair.Value.speed * pair.Value.dir;
                pair.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletFromToSystem
{
    private struct FromToInfo
    {
        public float spawnTime;
        public float destroyTime;
        public Vector3 startPos;
        public Vector3 endPos;

        public FromToInfo(float spawntime, float destroytime,Vector3 startpos,Vector3 endpos)
        {
            spawnTime = spawntime;
            destroyTime = destroytime;
            startPos = startpos;
            endPos = endpos;
        }
    }

    private static readonly Dictionary<Bullet, FromToInfo> Collection = new Dictionary<Bullet, FromToInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();

    public static void RegistObject(Bullet obj, float radius, float lifetime, Vector3 worldStartPos, Vector3 worldEndPos)
    {
        obj.transform.localScale *= radius;
        obj.transform.position = worldStartPos;
        Collection.Add(obj, new FromToInfo(
            Time.time,
            Time.time+lifetime,
            worldStartPos,
            worldEndPos
        ));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach (var pair in Collection)
        {
            if (Time.time > pair.Value.destroyTime) ToRemove.Add(pair.Key);
            else
            {
                float lerp = (Time.time - pair.Value.spawnTime) / (pair.Value.destroyTime - pair.Value.spawnTime);
                pair.Key.transform.position = Vector3.LerpUnclamped(pair.Value.startPos, pair.Value.endPos, lerp);
                pair.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletProectileAimSystem
{
    private struct ProjectileAimInfo
    {
        public float spawnTime;
        public float destroyTime;
        public Vector3 startPos;
        public Vector3 startVelocity;
        public Vector3 acceleration;

        public ProjectileAimInfo(float spawntime,float destroytime,Vector3 startpos,Vector3 startvelocity,Vector3 acceleration)
        {
            spawnTime = spawntime;
            destroyTime = destroytime;
            startPos=startpos;
            startVelocity=startvelocity;
            this.acceleration=acceleration;
        }
    }
    
    private static readonly Dictionary<Bullet, ProjectileAimInfo> Collection = new Dictionary<Bullet, ProjectileAimInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();

    public static void RegistObject(Bullet obj, float radius, float lifetime, Vector3 startPos, Vector3 startVelocity, Vector3 endPos, float hitTime)
    {
        obj.transform.localScale *= radius;
        obj.transform.position = startPos;

        Vector3 displacement = endPos - startPos;
        Vector3 acceleration = 2 * (displacement - startVelocity * hitTime) / (hitTime * hitTime);

        Collection.Add(obj, new ProjectileAimInfo(
            Time.time,
            Time.time + lifetime,
            startPos,
            startVelocity,
            acceleration
        ));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }
    public static void Update()
    {
        foreach (var pair in Collection)
        {
            if (Time.time > pair.Value.destroyTime) ToRemove.Add(pair.Key);
            else
            {
                float t =Time.time - pair.Value.spawnTime;
                pair.Key.transform.position = pair.Value.startPos+pair.Value.startVelocity*t+pair.Value.acceleration*(t*t);
                pair.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}
public static class BulletProectileSystem
{
    private struct ProjectileInfo
    {
        public float spawnTime;
        public float destroyTime;
        public Vector3 startPos;
        public Vector3 startVelocity;

        public ProjectileInfo(float spawntime, float destroytime, Vector3 startpos, Vector3 startvelocity)
        {
            spawnTime = spawntime;
            destroyTime = destroytime;
            startPos = startpos;
            startVelocity = startvelocity;
        }
    }

    private static readonly Dictionary<Bullet, ProjectileInfo> Collection = new Dictionary<Bullet, ProjectileInfo>();
    private static readonly List<Bullet> ToRemove = new List<Bullet>();

    public static void RegistObject(Bullet obj, float radius, float lifetime, Vector3 startPos, Vector3 startVelocity)
    {
        obj.transform.localScale *= radius;
        obj.transform.position = startPos;

        Collection.Add(obj, new ProjectileInfo(
            Time.time,
            Time.time + lifetime,
            startPos,
            startVelocity
        ));
        obj.ReleaseBulletSystemReference = ReleaseReference;
    }
    private static void ReleaseReference(Bullet b)
    {
        Collection.Remove(b);
    }

    public static void Update()
    {
        foreach (var pair in Collection)
        {
            if (Time.time > pair.Value.destroyTime) ToRemove.Add(pair.Key);
            else
            {
                float t = Time.time - pair.Value.spawnTime;
                pair.Key.transform.position = pair.Value.startPos + pair.Value.startVelocity * t + Vector3.down*(9.81f * t * t);
                pair.Key.UpdateDetectors();
            }
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove)
            {
                i.DestroyBullet();
            }
            ToRemove.Clear();
        }
    }
}