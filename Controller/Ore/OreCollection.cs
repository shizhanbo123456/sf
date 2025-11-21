using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreCollection : EnsBehaviourCollection
{
    protected override void Init(string data)
    {
        Ore o=GetComponent<Ore>();
        o.Init(data);
    }
}
