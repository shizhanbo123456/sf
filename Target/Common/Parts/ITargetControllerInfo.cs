using LevelCreator;
using System;

public interface ITargetcontrollerInfo
{
    Action OnPostSync { get; set; }
    TargetTransformInfo Info { get; set; }
}
public struct TargetTransformInfo
{
    public bool faceRight;
    public bool isGrounded;
    public bool hitDown;
    public bool ignoreLevitatingPlatform;
    public bool motionIsNull;
    public enum StateFlags
    {
        FaceRight=1,
        IsGrounded = 2,
        HitDown = 4,
        IgnorLevitatingPlatform = 8,
        MotionIsNull = 16,
    }
    public static TargetTransformInfo Create() => new()
    {
        faceRight = true,
        isGrounded = true,
        hitDown = false,
        ignoreLevitatingPlatform = false,
        motionIsNull = true
    };
    public TargetTransformInfo(bool faceright,bool isgrouded,bool hitdown,bool ignorelevitatingplatform,bool motionisnull)
    {
        faceRight= faceright;
        isGrounded= isgrouded;
        hitDown= hitdown;
        ignoreLevitatingPlatform= ignorelevitatingplatform;
        motionIsNull= motionisnull;
    }
    public TargetTransformInfo(int id)
    {
        StateFlags flags = (StateFlags)id;
        faceRight = flags.HasFlag(StateFlags.FaceRight);
        isGrounded = flags.HasFlag(StateFlags.IsGrounded);
        hitDown = flags.HasFlag(StateFlags.HitDown);
        ignoreLevitatingPlatform = flags.HasFlag(StateFlags.IgnorLevitatingPlatform);
        motionIsNull = flags.HasFlag(StateFlags.MotionIsNull);
    }
    public TargetTransformInfo(StateFlags flags)
    {
        faceRight=flags.HasFlag(StateFlags.FaceRight);
        isGrounded=flags.HasFlag(StateFlags.IsGrounded);
        hitDown=flags.HasFlag(StateFlags.HitDown);
        ignoreLevitatingPlatform = flags.HasFlag(StateFlags.IgnorLevitatingPlatform);
        motionIsNull=flags.HasFlag(StateFlags.MotionIsNull);
    }
    public StateFlags ToFlags()
    {
        StateFlags flag = 0;
        if (faceRight) flag |= StateFlags.FaceRight;
        if (isGrounded) flag |= StateFlags.IsGrounded;
        if (hitDown) flag |= StateFlags.HitDown;
        if (ignoreLevitatingPlatform) flag |= StateFlags.IgnorLevitatingPlatform;
        if (motionIsNull) flag |= StateFlags.MotionIsNull;
        return flag;
    }

    public readonly bool Compare(TargetTransformInfo info)
    {
        return faceRight == info.faceRight &&
                isGrounded == info.isGrounded &&
                hitDown == info.hitDown &&
                ignoreLevitatingPlatform == info.ignoreLevitatingPlatform &&
                motionIsNull == info.motionIsNull;
    }
}