using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class TargetCollection : EnsBehaviourCollection
    {
        protected override void Init(string data)
        {
            string[] s = data.Split('/',System.StringSplitOptions.RemoveEmptyEntries);
            var creater = Tool.LevelCreatorManager.GetTargetInfo(short.Parse(s[0]));
            var identify = new TargetIdentify(int.Parse(s[1]), int.Parse(s[2]), creater.level, s[3], creater.size, float.Parse(s[4]), float.Parse(s[5]), creater.label);
            ApplyForTarget(creater,identify, gameObject);
        }
        public void ApplyForTarget(TargetInfo i,TargetIdentify identify,GameObject obj)
        {
            bool isLocalPlayer = identify.owner == EnsInstance.LocalClientId;
            TargetGraphic graphic;
            Target target;
            TargetController controller = null;
            TargetSkillController skillcontroller = null;
            TargetEffectController effectController = null;

            graphic = Object.Instantiate(Tool.PrefabManager.GraphicCollection[i.graphicType].gameObject,obj.transform).GetComponent<TargetGraphic>();
            graphic.transform.localPosition = Vector3.zero;

            switch (i.targetType)
            {
                case 0: target = obj.AddComponent<SingleTarget>(); break;
                case 1: target = obj.AddComponent<PlayerData>(); break;
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
                    case 0: skillcontroller = null; break;
                    case 1: skillcontroller = obj.AddComponent<PlayerSkillController>(); break;
                    case 2: skillcontroller = obj.AddComponent<AutoSkillController>(); break;
                    default: skillcontroller = null; break;
                }
                switch (i.effectControllerType)
                {
                    case 0: effectController = null; break;
                    case 1: effectController = obj.AddComponent<TargetEffectController>(); break;
                    default: effectController = null; break;
                }
                target.controller = controller;
                target.effectController = effectController;
                target.skillController = skillcontroller;
            }

            target.Init(identify, i.param);
            if (isLocalPlayer)
            {
                if (controller) controller.Init(target, i.param);
                if (skillcontroller) skillcontroller.Init(target, i.param);
                if (effectController) effectController.Init(target, i.param);
            }
            graphic.Init(obj,identify.camp);
        }
    }
}