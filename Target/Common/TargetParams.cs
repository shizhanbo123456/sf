using System;
using XLua;

[LuaCallCSharp]
public enum TargetParams
{
    RegenerationRate,
    MonsterSkillCD,
    [Obsolete]LanternRegenerationTime,
    CanFly,
    Skill,
    HealthScale,
    [Obsolete]BodySize
}