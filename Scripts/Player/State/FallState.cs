using UnityEngine;

public class FallState : PlayerBaseState
{
    public override void OnEnter(BasePlayerController ctx)
    {
        base.OnEnter(ctx);
        Anim_SwitchExclusive(P.hFall);
        Anim_SetGrounded(false);
    }

    protected override void Tick(float dt)
    {
        bool wasGrounded = P.IsGrounded;
        UpdateGrounded();

        var input = P.ReadMoveInput();
        float speed = P.WalkHeld() ? P.walkSpeed : P.runSpeed;
        var dir = CamRelativeMoveDir(input);

        P.PlanarVelocity = dir * speed;
        RotateTowards(dir, dt);

        ApplyGravity(dt);
        MoveCharacter(P.PlanarVelocity, P.VerticalVelocity, dt);

        if (P.IsGrounded)
        {
            Anim_SetGrounded(true);
            // 착지 후 입력 유무/Shift 상태로 즉시 스위칭
            if (input.sqrMagnitude < kMinInputSq)
            {
                Anim_SwitchExclusive(P.hIdle);
                P.FSM.ChangeState(P, P.Idle);
            }
            else
            {
                Anim_SwitchExclusive(P.WalkHeld() ? P.hWalk : P.hRun);
                P.FSM.ChangeState(P, P.Move);
            }
        }
    }
}