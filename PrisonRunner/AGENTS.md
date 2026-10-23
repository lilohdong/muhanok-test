# AGENTS.md

## 현재 명세
- 현재 상태와 남은 일은 docs/PROGRESS.md, 문서 목록은 docs/README.md다. 버전별 과거 기록은 git 히스토리에 있다.
- 기획서는 docs/무한옥_화면별_게임기획서_V13.docx다. V13은 거리·속도 수치를 미정으로 둔 기획 문서이며 실제 수치는 GameSettings.asset을 따른다.
- GameScene 하나만 둔다. 흐름은 Title → 관람객 정보(이름·몸무게·플레이 방식) → LoadingIntro → Running → Caught → Result → 다시 하기/다음 관람객이다. Scene reload 금지.
- 경로는 광산 → 전용 수레 → 계단 → 내려오는 문 아래 슬라이드 → 감옥 복도 → 사육장이다. 본편 1인칭, 수레에서만 후방 3인칭. 3 lane X -2.2 / 0 / +2.2, 24m chunk, pooling.
- 고체 장애물은 한 번 충돌하면 종료한다. 바나나·물웅덩이는 감속만 하고, 바나나 누적 2회는 추격 후 잡힌다. 수레에는 일반 장애물이 없고 철로 붕괴를 반대쪽 기울이기로 피한다. 계단 하이니를 점프로 대체하지 않는다.
- 첫 바퀴는 9→17m/s 점진 가속(docs/MAP_EXERCISES_SPEED.md), 이후 휴식/질주 인터벌이다. 운동 장애물만 재배치할 때 MuhanokGameplayUpgrade.Apply, 전체 맵·아트 재생성은 MuhanokVisualPolish.UpgradeV10을 사용한다(docs/ART_V10.md).
- 독방 원숭이 NPC는 CellActors 아래에 두고 EnvironmentRoot 정적 메시와 합치지 않는다.
- 몸동작은 Unity Inference Engine에서 BlazePose(검출기+랜드마크)로 추정하고 Domain의 MotionClassifier가 전신/상체 명령으로 바꾼다. 상세는 docs/BODY_TRACKING.md.
- 게임 설정창은 UI Toolkit(Resources/MuhanokUI/SettingsWindow.uxml/.uss)이고 UI/UX 팀이 그래픽을 입힌다. 코드가 찾는 요소 name은 docs/SETTINGS_UI.md에 있으며 바꾸지 않는다. F1 설정, F3 성능 측정 표시.
- Domain은 UnityEngine 없이 유지한다. 기존 Core LaneRules는 재사용한다.
- 캐릭터는 자체 Blender Humanoid 모델이다. 달리기/점프/숙이기/비틀거림은 Mixamo 동작, 나머지는 자체 Blender 동작이다. 무료 경찰 에셋은 미임포트이며 현재 경찰은 자체 모델이다. 실제 적용·자체 제작·미임포트를 구분해 보고하고 상용 수준 품질을 달성했다고 주장하지 않는다. 교체 지점은 docs/ASSET_CATALOG.md.
- 기존 패키지 버전 업그레이드 금지. Timeline과 audio/animation/director/particlesystem 내장 모듈은 필수라서 활성화했다(docs/ARCHITECTURE.md).

## 프로젝트
- Unity 기반 3레인 Endless Runner 캡스톤 프로젝트다.
- 배경은 Stylized 3D 감옥이다.
- 웹캠 + Unity Inference Engine(BlazePose)으로 몸동작을 인식한다. 플레이 방식은 전신/상체/키보드다.
- 인식 결과를 Unity 캐릭터 조작으로 연결한다.
- 구현은 프로젝트 개발 규칙을 따른다.
- 기능 완성을 그래픽 완성보다 우선한다.

## 반드시 읽을 문서
- docs/DEVELOPMENT_PLAN.md
- docs/ARCHITECTURE.md
- docs/ART_DIRECTION.md

## 개발 원칙
- 작업 전 기존 코드를 먼저 확인한다.
- 요청하지 않은 전체 리팩터링을 하지 않는다.
- 정상 동작하는 기능을 이유 없이 수정하지 않는다.
- 한 번에 하나의 기능 단위로 개발한다.
- Placeholder로 먼저 기능을 검증한다.
- 과도한 추상화를 하지 않는다.
- 컴파일 가능한 상태를 유지한다.

## 게임
- 캐릭터는 자동으로 전진한다.
- LEFT / RIGHT는 레인 이동이다.
- JUMP는 점프다.
- CROUCH는 숙이기다.
- HIGH_KNEE는 계단 동작이다.
- IDLE은 기본 달리기다.
- 기본적으로 3개의 레인을 유지한다.
- 충돌 시 속도 감소 또는 Stumble을 적용한다.

## 아키텍처
- Clean Architecture Lite를 사용한다.
- Core는 순수 게임 규칙을 담당한다.
- Application은 게임 흐름을 담당한다.
- Infrastructure는 외부 시스템을 담당한다.
- Presentation은 Unity 표현 계층이다.
- 자세한 구조는 docs/ARCHITECTURE.md를 따른다.

## 입력
- 게임 로직은 키보드나 웹캠·추론 엔진을 직접 참조하지 않는다.
- 모든 입력은 공통 입력 인터페이스를 통해 전달한다.
- KeyboardGameInput을 항상 유지한다.
- 몸동작 입력은 PoseGameInput으로 별도 구현한다.
- Pose 입력이 실패해도 키보드 플레이가 가능해야 한다.

## 몸동작 인식
- PoseInference 어셈블리가 웹캠과 BlazePose 추론을 담당하고, Inference Engine 패키지가 없으면 컴파일에서 빠진다.
- MotionClassifier(Domain)가 관절 좌표를 레인 구역·점프·숙이기·하이니로 바꾸며 게임 로직은 MotionCommand만 받는다.
- 허용 명령은 CENTER LEFT RIGHT JUMP CROUCH HIGH_KNEE다.
- 사람이 안 보이면 명령을 보내지 않고, 오래 사라지면 다시 보정한다.

## Unity
- MonoBehaviour 하나에 여러 책임을 몰아넣지 않는다.
- Player 입력 이동 애니메이션 점수 맵 생성을 분리한다.
- 설정값은 가능한 ScriptableObject로 관리한다.
- Visual Mesh와 Collider를 분리한다.
- 모델 교체가 게임 로직에 영향을 주면 안 된다.

## Endless Map
- MapChunk 기반 구조를 사용한다.
- Chunk마다 EntrySocket과 ExitSocket을 가진다.
- 장애물 위치는 ObstacleSocket으로 관리한다.
- 모든 Chunk는 동일한 Lane 규격을 사용한다.
- 지나간 Chunk와 장애물은 Object Pooling으로 재사용한다.
- 피할 수 없는 장애물 패턴을 생성하지 않는다.

## Art
- 모든 그래픽은 docs/ART_DIRECTION.md를 따른다.
- Reference 이미지는 References/ 폴더에서 관리한다.
- Stylized 3D 스타일을 유지한다.
- 게임 가독성을 장식보다 우선한다.

## 외부 에셋
- AI 생성 에셋은 바로 Production에 넣지 않는다.
- 먼저 Staging에서 확인한다.
- 3D Model은 크기 Pivot Material Collider를 검수한다.
- API Key와 .env는 Git에 올리지 않는다.

## Git
- main은 발표 가능한 안정 버전이다.
- develop은 통합 개발 브랜치다.
- feature/*에서 개별 기능을 개발한다.
- 기능 완료 후 develop으로 병합한다.
- 발표 가능한 시점에 develop을 main으로 병합한다.
- 하나의 Commit은 하나의 목적만 가진다.
- 브랜치명과 커밋 메시지는 기능과 변경 목적만 표현한다.
- 작성 도구명이나 생성 주체에 대한 문구를 넣지 않는다.
- 자세한 작업 흐름은 docs/GIT_WORKFLOW.md를 따른다.

## 금지
- Unity 버전을 임의로 변경하지 않는다.
- 필요 없는 Package를 설치하지 않는다.
- API Key를 코드에 작성하지 않는다.
- Library Temp Logs Build를 Git에 Commit하지 않는다.
- Scene 전체를 이유 없이 다시 만들지 않는다.

## 작업 종료 보고
작업 완료 후 반드시 다음을 보고한다.
1. 생성한 파일
2. 수정한 파일
3. 구현한 기능
4. 테스트 결과
5. Compile Error 여부
6. 남은 문제
7. 다음 추천 작업
