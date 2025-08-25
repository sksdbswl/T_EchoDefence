using UnityEngine;

public static class PlayerAnimationController
{
    public static readonly int Idle = Animator.StringToHash("IDLE");
    public static readonly int Run  = Animator.StringToHash("RUN");
    public static readonly int Hit  = Animator.StringToHash("HIT");
}