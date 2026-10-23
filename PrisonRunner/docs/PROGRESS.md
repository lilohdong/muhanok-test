# 무한옥 진행 상태

기준일 2026-10-23. 버전별 상세 기록은 git 히스토리에 있다.

## 구현 완료

| 영역 | 내용 | 문서 |
| --- | --- | --- |
| 게임 흐름 | 타이틀 → 메인 메뉴(게임 시작·랭킹·상점·프로필·설정·종료) → 탈출 준비 → 로딩 → 1인칭 달리기 → 결과(랭킹) → 다시 하기 / 메인으로. 씬 재로드 없음 | [GAME_FLOW_UI.md](GAME_FLOW_UI.md) |
| 맵 | 광산 → 수레(3인칭, 철로 붕괴 기울이기) → 계단(하이니) → 닫히는 문 슬라이드 → 감옥 복도·독방 → 사육장. 24m 청크 풀링 | [ART_V10.md](ART_V10.md) |
| 운동·속도 | 도보 청크마다 점프·슬라이드 행, 첫 바퀴 9→17m/s 점진 가속, 이후 휴식/질주 인터벌 | [MAP_EXERCISES_SPEED.md](MAP_EXERCISES_SPEED.md) |
| 몸동작 인식 | Unity Inference Engine + BlazePose, 전신/상체/키보드 모드, 자동 보정, 이어서 추적, One Euro 필터 | [BODY_TRACKING.md](BODY_TRACKING.md) |
| 플레이어 데이터 | PC당 프로필, 개인 코인 지갑과 상점 해금, 거리 기준 전체/오늘 랭킹, 누적 기록, 예상 칼로리(MET). 전시용 개발자 모드(전체 해금·판마다 이름) | [GAME_FLOW_UI.md](GAME_FLOW_UI.md) |
| 설정·성능 | UI Toolkit 설정창(F1), 성능 표시(F3), 화질 프리셋, 렌더 예산, GPU Resident Drawer | [SETTINGS_UI.md](SETTINGS_UI.md), [PERFORMANCE.md](PERFORMANCE.md) |
| 아트 | 자체 Blender 캐릭터·동작, Mixamo 동작 4개, Kenney CC0 소품, 독방·발 접지 IK | [ART_V10.md](ART_V10.md), [ASSET_CATALOG.md](ASSET_CATALOG.md) |

## 남은 확인

- 실제 웹캠으로 전신·상체 판정 정확도와 최고 속도 체감 난이도 현장 검수
- 저사양 노트북(내장 그래픽)에서 화질 프리셋별 FPS 측정
- 사육장 기린·코끼리 장애물은 기본 도형 임시 모델
- 무료 경찰 에셋 미임포트 (현재 자체 모델)

## 다음 후보

- 회원 정보·기록 저장 서버 (PostgreSQL + API, Docker)
- 얼굴 로그인 (BlazeFace + 얼굴 임베딩 모델, Inference Engine)
