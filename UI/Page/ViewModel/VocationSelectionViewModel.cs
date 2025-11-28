using System.Collections.Generic;
using Variety.Base;

public class VoctionSelectionViewModel:Singleton<VoctionSelectionViewModel>
{
    public List<List<SkillBase>>Skills=new List<List<SkillBase>>();
    public List<string>SkillPackageNames=new List<string>();
    public List<string>SkillDes=new List<string>();
    public VoctionSelectionViewModel()
    {
        Skills = Tool.VarietyManager.GetAllSkills();
        SkillPackageNames = Tool.VarietyManager.GetSkillPackageName();
        SkillDes = Tool.VarietyManager.GetSkillPackageDes();
    }
}