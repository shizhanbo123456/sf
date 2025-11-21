using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidSkillLayoutGroup : MonoBehaviour
{
    public List<Transform> Pos=new List<Transform>();
    public void Update()
    {
        if (transform.childCount > 0)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                var t= transform.GetChild(i);
                t.position = Pos[i].position;
            }
            enabled = false;
        }
    }
}
