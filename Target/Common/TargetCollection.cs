using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class TargetCollection : EnsBehaviourCollection
    {
        protected override void Init(string data)
        {
            string[] s = data.Split('/',System.StringSplitOptions.RemoveEmptyEntries);
            var creator = Tool.LevelCreatorManager.GetTargetInfo(ushort.Parse(s[0]));
            var identify = new TargetIdentify(int.Parse(s[1]), int.Parse(s[2]), creator.level, s[3], creator.size, float.Parse(s[4]), float.Parse(s[5]), creator.label);
            ApplyForTarget(creator,identify, gameObject);
        }
        public void ApplyForTarget(TargetInfo i,TargetIdentify identify,GameObject obj)
        {
            bool isLocalPlayer = identify.owner == EnsInstance.LocalClientId;
            TargetGraphic graphic;
            Target target;
            TargetController controller = null;
            TargetSkillController skillController = null;
            TargetEffectController effectController = null;

            graphic = Object.Instantiate(Tool.PrefabManager.GraphicCollection[i.graphicType].gameObject,obj.transform).GetComponent<TargetGraphic>();
            graphic.transform.localPosition = Vector3.zero;

            switch (i.targetType)
            {
                case 0: target = obj.AddComponent<SingleTarget>(); break;
                case 1: target = obj.AddComponent<PlayerTarget>(); break;
                case 2: target = obj.AddComponent<BossTarget>(); break;
                default: target = null; break;
            }
            target.graphic = graphic;

            if (isLocalPlayer)
            {
                switch (i.controllerType)
                {
                    case 0: controller = null; break;
                    case 1: controller = obj.AddComponent<PlayerController>(); break;
                    case 2: controller = obj.AddComponent<AutoController>(); break;
                    default: controller = null; break;
                }
                switch (i.skillControllerType)
                {
                    case 0: skillController = null; break;
                    case 1: skillController = obj.AddComponent<PlayerSkillController>(); break;
                    case 2: skillController = obj.AddComponent<AutoSkillController>(); break;
                    default: skillController = null; break;
                }
                switch (i.effectControllerType)
                {
                    case 0: effectController = null; break;
                    case 1: effectController = obj.AddComponent<TargetEffectController>(); break;
                    default: effectController = null; break;
                }
                target.controller = controller;
                target.effectController = effectController;
                target.skillController = skillController;
            }

            var param = new Dictionary<TargetParams, string>();
            foreach (var p in i.param) param.Add((TargetParams)p.Key,p.Value);
            target.Init(identify,param);
            if (isLocalPlayer)
            {
                if (controller) controller.Init(target, param);
                if (skillController) skillController.Init(target, param);
                if (effectController) effectController.Init(target, param);
            }
            graphic.Init(obj,identify.camp);
            if(param.TryGetValue(TargetParams.Visibility,out string visibility)&&int.TryParse(visibility,out int vis)&&vis==0) graphic.SetAsInvisiable();
        }
    }
}