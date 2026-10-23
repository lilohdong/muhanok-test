# ARCHITECTURE.md

## 목적

게임 규칙, 입력 장치(키보드·웹캠), Unity 표현을 분리한다.
Clean Architecture를 그대로 적용하지 않고 게임 개발에 필요한 부분만 쓰는 Clean Architecture Lite를 사용한다.

---

## 전체 흐름

```
Player Body
→ Webcam (WebCamTexture)
→ BlazePoseEstimator (Unity Inference Engine, GPU)
→ PoseTracker → MotionClassifier
→ PoseGameInput ┐
                ├→ IGameInput → InputRouter → RunnerController → RunnerView
KeyboardGameInput ┘
```

게임은 명령이 키보드에서 왔는지 몸동작에서 왔는지 모른다. 모든 입력은 `MotionCommand`(Center, Left, Right, Jump, Crouch, HighKnee)로 들어온다.

---

## Unity 구조

`Unity/Assets/Game/` 아래에 어셈블리(asmdef) 단위로 나눈다.

| 폴더 / 어셈블리 | 역할 | UnityEngine 의존 |
| --- | --- | --- |
| `Core/` (Muhanok.LegacyCore) | 레인 규칙 `LaneRules` | 없음 |
| `Domain/` (Muhanok.Domain) | 게임 규칙: `RunRules`(흐름·속도·충돌·점수), `MotionClassifier`(관절 → 명령), `WorkoutEstimate`(칼로리), `Ranking`, `PowerUps`, `RoutePlan` | 없음 |
| `Application/` (Muhanok.Application) | `IGameInput`, `InputRouter` | 없음 |
| `Infrastructure/` (Muhanok.Infrastructure) | 키보드 입력, 포즈 추적·명령 변환(`Pose/PoseInput.cs`), 저장(`SaveRepository`, `UserSettingsStore`, `ExhibitionStore`) | 있음 |
| `PoseInference/` (Muhanok.PoseInference) | 웹캠 + BlazePose 추론 `BlazePoseEstimator`. `com.unity.ai.inference`가 없으면 컴파일에서 빠진다 | 있음 |
| `Presentation/` (Muhanok.Presentation) | 맵·플레이어·카메라·UI·오디오 MonoBehaviour | 있음 |
| `Bootstrap/` (Muhanok.Bootstrap) | `GameBootstrap`: 계층 조립, 씬 진입점 | 있음 |
| `Content/` | 프리팹, 애니메이션, 재질, `GameSettings.asset`, 포즈 모델 | — |
| `Editor/` | 맵·아트 생성기, 빌드·임포트 메뉴 | — |
| `Tests/` | EditMode(규칙·몸동작·전시), PlayMode(씬·충돌·시각 예산) | — |

의존 방향은 Presentation/Infrastructure → Application → Domain → Core다.
Domain은 UnityEngine 없이 유지해서 Unity 없이 테스트할 수 있게 한다.

`PoseInference`는 `PoseEstimatorRegistry`에 자신을 등록한다. Infrastructure는 `IPoseEstimator` 인터페이스만 알고 Inference Engine을 직접 참조하지 않는다.

---

## Player 구조

```
IGameInput → InputRouter → RunnerController (Domain) → RunnerView (Presentation) → Animator
```

RunnerController 안에 키보드나 웹캠 코드를 넣지 않는다.

---

## Map 구조

```
ChunkSpawner → MuhanokMapChunk (풀링) → MuhanokObstacleSocket → ObstacleController
```

- 청크 길이 24m, 레인 X -2.2 / 0 / +2.2.
- 각 청크는 Entry/Exit와 레인 규격을 공유하고, 지나간 청크와 장애물은 풀링으로 재사용한다.
- 경로: 광산 → 수레 → 계단 → 감옥 복도 → 사육장. 청크 프리팹은 `Content/Prefabs`, 생성기는 `Editor/MuhanokArtUpgrade.*`.

---

## Asset 교체 원칙

게임 판정(Collider, Socket, `ObstacleController.Kind`)은 유지하고 `EnvironmentRoot`/시각 메시만 교체한다.
교체 과정에서 게임 로직은 변경하지 않는다. 자세한 절차는 [ASSET_CATALOG.md](ASSET_CATALOG.md).

---

## 설정

속도, 레인 간격, 점프 높이, 몸동작 임계값, 카메라, 렌더 예산은 `Content/Data/GameSettings.asset`(ScriptableObject)에서 관리한다. 코드에 흩어놓지 않는다.
플레이어별 화면·오디오 설정은 `UserSettingsStore`가 저장한다.

---

## 패키지

Unity 6000.3.10f1, URP, Input System, Cinemachine, ProBuilder, Timeline, Inference Engine 2.6.1.
Audio/Animation/Director/ParticleSystem 내장 모듈은 애니메이션·소리·인트로·VFX에 필요해서 활성화했다.
기존 패키지 버전은 임의로 올리지 않는다.

---

## 핵심 원칙

의존성은 내부 게임 규칙 방향으로 향한다.
외부 기술(입력 장치, 추론 엔진, 저장소)을 교체해도 Domain이 영향을 받지 않게 한다.
단, 아키텍처를 지키기 위해 불필요한 Interface나 Class를 만들지 않는다.
