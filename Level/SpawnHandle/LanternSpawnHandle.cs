using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternSpawnHandle : SpawnHandle
{
    public override void Spawn()
    {
        string s = $"{transform.position.x:F1}_{transform.position.y:F1}";
        EnsInstance.NOMSpawner.CreateServerRpc(Tool.PrefabManager.LanternCollection.NOMCollectionId, EnsBehaviour.SendTo.Everyone, s, KeyLibrary.KeyFormatType.Nonsequential);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f,188/255f,62/255f);
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
