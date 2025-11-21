using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variety.Base
{
    public class BulletControllerBase
    {
        public Target shooter;
        public float LifeTime;
        public float SpawnTime;

        public enum MoveSpace
        {
            Local,Player,World
        }
        public MoveSpace BulletMoveSpace;

        public BulletControllerBase(Target shooter,float lifetime,MoveSpace moveSpace=MoveSpace.Local)
        {
            this.shooter = shooter;
            LifeTime= lifetime;
            BulletMoveSpace= moveSpace;
        }
        public virtual Vector3 GetPosition()
        {
            return new Vector3(0, 0, 0);
        }
        public virtual Vector3 GetPositionL()
        {
            Vector3 v = GetPosition();
            return new Vector3(-v.x, v.y, 0);
        }
        public virtual float GetScale()
        {
            return 1;
        }
    }
}
