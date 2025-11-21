using UnityEngine;
using Variety.Base;

namespace Variety.Template
{
    public class BulletStatic : BulletControllerBase
    {
        private float radius;
        private Vector3 pos;
        public BulletStatic(Target t, float lifetime, float radius,Vector3 pos) : base(t, lifetime, MoveSpace.World)
        {
            this.radius = radius;
            this.pos = pos;
        }
        public override Vector3 GetPosition()
        {
            return pos;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletStaticScaleChange : BulletControllerBase
    {
        private float endRadius;
        private float startRadius;
        public BulletStaticScaleChange(Target t, float lifetime, float endRadius,float startRadius=0) : base(t, lifetime, MoveSpace.Local)
        {
            this.endRadius = endRadius;
            this.startRadius = startRadius;
        }
        public override Vector3 GetPosition()
        {
            return new Vector3();
        }
        public override float GetScale()
        {
            return Mathf.Lerp(startRadius, endRadius, SpawnTime / LifeTime);
        }
    }
    public class BulletStaticWorldScaleChange : BulletControllerBase
    {
        private float endRadius;
        private float startRadius;
        private Vector3 pos;
        public BulletStaticWorldScaleChange(Target t, float lifetime, float endRadius,Vector3 pos, float startRadius = 0) : base(t, lifetime, MoveSpace.World)
        {
            this.endRadius = endRadius;
            this.startRadius = startRadius;
            this.pos = pos;
        }
        public override Vector3 GetPosition()
        {
            return pos;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return Mathf.Lerp(startRadius, endRadius, SpawnTime / LifeTime);
        }
    }
    public class BulletFollow : BulletControllerBase
    {
        private float radius;
        private Transform follow;
        public BulletFollow(Target t, float lifetime, float radius) : base(t, lifetime, MoveSpace.World)
        {
            this.radius = radius;
            follow = t.transform;
        }
        public BulletFollow(Target t, float lifetime, float radius,Transform follow) : base(t, lifetime, MoveSpace.World)
        {
            this.radius = radius;
            this.follow = follow;
        }
        public override Vector3 GetPosition()
        {
            return follow.position;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletAngle : BulletControllerBase
    {
        private float speed;
        private float rad;
        private float radius;
        public BulletAngle(Target t,float lifetime,float speed,float angle,float radius) : base(t,lifetime, MoveSpace.Local)
        {
            this.speed = speed;
            rad = angle*Mathf.Deg2Rad;
            this.radius = radius;
        }
        public override Vector3 GetPosition()
        {
            var d = SpawnTime * speed;
            return new Vector3(d*Mathf.Cos(rad),d*Mathf.Sin(rad));
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletAngleNonFacing : BulletControllerBase
    {
        private float speed;
        private float rad;
        private float radius;
        private Vector3 posStart;
        public BulletAngleNonFacing(Target t, float lifetime, float speed, float angle, float radius) : base(t, lifetime, MoveSpace.World)
        {
            this.speed = speed;
            rad = angle * Mathf.Deg2Rad;
            this.radius = radius;
            posStart = t.transform.position;
        }
        public override Vector3 GetPosition()
        {
            var d = SpawnTime * speed;
            return posStart+new Vector3(d * Mathf.Cos(rad), d * Mathf.Sin(rad));
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletDir : BulletControllerBase
    {
        private float speed;
        private Vector2 dir;
        private float radius;
        public BulletDir(Target t,float lifetime, float speed, Vector2 dir, float radius) : base(t,lifetime, MoveSpace.Local)
        {
            this.speed = speed;
            this.dir = dir.normalized;
            this.radius = radius;
        }
        public override Vector3 GetPosition()
        {
            return SpawnTime * speed * dir;
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletAim : BulletControllerBase
    {
        private float speed;
        private Vector3 dir;
        private float radius;
        private Vector3 startPos;
        public BulletAim(Target t,float lifetime,Vector3 worldStartPos, float speed, Vector3 aimAt, float radius) : base(t,lifetime, MoveSpace.World)
        {
            this.speed = speed;
            dir = (aimAt - worldStartPos).normalized;
            this.radius = radius;
            startPos = worldStartPos;
        }
        public override Vector3 GetPosition()
        {
            return startPos+SpawnTime * speed * dir;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletOrbit : BulletControllerBase
    {
        private float angle;
        private float angleSpeed;
        private float radius;
        private float scale;
        private Vector3 offset;
        public BulletOrbit(Target shooter, float lifetime,float radius,float degreePerSec,float angleOffset,float scale,Vector3 offset=new Vector3()) : base(shooter, lifetime, MoveSpace.Player)
        {
            angle = angleOffset*Mathf.Deg2Rad;
            angleSpeed= degreePerSec*Mathf.Deg2Rad;
            this.radius= radius;
            this.scale = scale;
            this.offset = offset;
        }
        public override Vector3 GetPosition()
        {
            float a = angle + SpawnTime * angleSpeed;
            return new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * radius+offset;
        }
        public override float GetScale()
        {
            return scale;
        }
    }
    public class BulletOrbitWorld : BulletControllerBase
    {
        private float angle;
        private float angleSpeed;
        private float radius;
        private float scale;
        private Vector3 center;
        public BulletOrbitWorld(Target shooter, float lifetime, Vector3 center,float radius, float degreePerSec, float angleOffset, float scale) : base(shooter, lifetime, MoveSpace.World)
        {
            angle = angleOffset * Mathf.Deg2Rad;
            angleSpeed = degreePerSec * Mathf.Deg2Rad;
            this.radius = radius;
            this.scale = scale;
            this.center = center;
        }
        public override Vector3 GetPosition()
        {
            float a = angle + SpawnTime * angleSpeed;
            return center+new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * radius;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return scale;
        }
    }
    public class BulletAimAwait : BulletControllerBase
    {
        private float speed;
        private float radius;
        private Vector2 startWorldPos;
        private float awaitFor;
        private Vector2 dir;
        public BulletAimAwait(Target t, Vector2 startWorldPos,float awaitFor,float lifetime, float speed, Vector2 dir, float radius) : base(t, lifetime, MoveSpace.World)
        {
            this.speed = speed;
            this.radius = radius;
            this.startWorldPos = startWorldPos;
            this.awaitFor = awaitFor;
            this.dir = dir.normalized;
        }
        public override Vector3 GetPosition()
        {
            if (SpawnTime < awaitFor) return startWorldPos;
            else return startWorldPos + (SpawnTime - awaitFor) * speed * dir;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletFromTo : BulletControllerBase
    {
        private Vector3 start;
        private Vector3 end;
        private float radius;
        public BulletFromTo(Target t, float lifetime,Vector3 start,Vector3 end, float radius) : base(t, lifetime, MoveSpace.World)
        {
            this.start=start;
            this.end=end;
            this.radius = radius;
        }
        public override Vector3 GetPosition()
        {
            return Vector3.LerpUnclamped(start, end, SpawnTime / LifeTime);
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletProjectileAim : BulletControllerBase
    {
        private Vector3 startPos;
        private Vector3 startVelocity;
        //private Vector3 endPos;
        //private float hitTime;
        private Vector3 acceleration; // 抛物线加速度
        private float radius;

        public BulletProjectileAim(Target t, float lifetime, Vector3 startPos, Vector3 startVelocity,Vector3 endPos, float hitTime, float radius) : base(t, lifetime, MoveSpace.World)
        {
            this.startPos = startPos;
            this.startVelocity = startVelocity;
            //this.endPos = endPos;
            //this.hitTime = hitTime;
            this.radius = radius;

            Vector3 displacement = endPos - startPos;
            acceleration = 2 * (displacement - startVelocity * hitTime) / (hitTime * hitTime);
        }

        public override Vector3 GetPosition()
        {
            return startPos + startVelocity * SpawnTime + 0.5f * SpawnTime * SpawnTime * acceleration;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
    public class BulletProjectile : BulletControllerBase
    {
        private Vector3 startPos;
        private Vector3 startVelocity;
        private float radius;

        public BulletProjectile(Target t, float lifetime, Vector3 startVelocity, float radius) : base(t, lifetime, MoveSpace.World)
        {
            startPos = t.transform.position;
            this.startVelocity = startVelocity;
            this.radius = radius;
        }

        public override Vector3 GetPosition()
        {
            return startPos + startVelocity * SpawnTime + 5f * SpawnTime * SpawnTime * Vector3.down;
        }
        public override Vector3 GetPositionL()
        {
            return GetPosition();
        }
        public override float GetScale()
        {
            return radius;
        }
    }
}