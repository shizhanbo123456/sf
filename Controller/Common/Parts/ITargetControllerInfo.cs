using System;

public interface ITargetcontrollerInfo
{
    Action OnPostSyncRpc { get; set; }
    bool FaceRight { get; set; }
    bool isGrounded { get; set; }
    float Resistance { get; set; }
    bool IgnoreLevitaningPlatrm { get; set; }
    bool MotionIsNull { get; set; }
}