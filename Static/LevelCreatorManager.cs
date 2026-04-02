using LevelCreator;
using LevelCreator.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator 
{ 
    public partial class LevelCreatorManager : EnsBehaviour
    {
        private void Awake()
        {
            Tool.LevelCreatorManager = this;
        }

        private Dictionary<short, BulletInfo> BulletInfoCollection = new();
        private Dictionary<short, EffectInfo> EffectInfoCollection = new();
        private Dictionary<short, LandscapeInfo> LandscapeInfoCollection = new();
        private Dictionary<short, OperationInfo> OperationInfoCollection = new();
        private Dictionary<short, SkillInfo> SkillInfoCollection = new();
        private Dictionary<short, TargetInfo> TargetInfoCollection = new();

        public void LoadInfo(BulletInfo info)=> BulletInfoCollection.Add(info.id, info);
        public void LoadInfo(EffectInfo info)=> EffectInfoCollection.Add(info.id, info);
        public void LoadInfo(LandscapeInfo info)=> LandscapeInfoCollection.Add(info.id, info);
        public void LoadInfo(OperationInfo info)=> OperationInfoCollection.Add(info.id, info);
        public void LoadInfo(SkillInfo info)=> SkillInfoCollection.Add(info.id, info);
        public void LoadInfo(TargetInfo info)=> TargetInfoCollection.Add(info.id, info);


        private Action onFinish;
        public  void SyncInfo(Action onFinishSync)
        {
            onFinish=onFinishSync;
            StartCoroutine(SyncCoroutine());
        }
        private IEnumerator SyncCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            CustomLevel.OnInitTemplate();

            foreach (var info in BulletInfoCollection.Values)
            {
                CallFuncRpc(SyncBulletInfo,SendTo.ExcludeSender,Delivery.Reliable,info);
                yield return null;
            }
            foreach (var info in EffectInfoCollection.Values)
            {
                CallFuncRpc(SyncEffectInfo, SendTo.ExcludeSender, Delivery.Reliable, info);
                yield return null;
            }
            foreach (var info in LandscapeInfoCollection.Values)
            {
                CallFuncRpc(SyncLandscapeInfo, SendTo.ExcludeSender, Delivery.Reliable, info);
                yield return null;
            }
            foreach (var info in OperationInfoCollection.Values)
            {
                CallFuncRpc(SyncOperationInfo, SendTo.ExcludeSender, Delivery.Reliable, info);
                yield return null;
            }
            foreach (var info in SkillInfoCollection.Values)
            {
                CallFuncRpc(SyncSkillInfo, SendTo.ExcludeSender, Delivery.Reliable, info);
                yield return null;
            }
            foreach (var info in TargetInfoCollection.Values)
            {
                CallFuncRpc(SyncTargetInfo, SendTo.ExcludeSender, Delivery.Reliable, info);
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            onFinish?.Invoke();
        }
        [Rpc]
        private void SyncBulletInfo(BulletInfo info)
        {
            BulletInfoCollection.Add(info.id,info);
        }
        [Rpc]
        private void SyncEffectInfo(EffectInfo info)
        {
            EffectInfoCollection.Add(info.id, info);
        }
        [Rpc]
        private void SyncLandscapeInfo(LandscapeInfo info)
        {
            LandscapeInfoCollection.Add(info.id, info);
        }
        [Rpc]
        private void SyncOperationInfo(OperationInfo info)
        {
            OperationInfoCollection.Add(info.id, info);
        }
        [Rpc]
        private void SyncSkillInfo(SkillInfo info)
        {
            SkillInfoCollection.Add(info.id, info);
        }
        [Rpc]
        private void SyncTargetInfo(TargetInfo info)
        {
            TargetInfoCollection.Add(info.id, info);
            Debug.Log(info.graphicType);
        }


        public BulletInfo GetBulletInfo(short id) => BulletInfoCollection[id];
        public EffectInfo GetEffectInfo(short id) => EffectInfoCollection.ContainsKey(id)?EffectInfoCollection[id]:new(id,null);
        public LandscapeInfo GetLandscapeInfo(short id) => LandscapeInfoCollection[id];
        public OperationInfo GetOperationInfo(short id) => OperationInfoCollection[id];
        public SkillInfo GetSkillInfo(short id) => SkillInfoCollection[id];
        public TargetInfo GetTargetInfo(short id) => TargetInfoCollection[id];
    }
    public interface Info
    {

    }
}