using UnityEngine;

public class JumpState : PlayerBaseState
{
    public override void OnEnter(BasePlayerController ctx)
    {
        base.OnEnter(ctx);
        P.VerticalVelocity = P.jumpSpeed;

        //제자리 점프 보장: 진입 시 입력이 없으면 수평속도를 0으로
        var input = P.ReadMoveInput();
        if (input.sqrMagnitude < kMinInputSq)
            P.PlanarVelocity = Vector3.zero; // Move→Jump일 땐 기존 속도 유지, Idle→Jump면 0

        Anim_SwitchExclusive(P.hJump);
        Anim_SetGrounded(false);
    }

    protected override void Tick(float dt)
    {
        UpdateGrounded();

        var input = P.ReadMoveInput();
        float speed = P.WalkHeld() ? P.walkSpeed : P.runSpeed;
        var dir = CamRelativeMoveDir(input);

        P.PlanarVelocity = dir * speed;
        RotateTowards(dir, dt);

        ApplyGravity(dt);
        MoveCharacter(P.PlanarVelocity, P.VerticalVelocity, dt);

        if (P.VerticalVelocity <= 0f)
        {
            Anim_SwitchExclusive(P.hFall);
            Anim_SetGrounded(false);
            P.FSM.ChangeState(P, P.Fall);
        }
    }
}