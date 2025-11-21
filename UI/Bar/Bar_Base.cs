using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.UI.Bar
{
    public abstract class Bar_Base : MonoBehaviour
    {
        public Vector2 OccupyArea;
        public float Min = 0;
        public float Max = 100;
        public abstract void Init();
        public abstract void SetValue(float value);
    }
}