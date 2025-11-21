using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternCollection : EnsBehaviourCollection
{
    protected override void Init(string data)
    {
        Lantern lantern = GetComponent<Lantern>();
        lantern.Init(data);
    }
}
