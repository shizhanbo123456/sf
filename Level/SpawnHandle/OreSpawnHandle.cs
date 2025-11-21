using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreSpawnHandle : SpawnHandle
{
    public override void Spawn()
    {
        string s = $"{transform.position.x:F1}_{transform.position.y:F1}_{Ore.OreIndexNext++}";
        EnsInstance.NOMSpawner.CreateServerRpc(Tool.PrefabManager.OreCollection.NOMCollectionId, EnsBehaviour.SendTo.Everyone,s, KeyLibrary.KeyFormatType.Nonsequential);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
