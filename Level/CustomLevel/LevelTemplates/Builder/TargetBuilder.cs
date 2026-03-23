using LevelCreator.TargetTemplate;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    public static class TargetBuilder
    {
        private static TargetIdentify info;
        private static int targetType;//Player,Ore,Lantern,Monster
        private static int graphicType;

        private static int controllerType;//None,Player,Monster
        private static int skillControllerType;//None,Player,Monster
        private static int effectControllerType;//None,Default
        private static Dictionary<TargetParams, string> Params = new();
        public static void Init(TargetIdentify info, int targetType, int graphicType)
        {
            TargetBuilder.info = info;

            TargetBuilder.targetType = targetType;
            TargetBuilder.graphicType = graphicType;
        }
        public static void LoadController(int controllertype)
        {
            controllerType = controllertype;
        }
        public static void LoadSkillController(int skillcontrollertype)
        {
            skillControllerType = skillcontrollertype;
        }
        public static void LoadEffectController(int effectcontrollertype)
        {
            effectControllerType = effectcontrollertype;
        }
        public static void LoadParams(Dictionary<TargetParams, string> Params)
        {
            TargetBuilder.Params = Params;
        }
        public static void Upload()
        {
            new TargetInfo(info, targetType, graphicType, controllerType, skillControllerType, effectControllerType, Params);
        }
    }
    public struct TargetInfo:Info
    {
        private TargetIdentify info;
        private int targetType;//Player,Ore,Lantern,Monster
        private int graphicType;

        private int controllerType;//None,Player,Monster
        private int skillControllerType;//None,Player,Monster
        private int effectControllerType;//None,Default
        private Dictionary<TargetParams, string> param;
        public TargetInfo(TargetIdentify info, int targetType, int graphicType, int controllerType, int skillControllerType, int effectControllerType, Dictionary<TargetParams, string> param)
        {
            this.info = info;
            this.targetType = targetType;
            this.graphicType = graphicType;
            this.controllerType = controllerType;
            this.skillControllerType = skillControllerType;
            this.effectControllerType = effectControllerType;
            this.param = param;
        }
        public void ApplyForTarget(GameObject obj)
        {
            bool isLocalPlayer = info.owner == EnsInstance.LocalClientId;
            TargetGraphic graphic;
            Target target;
            TargetController controller = null;
            TargetSkillController skillcontroller = null;
            TargetEffectController effectController = null;

            graphic = Object.Instantiate(Tool.PrefabManager.GraphicCollection[graphicType].gameObject,
                Vector3.zero, Quaternion.identity, obj.transform).GetComponent<TargetGraphic>();
            switch (targetType)
            {
                case 0: target = obj.AddComponent<PlayerData>(); break;
                case 1: target = obj.AddComponent<Lantern>(); break;
                case 2: target = obj.AddComponent<Ore>(); break;
                case 3: target = obj.AddComponent<Monster>(); break;
                default: target = null; break;
            }
            target.graphic = graphic;

            if (isLocalPlayer)
            {
                switch (controllerType)
                {
                    case 0: controller = null; break;
                    case 1: controller = obj.AddComponent<PlayerController>(); break;
                    case 2: controller = obj.AddComponent<MonsterController>(); break;
                    default: controller = null; break;
                }
                switch (skillControllerType)
                {
                    case 0: skillcontroller = null; break;
                    case 1: skillcontroller = obj.AddComponent<PlayerSkillController>(); break;
                    case 2: skillcontroller = obj.AddComponent<MonsterSkillController>(); break;
                    default: skillcontroller = null; break;
                }
                switch (effectControllerType)
                {
                    case 0: effectController = null; break;
                    case 1: effectController = obj.AddComponent<TargetEffectController>(); break;
                    default: effectController = null; break;
                }
                target.controller = controller;
                target.effectController = effectController;
                target.skillController = skillcontroller;
            }

            target.Init(info, param);
            if (isLocalPlayer)
            {
                if (controller) controller.Init(target, param);
                if (skillcontroller) skillcontroller.Init(target, param);
                if (effectController) effectController.Init(target, param);
            }
            graphic.Init(obj);
        }
    }
}