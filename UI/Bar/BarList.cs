using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.UI.Bar
{
    public class BarList : MonoBehaviour
    {
        public List<BarBase> Bars = new List<BarBase>();


        public void LayoutBars()
        {
            float i = 0;
            foreach (var bar in Bars)
            {
                bar.transform.localPosition = new Vector3(bar.OccupyArea.x, -i);
                i += bar.OccupyArea.y;
            }
        }
    }
}