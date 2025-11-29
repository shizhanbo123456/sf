using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem.Attributes
{
    [CreateAssetMenu(menuName = "Attributes/PlayerAttributes")]
    public class PlayerAttributes : TargetAttributes
    {
        [Space]
        public LinerGrowth Mofa;
        public LinerGrowth Huixie;
        public LinerGrowth Huimo;
    }
}