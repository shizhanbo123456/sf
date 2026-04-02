using LevelCreator.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public partial class TargetDataSync : EnsBehaviour
    {
        private Target target;
        public DedicateSyncAttributes DedicatedAttributes;
        public void Init(Target target, Dictionary<TargetParams, string> param)
        {
            this.target = target;
            nomEnabled = target.UpdateLocally;
            float healthRate = param.ContainsKey(TargetParams.HealthScale) ? float.Parse(param[TargetParams.HealthScale]) : 1;
            DedicatedAttributes = new DedicateSyncAttributes(target.Level, healthRate);
        }
        public override void ManagedUpdate()
        {
            if (HealthDirtyClearCD > 0) HealthDirtyClearCD -= Time.deltaTime;
            else if (HealthDirty)
            {
                HealthDirty = false;
                CallFuncRpc(OnSyncHealthLocal, SendTo.ExcludeSender, Delivery.Unreliable, DedicatedAttributes.Shengming.Value.Item1,DedicatedAttributes.Shengming.Value.Item2);
                HealthDirtyClearCD = 0.15f;
            }
        }

        private float HealthDirtyClearCD;
        private bool HealthDirty;
        public void SyncShengming(int max, int present)
        {
            HealthDirty = true;
            DedicatedAttributes.Shengming.Value = (max, present);
        }
        [Rpc]
        private void OnSyncHealthLocal(int max,int present)
        {
            DedicatedAttributes.Shengming.Value = (max,present);
        }
        public void SyncGongji(int value)
        {
            CallFuncRpc(Sgj, SendTo.ExcludeSender, Delivery.Reliable,value);
            DedicatedAttributes.Gongji = value;
        }
        [Rpc]
        private void Sgj(int value)
        {
            DedicatedAttributes.Gongji = value;
        }
        public void SyncMingzhong(int value)
        {
            CallFuncRpc(Smz, SendTo.ExcludeSender, Delivery.Reliable, value);
            DedicatedAttributes.Mingzhong = value;
        }
        [Rpc]
        private void Smz(int value)
        {
            DedicatedAttributes.Mingzhong = value;
        }
        public void SyncBaoji(int value)
        {
            CallFuncRpc(Sbj, SendTo.ExcludeSender, Delivery.Reliable, value);
            DedicatedAttributes.Baoji = value;
        }
        [Rpc]
        private void Sbj(int value)
        {
            DedicatedAttributes.Baoji = value;
        }
        public void SyncJiashang(int value)
        {
            CallFuncRpc(Sjs, SendTo.ExcludeSender, Delivery.Reliable, value);
            DedicatedAttributes.Jiashang = value;
        }
        [Rpc]
        private void Sjs(int value)
        {
            DedicatedAttributes.Jiashang = value;
        }



        public void UseSkillRpc(short index)
        {
            CallFuncRpc(UseSkillLocal, SendTo.Everyone, Delivery.Unreliable, index,target.FaceRight);
        }
        [Rpc]
        private void UseSkillLocal(short index,bool faceright)
        {
            SkillExecuter.UseSkill(index,target, target.transform.position, faceright);
        }


        public void SyncEffectIconRpc(HashSet<(EffectType, int)> values)
        {
            if (values == null || values.Count == 0)
            {
                CallFuncRpc(SyncEffectIconLocal, SendTo.Everyone, Delivery.Unreliable, "null");
                return;
            }
            CallFuncRpc(SyncEffectIconLocal, SendTo.Everyone, Delivery.Unreliable, Format.ListToString(values, t => ((int)t.Item1).ToString(), '+'));
        }
        [Rpc]
        private void SyncEffectIconLocal(string data)
        {
            if (data == "null")
            {
                target.graphic.ShowEffects(new List<EffectType>());
                return;
            }
            var list = Format.StringToList(data, int.Parse, '+');
            target.graphic.ShowEffects(list.Select(i => (EffectType)i).ToList());
        }


        public void DestroyRpc()
        {
            CallFuncRpc(DestroyLocal, SendTo.Everyone, Delivery.Reliable);
        }
        [Rpc]
        private void DestroyLocal()
        {
            Destroy(gameObject);
        }
    }
}