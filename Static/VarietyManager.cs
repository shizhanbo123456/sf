using System;
using System.Collections;
using System.Collections.Generic;
using Variety.Base;
using vs = Variety.Skill;

public static class VarietyManager
{
    public static Dictionary<int,SkillBase> PlayerSkills = new()
    {
        {0, new vs.PackageA.Skill0() },
        {1, new vs.PackageA.Skill1() },
        {2, new vs.PackageA.Skill2() },
        {3, new vs.PackageA.Skill3() },
        {4, new vs.PackageA.Skill4() },
        {5, new vs.PackageA.Skill5() },

        {10, new vs.PackageB.Skill0() },
        {11, new vs.PackageB.Skill1() },
        {12, new vs.PackageB.Skill2() },
        {13, new vs.PackageB.Skill3() },
        {14, new vs.PackageB.Skill4() },
        {15, new vs.PackageB.Skill5() },

        {20, new vs.PackageC.Skill0() },
        {21, new vs.PackageC.Skill1() },
        {22, new vs.PackageC.Skill2() },
        {23, new vs.PackageC.Skill3() },
        {24, new vs.PackageC.Skill4() },
        {25, new vs.PackageC.Skill5() },
    };
    public static Dictionary<int,SkillBase> BossSkills = new()
    {
        {1000, new vs.Boss1.Skill0() },
        {1001, new vs.Boss1.Skill1() },
        {1002, new vs.Boss1.Skill2() },
        {1003, new vs.Boss1.Skill3() },
        {1004, new vs.Boss1.Skill4() },
        {1005, new vs.Boss1.Skill5() },

        {1010, new vs.Boss2.Skill0() },
        {1011, new vs.Boss2.Skill1() },
        {1012, new vs.Boss2.Skill2() },
        {1013, new vs.Boss2.Skill3() },
        {1014, new vs.Boss2.Skill4() },
        {1015, new vs.Boss2.Skill5() },

        {1020, new vs.Boss3.Skill0() },
        {1021, new vs.Boss3.Skill1() },
        {1022, new vs.Boss3.Skill2() },
        {1023, new vs.Boss3.Skill3() },
        {1024, new vs.Boss3.Skill4() },
        {1025, new vs.Boss3.Skill5() },

        {1030, new vs.Boss4.Skill0() },
        {1031, new vs.Boss4.Skill1() },
        {1032, new vs.Boss4.Skill2() },
        {1033, new vs.Boss4.Skill3() },
        {1034, new vs.Boss4.Skill4() },
        {1035, new vs.Boss4.Skill5() },

        {1040, new vs.Boss5.Skill0() },
        {1041, new vs.Boss5.Skill1() },
        {1042, new vs.Boss5.Skill2() },
        {1043, new vs.Boss5.Skill3() },
        {1044, new vs.Boss5.Skill4() },
        {1045, new vs.Boss5.Skill5() },

        {1050, new vs.Boss6.Skill0() },
        {1051, new vs.Boss6.Skill1() },
        {1052, new vs.Boss6.Skill2() },
        {1053, new vs.Boss6.Skill3() },
        {1054, new vs.Boss6.Skill4() },
        {1055, new vs.Boss6.Skill5() },

        {1060, new vs.Boss7.Skill0() },
        {1061, new vs.Boss7.Skill1() },
        {1062, new vs.Boss7.Skill2() },
        {1063, new vs.Boss7.Skill3() },
        {1064, new vs.Boss7.Skill4() },
        {1065, new vs.Boss7.Skill5() },

        {1070, new vs.Boss8.Skill0() },
        {1071, new vs.Boss8.Skill1() },
        {1072, new vs.Boss8.Skill2() },
        {1073, new vs.Boss8.Skill3() },
        {1074, new vs.Boss8.Skill4() },
        {1075, new vs.Boss8.Skill5() },

        {1080, new vs.Boss9.Skill0() },
        {1081, new vs.Boss9.Skill1() },
        {1082, new vs.Boss9.Skill2() },
        {1083, new vs.Boss9.Skill3() },
        {1084, new vs.Boss9.Skill4() },
        {1085, new vs.Boss9.Skill5() },

        {1090, new vs.Boss10.Skill0() },
        {1091, new vs.Boss10.Skill1() },
        {1092, new vs.Boss10.Skill2() },
        {1093, new vs.Boss10.Skill3() },
        {1094, new vs.Boss10.Skill4() },
        {1095, new vs.Boss10.Skill5() },

        {1100, new vs.Boss11.Skill0() },
        {1101, new vs.Boss11.Skill1() },
        {1102, new vs.Boss11.Skill2() },
        {1103, new vs.Boss11.Skill3() },
        {1104, new vs.Boss11.Skill4() },
        {1105, new vs.Boss11.Skill5() },

        {1110, new vs.Boss12.Skill0() },
        {1111, new vs.Boss12.Skill1() },
        {1112, new vs.Boss12.Skill2() },
        {1113, new vs.Boss12.Skill3() },
        {1114, new vs.Boss12.Skill4() },
        {1115, new vs.Boss12.Skill5() },

        {1120, new vs.Boss13.Skill0() },
        {1121, new vs.Boss13.Skill1() },
        {1122, new vs.Boss13.Skill2() },
        {1123, new vs.Boss13.Skill3() },
        {1124, new vs.Boss13.Skill4() },
        {1125, new vs.Boss13.Skill5() },

        {1130, new vs.Boss14.Skill0() },
        {1131, new vs.Boss14.Skill1() },
        {1132, new vs.Boss14.Skill2() },
        {1133, new vs.Boss14.Skill3() },
        {1134, new vs.Boss14.Skill4() },
        {1135, new vs.Boss14.Skill5() },

        {1140, new vs.Boss15.Skill0() },
        {1141, new vs.Boss15.Skill1() },
        {1142, new vs.Boss15.Skill2() },
        {1143, new vs.Boss15.Skill3() },
        {1144, new vs.Boss15.Skill4() },
        {1145, new vs.Boss15.Skill5() },

        {1150, new vs.Boss16.Skill0() },
        {1151, new vs.Boss16.Skill1() },
        {1152, new vs.Boss16.Skill2() },
        {1153, new vs.Boss16.Skill3() },
        {1154, new vs.Boss16.Skill4() },
        {1155, new vs.Boss16.Skill5() },

        {1160, new vs.Boss17.Skill0() },
        {1161, new vs.Boss17.Skill1() },
        {1162, new vs.Boss17.Skill2() },
        {1163, new vs.Boss17.Skill3() },
        {1164, new vs.Boss17.Skill4() },
        {1165, new vs.Boss17.Skill5() },

        {1170, new vs.Boss18.Skill0() },
        {1171, new vs.Boss18.Skill1() },
        {1172, new vs.Boss18.Skill2() },
        {1173, new vs.Boss18.Skill3() },
        {1174, new vs.Boss18.Skill4() },
        {1175, new vs.Boss18.Skill5() },
    };

    private static Dictionary<int,RepeatContent> BossRepeatContents = new()
    {
        { 1002,new vs.Boss2.RepeatBoss()},
        { 1003,new vs.Boss3.RepeatBoss()},
        { 1004,new vs.Boss4.RepeatBoss()},
        { 1005,new vs.Boss5.RepeatBoss()},
        { 1008,new vs.Boss8.RepeatBoss()},
        { 1011,new vs.Boss11.RepeatBoss()},
        { 1013,new vs.Boss13.RepeatBoss()},
        { 1014,new vs.Boss14.RepeatBoss()},
        { 1015,new vs.Boss15.RepeatBoss()},
        { 1016,new vs.Boss16.RepeatBoss()},
        { 1017,new vs.Boss17.RepeatBoss()},
        { 1018,new vs.Boss18.RepeatBoss()},
    };
    public static SkillBase GetSkill(int index)
    {
        if (index >= 0 && index < 1000) return PlayerSkills[index];
        else if(index>=1000&&index<1999)return BossSkills[index];
        UnityEngine.Debug.LogError("Î´ÕÒµ½¼¼ÄÜ£º" + index);
        return null;
    }
    public static RepeatContent GetRepeatContent(int index)
    {
        return BossRepeatContents[index];
    }
}
