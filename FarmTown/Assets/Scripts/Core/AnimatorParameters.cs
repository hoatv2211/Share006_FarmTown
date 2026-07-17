using UnityEngine;

public static class AnimatorParameters
{
    public static readonly int Open = Animator.StringToHash("Open");
    public static readonly int Close = Animator.StringToHash("Close");
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Move = Animator.StringToHash("Move");
    public static readonly int CropPlotClick = Animator.StringToHash("CropPlotClick");
    public static readonly int SeedSelectionState = Animator.StringToHash("SeedSelectionState");
    public static readonly int SicklePanelOpen = Animator.StringToHash("SicklePanelOpen");
}
