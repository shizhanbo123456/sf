using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variety.Base
{
    public struct MotionBase
    {
        private static readonly Dictionary<int, float> SpeedMap = new()
        {
            {0,-24 },
            {1,-18 },
            {2,-12 },
            {3,-6 },
            {4,0 },
            {5,6 },
            {6,12 },
            {7,18 },
            {8,24 },
        }; 
        private static readonly Dictionary<int, float> ShortSpeedMap = new()
        {
            {0,-12 },
            {1,-6 },
            {2,0 },
            {3,6 },
            {4,12 },
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

        public MotionBase(ushort id)
        {
            startTime = Time.time;
            MotionIdCalculator.mod.Decode(id, out var timeFactor, out var stoicLevel, out int code);
            workTime = timeFactor * 0.2f + 0.2f;
            this.stoicLevel = stoicLevel;
            if (code == 0)
            {
                type = 1;
                lx = 0;
                ly = 0;
                hx = 0;
                hy = 0;
            }
            else if (code <= 81)
            {
                type = 2;
                code -= 1;
                MotionIdCalculator.speedV2Mod.Decode(code, out lx, out ly);
                hx = 0;
                hy = 0;
            }
            else if (code <= 162)
            {
                type = 3;
                code -= 82;
                MotionIdCalculator.speedV2Mod.Decode(code, out lx, out ly);
                hx = 0;
                hy = 0;
            }
            else if (code <= 787)
            {
                type = 4;
                code -= 163;
                MotionIdCalculator.speedV4Mod.Decode(code, out lx, out ly,out hx,out hy);
            }
            else if (code <= 868)
            {
                type = 5;
                code -= 788;
                MotionIdCalculator.speedV2Mod.Decode(code, out lx, out ly);
                hx = 0;
                hy = 0;
            }
            else
            {
                throw new System.Exception("Id错误"+id);
            }
        }

        //传入当前速度，传出生效速度
        public Vector2 GetVelocity(Vector2 v, bool faceRight)
        {
            switch (type)
            {
                case 1:return Vector2.zero;
                case 2:return new Vector2(faceRight ? SpeedMap[lx] : -SpeedMap[lx], SpeedMap[ly]);
                case 3:return v + new Vector2(faceRight ? SpeedMap[lx] : -SpeedMap[lx], SpeedMap[ly]) * Time.deltaTime;
                case 4:return Vector2.Lerp(new Vector2(faceRight ? ShortSpeedMap[lx] : -ShortSpeedMap[lx], ShortSpeedMap[ly]), new Vector2(faceRight ? ShortSpeedMap[hx] : -ShortSpeedMap[hx], ShortSpeedMap[hy]), SpawnTime / workTime);
                case 5:return v;
            }
            Debug.LogError("错误的type");
            return v;
        }

        public Vector2 Entry(Vector2 v, bool faceRight)
        {
            switch (type)
            {
                case 1: return Vector2.zero;
                case 2: return new Vector2(faceRight ? SpeedMap[lx] : -SpeedMap[lx], SpeedMap[ly]);
                case 3: return v;
                case 4: return new Vector2(faceRight ? ShortSpeedMap[lx] : -ShortSpeedMap[lx], ShortSpeedMap[ly]);
                case 5: return new Vector2(SpeedMap[lx], SpeedMap[ly]);
            }
            Debug.LogError("错误的type");
            return v;
        }
        public Vector2 Exit(Vector2 v, bool faceRight)
        {
            switch (type)
            {
                case 1: return Vector2.zero;
                case 2: return new Vector2(faceRight?SpeedMap[lx]:-SpeedMap[lx], SpeedMap[ly]);
                case 3: return v;
                case 4: return new Vector2(faceRight?ShortSpeedMap[hx]:-ShortSpeedMap[hx], ShortSpeedMap[hy]);
                case 5: return v;
            }
            Debug.LogError("错误的type");
            return v;
        }
    }
}
