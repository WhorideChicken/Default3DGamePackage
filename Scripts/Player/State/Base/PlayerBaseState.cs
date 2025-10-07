using UnityEngine;

public abstract class PlayerBaseState : IState<BasePlayerController>
{
    protected const float kMinInputSq = 0.01f;
    protected const float kMinRotDirSq = 0.001f;

    protected BasePlayerController P;

    public virtual void OnEnter(BasePlayerController ctx) { P = ctx; }
    public virtual void OnExit(BasePlayerController ctx)  { if (ReferenceEquals(P, ctx)) P = null; }
    public void OnUpdate(BasePlayerController ctx) { if (!ReferenceEquals(P, ctx)) P = ctx; Tick(Time.deltaTime); }
    protected abstract void Tick(float dt);

    // ── 공통 이동/중력/접지 ─────────────────────────────────
    protected Vector3 CamRelativeMoveDir(Vector2 input)
    {
        if (P.cameraTransform == null) return new Vector3(input.x, 0f, input.y).normalized;
        Vector3 fwd = P.cameraTransform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = P.cameraTransform.right; right.y = 0f; right.Normalize();
        Vector3 dir = right * input.x + fwd * input.y;
        return dir.sqrMagnitude > 1f ? dir.normalized : dir;
    }

    protected void RotateTowards(Vector3 dir, float dt)
    {
        if (dir.sqrMagnitude <= kMinRotDirSq) return;
        var target = Quaternion.LookRotation(dir, Vector3.up);
        P.transform.rotation = Quaternion.Slerp(P.transform.rotation, target, P.rotationSpeed * dt);
    }

    protected void MoveCharacter(Vector3 planar, float vertical, float dt)
    {
        P.CC.Move(new Vector3(planar.x, vertical, planar.z) * dt);
    }

    protected void ApplyGravity(float dt)
    {
        if (P.IsGrounded)
        {
            if (P.VerticalVelocity < 0f) P.VerticalVelocity = P.groundedGravity;
        }
        else P.VerticalVelocity += P.gravity * dt;
    }

    protected void UpdateGrounded()
    {
        if (P.CC.isGrounded) { P.IsGrounded = true; return; }
        Vector3 origin = P.transform.position + Vector3.up * (P.CC.radius + P.groundCheckOffset);
        P.IsGrounded = Physics.CheckSphere(origin, P.groundCheckRadius, P.groundMask, QueryTriggerInteraction.Ignore);
    }

    // ── ★ Animator Bool 전환 헬퍼 (BlendTree 없음) ───────────────────────────
    // 하나만 true, 나머지는 false로 맞춰줌 (Idle/Walk/Run/Jump/Fall)
    protected void Anim_SwitchExclusive(int activeHash)
    {
        if (!P.animator) return;
        P.animator.SetBool(P.hIdle, activeHash == P.hIdle);
        P.animator.SetBool(P.hWalk, activeHash == P.hWalk);
        P.animator.SetBool(P.hRun,  activeHash == P.hRun);
        P.animator.SetBool(P.hJump, activeHash == P.hJump);
        P.animator.SetBool(P.hFall, activeHash == P.hFall);
    }

    protected void Anim_SetGrounded(bool g)
    {
        if (!P.animator || P.hGrounded == 0) return;
        P.animator.SetBool(P.hGrounded, g);
    }
    
    
}
