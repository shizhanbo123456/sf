using AttributeSystem.Attributes;
using UnityEngine;

public class AttributesManager : MonoBehaviour
{
    private void Awake()
    {
        Tool.AttributesManager=this;
    }
    [Header("PlayerAttributes")]
    [SerializeField]private PlayerAttributes PlayerDefault;
    [SerializeField] private PlayerAttributes PlayerBattle;
    [Header("MonsterAttributes")]
    [SerializeField] private MonsterAttributes MonsterDefault;
    [Header("LanternAttributes")]
    [SerializeField] private LanternAttributes LanternDefault;
    [Header("OreAttributes")]
    [SerializeField] private TargetAttributes OreDefault;

    public TargetAttributes GetDynamicAttribute(Target data)
    {
        if (data == null) return PlayerDefault;
        if(data is PlayerData)return PlayerBattle;
        if(data is Monster m) return MonsterDefault;
        if (data is Lantern)return LanternDefault;
        if (data is Ore) return OreDefault;
        return null;
    }
}