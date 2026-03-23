using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidSkillUse : MonoBehaviour
{
    [SerializeField] private Text SkillRef;
    private static List<string> SkillKeyContainer;
    private int KeyIndex;
    private bool Initialized = false;

    private void Init()
    {
        if (SkillKeyContainer == null)
        {
            SkillKeyContainer = new List<string>()
            {
                PlayerSkillController.Keys[0].ToString(),
                PlayerSkillController.Keys[1].ToString(),
                PlayerSkillController.Keys[2].ToString(),
                PlayerSkillController.Keys[3].ToString(),
                PlayerSkillController.Keys[4].ToString(),
                PlayerSkillController.Keys[5].ToString()
            };
        }
        for(int i=0;i<SkillKeyContainer.Count; i++)
        {
            if (SkillRef.text == SkillKeyContainer[i])
            {
                KeyIndex = i;
                break;
            }
        }
        Initialized= true;
    }
    public void OnClick()
    {
        if (!Initialized) Init();
        Tool.SubInput.AndroidUseSkill(KeyIndex);
    }
}
