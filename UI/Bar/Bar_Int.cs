using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SF.UI.Bar
{
    public class Bar_Int : Bar_Base
    {
        [Space]
        [Header("Validate")]
        public List<GameObject> GrapicUnits = new List<GameObject>();
        public Vector2 GrapicOffset;
        public float GrapicInterval;
        public float GrapicScale = 0.5f;

        private void OnValidate()
        {
            if (!Application.isEditor) return;
            for (int i = 0; i < GrapicUnits.Count; i++)
            {
                GrapicUnits[i].transform.localPosition = GrapicOffset + i * new Vector2(GrapicInterval, 0);
                GrapicUnits[i].transform.localScale = new Vector3(GrapicScale, GrapicScale, 1);
            }
        }
        public override void Init()
        {

        }
        public override void SetValue(float value)
        {

        }
    }
}