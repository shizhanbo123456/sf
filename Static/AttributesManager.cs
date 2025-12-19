using AttributeSystem.Attributes;
using UnityEngine;

public class AttributesManager : MonoBehaviour
{
    private void Awake()
    {
        Tool.AttributesManager=this;
    }
    [SerializeField] private TargetAttributes DefaultAtt;

    public TargetAttributes GetDynamicAttribute(Target data)
    {
        return DefaultAtt;
    }
}