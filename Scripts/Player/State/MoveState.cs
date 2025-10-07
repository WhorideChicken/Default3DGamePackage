using UnityEngine;

public class MoveState : PlayerBaseState
{
    public override void OnEnter(BasePlayerController ctx)
    {
        base.OnEnter(ctx);
        // 진입 시 애니 상태만 보장
        if (P.WalkHeld()) Anim_SwitchExclusive(P.hWalk);
        else              Anim_SwitchExclusive(P.hRun);
        Anim_SetGrounded(true);
    }

    protected override void Tick(float dt)
    {
        UpdateGrounded();

        var input = P.ReadMoveInput();
        if (input.sqrMagnitude < kMinInputSq)
        {
            Anim_SwitchExclusive(P.hIdle);
            P.FSM.ChangeState(P, P.Idle);
            return;
        }

        float speed = P.WalkHeld() ? P.walkSpeed : P.runSpeed;
        var dir = CamRelativeMoveDir(input);

        // 애니: Shift 여부에 따라 Walk/Run Bool만 바꿔줌
        if (P.WalkHeld()) Anim_SwitchExclusive(P.hWalk);
        else              Anim_SwitchExclusive(P.hRun);
        Anim_SetGrounded(P.IsGrounded);

        P.PlanarVelocity = dir * speed;
        RotateTowards(dir, dt);

        if (P.IsGrounded && P.JumpPressed())
        {
            Anim_SwitchExclusive(P.hJump);
            Anim_SetGrounded(false);
            P.FSM.ChangeState(P, P.Jump);
            return;
        }

        ApplyGravity(dt);
        MoveCharacter(P.PlanarVelocity, P.VerticalVelocity, dt);

        if (!P.IsGrounded)
        {
            Anim_SwitchExclusive(P.hFall);
            Anim_SetGrounded(false);
            P.FSM.ChangeState(P, P.Fall);
        }
    }
}