using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variety.Base
{
    public struct MotionBase
    {
        private static readonly Dictionary<int, float> SpeedMap = new()
        {
            {1,-24 },
            {2,-18 },
            {3,-12 },
            {4,-6 },
            {5,0 },
            {6,6 },
            {7,12 },
            {8,18 },
            {9,24 },
        };

        public bool IsNull => Time.time > workTime + startTime;
        public float workTime;
        public int stoicLevel;
        private int type;
        private int lx;
        private int ly;
        private int hx;
        private int hy;

        public float startTime;
        public float SpawnTime
        {
            get { return Time.time - startTime; }
        }

        public MotionBase(int id)
        {
            startTime = Time.time;
            int a = id % 10;
            id /= 10;
            int b = id % 10;
            id /= 10;
            workTime = a * 0.1f + b;
            a = id % 10;
            id /= 10;
            stoicLevel = a;
            a = id % 10;
            id /= 10;
            type = a;
            if (id > 0)
            {
                ly= id % 10;
                id /= 10;
                lx= id % 10;
                id /= 10;
                if(id > 0)
                {
                    hy= id % 10;
                    id /= 10;
                    hx= id % 10;
                }
                else
                {
                    hx = 0;
                    hy = 0;
                }
            }
            else
            {
                lx = 0; 
                ly = 0;
                hx = 0; 
                hy = 0;
            }
        }

        //传入当前速度，传出生效速度
        public Vector2 GetVelocity(Vector2 v)
        {
            switch (type)
            {
                case 1:return Vector2.zero;
                case 2:return new Vector2(SpeedMap[lx],SpeedMap[ly]);
                case 3:return v + new Vector2(SpeedMap[lx], SpeedMap[ly]) * Time.deltaTime;
                case 4:return Vector2.Lerp(new Vector2(SpeedMap[lx], SpeedMap[ly]), new Vector2(SpeedMap[hx], SpeedMap[hy]), SpawnTime / workTime);
                case 5:return v;
            }
            Debug.LogError("错误的type");
            return v;
        }

        public Vector2 Entry(Vector2 v)
        {
            switch (type)
            {
                case 1: return Vector2.zero;
                case 2: return new Vector2(SpeedMap[lx], SpeedMap[ly]);
                case 3: return v;
                case 4: return new Vector2(SpeedMap[lx], SpeedMap[ly]);
                case 5: return new Vector2(SpeedMap[lx], SpeedMap[ly]);
            }
            Debug.LogError("错误的type");
            return v;
        }
        public Vector2 Exit(Vector2 v)
        {
            switch (type)
            {
                case 1: return Vector2.zero;
                case 2: return new Vector2(SpeedMap[lx], SpeedMap[ly]);
                case 3: return v;
                case 4: return new Vector2(SpeedMap[hx], SpeedMap[hy]);
                case 5: return v;
            }
            Debug.LogError("错误的type");
            return v;
        }
    }
}
