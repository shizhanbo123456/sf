using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnHandle : SpawnHandle
{
    public Monster.MonsterType Type;
    public override void Spawn()
    {
        string s = $"{transform.position.x:F1}_{transform.position.y:F1}";
        EnsInstance.NOMSpawner.CreateServerRpc(Tool.PrefabManager.MonsterCollections[(int)Type].NOMCollectionId, EnsBehaviour.SendTo.Everyone, s, KeyLibrary.KeyFormatType.Nonsequential);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
