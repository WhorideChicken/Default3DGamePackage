using UnityEngine;

public class IdleState : PlayerBaseState
{
    public override void OnEnter(BasePlayerController ctx)
    {
        base.OnEnter(ctx);
        Anim_SwitchExclusive(P.hIdle);
        Anim_SetGrounded(true);
        P.PlanarVelocity = Vector3.zero;
    }

    protected override void Tick(float dt)
    {
        UpdateGrounded();

        if (P.IsGrounded && P.JumpPressed())
        {
            Anim_SwitchExclusive(P.hJump);
            Anim_SetGrounded(false);
            P.FSM.ChangeState(P, P.Jump);
            return;
        }

        
        var input = P.ReadMoveInput();
        if (input.sqrMagnitude >= kMinInputSq)
        {
            P.FSM.ChangeState(P, P.Move);
            return;
        }

        if (!P.IsGrounded)
        {
            Anim_SwitchExclusive(P.hFall);
            Anim_SetGrounded(false);
            P.FSM.ChangeState(P, P.Fall);
            return;
        }

        ApplyGravity(dt);
        MoveCharacter(P.PlanarVelocity, P.VerticalVelocity, dt);
    }
}