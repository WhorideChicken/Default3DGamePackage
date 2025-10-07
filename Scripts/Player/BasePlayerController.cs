using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class BasePlayerController : MonoBehaviour
{
    [Header("Speed")] public float runSpeed = 5.5f;
    public float walkSpeed = 2.6f; // LeftShift로 토글

    [Header("Rotation")] public float rotationSpeed = 10f; // Slerp 계수

    [Header("Jump / Gravity")] public float jumpSpeed = 7.5f;
    public float gravity = -20f;
    public float groundedGravity = -2f;

    [Header("Ground Check")] public LayerMask groundMask = ~0;
    public float groundCheckRadius = 0.25f;
    public float groundCheckOffset = 0.1f;

    [Header("Camera (optional)")] public Transform cameraTransform;

    // 런타임 캐시(상태들이 읽고/갱신)
    [HideInInspector] public CharacterController CC;
    [HideInInspector] public Vector3 PlanarVelocity; // xz
    [HideInInspector] public float VerticalVelocity; // y
    [HideInInspector] public bool IsGrounded; // 최신 ground 캐시

    [Header("Animation (Bool Params Only)")]
    public Animator animator;

    // 상태 전환용 Bool 파라미터명 (Exclusive One-Hot)
    [SerializeField] public string pIdle = "Idle";
    [SerializeField] public string pWalk = "Walk";
    [SerializeField] public string pRun = "Run";
    [SerializeField] public string pJump = "Jump";
    [SerializeField] public string pFall = "Fall";

    // 접지 상태(옵션)
    [SerializeField] public string pGrounded = "Grounded";
    [HideInInspector] public int hIdle, hWalk, hRun, hJump, hFall, hGrounded;
    
    
    
    
    // FSM
    public StateMachine<BasePlayerController> FSM { get; private set; }
    public IdleState Idle { get; private set; }
    public MoveState Move { get; private set; }
    public JumpState Jump { get; private set; }
    public FallState Fall { get; private set; }

    void Awake()
    {
        CC = GetComponent<CharacterController>();
        FSM = new StateMachine<BasePlayerController>();
        Idle = new IdleState();
        Move = new MoveState();
        Jump = new JumpState();
        Fall = new FallState();
        
        
        if (animator)
        {
            hIdle     = Animator.StringToHash(pIdle);
            hWalk     = Animator.StringToHash(pWalk);
            hRun      = Animator.StringToHash(pRun);
            hJump     = Animator.StringToHash(pJump);
            hFall     = Animator.StringToHash(pFall);
            hGrounded = string.IsNullOrEmpty(pGrounded) ? 0 : Animator.StringToHash(pGrounded);
        }
    }

    void Start() => FSM.ChangeState(this, Idle);
    void Update() => FSM.Update(this);

    // ── 입력: 상태들이 직접 호출 ─────────────────
    public Vector2 ReadMoveInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        var v = new Vector2(x, y);
        if (v.sqrMagnitude > 1f) v.Normalize();
        return v;
    }

    public bool WalkHeld() => Input.GetKey(KeyCode.LeftShift);
    public bool JumpPressed() => Input.GetKeyDown(KeyCode.Space);
}