# 🧱 Base 3D Player Controller (Unity)

 **기본 플레이어 컨트롤러**로 쓰도록 만든,  
심플하지만 3D 게임을 만들 때 빠르게 가져가 수정 및 확장하기 위해 만 **FSM 기반 3D 캐릭터 컨트롤러**입니다.

BlendTree 없이 **Animator Bool 파라미터만으로 상태 전환**,  
**CharacterController** 기반의 충돌/이동, **카메라 상대 이동**,  
**점프 / 낙하 / 접지 판정**, **Shift 걷기/달리기 토글**을 기본 제공합니다.

---

## ⚙️ 핵심 특징

- ✅ **정석 FSM 구조**  
  `StateMachine<T>` + `IState<T>` + 구체 상태(Idle / Move / Jump / Fall)  
  각 상태는 입력 / 애니 / 물리를 **자기 책임 안에서** 처리.

- 🧠 **Animator는 Bool만 사용**  
  `Idle`, `Walk`, `Run`, `Jump`, `Fall` 5개 Bool을 **one-hot**으로 전환.  
  BlendTree 불필요.

- 🕹️ **입력 정책 (기본값)**  
  `Horizontal`, `Vertical`, `LeftShift`, `Space`  
  — `BasePlayerController`에 캡슐화되어 있으므로 손쉽게 교체 가능.

- 🎥 **카메라 상대 이동 + 자연스러운 회전**  
  카메라 Forward/Right 평면 투영으로 이동 방향 계산.  
  `Quaternion.Slerp`로 회전 보정.

- 🪂 **접지 / 중력 / 점프 완비**  
  `CharacterController.isGrounded` + `CheckSphere` 보조 접지.  
  착지/낙하/점프 상태 전환 정밀 제어.

---

## 📂 폴더 구조
/Scripts
├─ Common
│ └─ Define
│ └─ Define.cs // 공용 enum/상수 (PlayerState 등)
│
└─ Player
├─ BasePlayerController.cs // 메인 컨트롤러: CC/입력/애니/중력/FSM 소유
│
└─ State
├─ StateMachine.cs // 경량 FSM 구현 (ChangeState/Update)
├─ IState.cs // 상태 인터페이스 (OnEnter/OnExit/OnUpdate)
├─ IStateMachine.cs // FSM 인터페이스
│
├─ Base
│ └─ PlayerBaseState.cs // 모든 상태의 공통 유틸(이동/회전/중력/애니 헬퍼)
│
├─ IdleState.cs // 정지: 입력 시 Move, 점프 시 Jump, 낙하 시 Fall
├─ MoveState.cs // 이동: Shift=Walk/Run 스위칭, 전이 판정
├─ JumpState.cs // 점프: 초기 상승속도, 제자리 점프 처리, 상승→하강 전이
└─ FallState.cs // 낙하: 공중 이동/회전, 접지 시 Idle/Move 분기


---

## 🚀 빠른 시작 (Quick Start)

1. **빈 GameObject**에 `CharacterController`와 `BasePlayerController`를 붙입니다.
2. **Animator Controller**에 아래 Bool 파라미터를 추가:
   - `Idle`, `Walk`, `Run`, `Jump`, `Fall`, *(선택)* `Grounded`
3. `BasePlayerController` 인스펙터에서:
   - Speed, Rotation, Gravity 등 수치 조정
   - Animator / Camera Transform 참조 설정
4. 실행 → `Idle → Move / Jump / Fall` 자동 전이 확인.

---

## 🧩 상태 전이 요약

| 상태 | 전이 조건 | 특징 |
|------|------------|------|
| **Idle** | 입력 발생 → Move<br>Space → Jump<br>비접지 → Fall | 정지 중에도 중력 유지 |
| **Move** | 입력 없음 → Idle<br>Space → Jump<br>비접지 → Fall | Shift에 따라 Walk/Run |
| **Jump** | 상승 종료 후 → Fall | 제자리 점프 가능 |
| **Fall** | 접지 → Idle / Move | 낙하 중에도 이동/회전 허용 |

---

## 🎞️ Animator 세팅 가이드

- 파라미터: `Idle`, `Walk`, `Run`, `Jump`, `Fall`, *(선택)* `Grounded`
- 각 State의 전이 조건을 해당 Bool == true 로 설정.
- 코드에서 one-hot 관리 → 트리거 필요 없음.
- 낙하/착지 전환 시 자연스럽게 Idle/Move로 복귀.

---

## 🧭 이동 / 회전 / 중력 디테일

- **카메라 상대 이동**  
  카메라의 Forward/Right를 평면에 투영해 입력 방향 계산.
- **회전**  
  `Quaternion.Slerp`로 부드럽게 보간, 입력 미세 시 회전 생략.
- **중력/접지**  
  - 접지 시: `groundedGravity` (작은 음수) 적용  
  - 비접지 시: `gravity` 적분  
  - `CheckSphere`로 경사/계단에서도 안정적 판정.
- **계단 이동**  
  `CharacterController.stepOffset`, `slopeLimit` 값으로 제어.

---

## 🧱 BasePlayerController 주요 변수

| 구분 | 변수명 | 설명 |
|------|--------|------|
| 속도 | `runSpeed`, `walkSpeed` | 달리기 / 걷기 속도 |
| 회전 | `rotationSpeed` | 회전 속도 |
| 점프 | `jumpSpeed` | 초기 점프 상승속도 |
| 중력 | `gravity`, `groundedGravity` | 중력 계수 |
| 접지 | `groundMask`, `groundCheckRadius`, `groundCheckOffset` | 지면 판정 |
| 카메라 | `cameraTransform` | 상대 이동 기준 |
| 애니 | `pIdle`, `pWalk`, `pRun`, `pJump`, `pFall`, `pGrounded` | Animator Bool 파라미터명 |

---

## 🔌 입력 함수

| 함수 | 설명 |
|------|------|
| `ReadMoveInput()` | Horizontal/Vertical 입력 벡터 |
| `WalkHeld()` | Shift 입력 여부 |
| `JumpPressed()` | Space 입력 여부 |

> 새 Input System / 모바일 조이스틱으로 교체 시 이 함수들만 수정하면 됨.

---

## 🧰 커스터마이징 포인트

- **입력 교체** → `BasePlayerController`의 3개 입력 함수 수정  
- **애니 파라미터명 변경** → 인스펙터에서 직접 문자열 교체 가능  
- **상태 추가** → `PlayerBaseState` 상속 후 FSM 등록  
- **카메라 비사용 모드** → Camera Transform 미할당 시 월드 기준 이동  
- **전역 입력 막기 (Cutscene 등)** → `bool inputLocked` 추가 후 입력 함수에서 처리

---

## 🧙 퍼포먼스 / 디자인 노트

- 상태별로 **책임 분리 (SRP)** 유지 → 전이 조건은 각 상태 내부에서만 판정.
- `CheckSphere` 접지 보조로 계단/경사 안정화.
- Animator Bool one-hot 구조로 디버깅 간결.
- FSM 전환 + Animator 전환이 한 줄로 동기화.

---

## 📦 패키지화 / 배포

### ✅ A. 프로젝트 패키지 폴더
- `/Packages/com.yourorg.base-player/` 에 복사하여 사용.

### 🧩 B. Git 패키지
1. 저장소를 Git에 업로드  
2. Unity → **Window → Package Manager** → `+` → *Add package from Git URL...*  
3. `https://github.com/YourOrg/base-player.git` 입력

> asmdef와 샘플 프리팹을 포함시키면 UPM 호환 완벽.

---

## 💡 자주 묻는 질문

> **Q. BlendTree 꼭 안 써도 돼요?**  
> 네. Bool one-hot 스위칭만으로 충분히 부드럽습니다.

> **Q. 제자리 점프 가능한가요?**  
> 가능합니다. 입력 없을 시 수평속도 0으로 설정됩니다.

> **Q. 계단 처리는 어떻게 되나요?**  
> CharacterController의 기본 `stepOffset`, `slopeLimit`로 커버합니다.

---

## 📜 라이선스 / 크레딧

팀 / 조직 표준에 맞게 `LICENSE` 파일을 추가하세요.  
README 및 코드 구조는 자유롭게 수정 가능합니다.

---

## 📁 원본 파일 목록
- `Define.cs`  
- `BasePlayerController.cs`  
- `PlayerBaseState.cs`  
- `StateMachine.cs`  
- `IState.cs`, `IStateMachine.cs`  
- `IdleState.cs`, `MoveState.cs`, `JumpState.cs`, `FallState.cs`

---



