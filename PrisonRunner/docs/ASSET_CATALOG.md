# 무한옥 에셋 현황과 교체 지점

무료 에셋과 자체 제작만 사용한다. 출처와 라이선스는 [THIRD_PARTY_NOTICES.md](../THIRD_PARTY_NOTICES.md)와 [FREE_ASSETS_IMPORTED.json](FREE_ASSETS_IMPORTED.json)에 있다.

## 현재 사용 중

| 구분 | 출처 | 위치 |
| --- | --- | --- |
| 원숭이·경찰 캐릭터 | 자체 Blender Humanoid 모델 | 원본 `References/Blender/MuhanokCharacters.blend`, FBX `Assets/External/Staging/Blender/` |
| 달리기·점프·숙이기·비틀거림 | Adobe Mixamo 동작 4개 (공개 소스에서는 제외, 자체 동작으로 자동 대체) | `Assets/External/Staging/Mixamo/` |
| 그 외 동작 (채굴, 수레 그립, 독방 대기 등) | 자체 Blender 동작 | 위 Blender 원본 |
| 광산·감옥·사육장 소품 | Kenney CC0 키트 5종 + 자체 Blender 모듈 | `Assets/External/Staging/Kenney/`, `References/Blender/MuhanokModules.blend` |
| 맵 경로 메시 | ProBuilder로 편집 후 런타임 메시로 베이크 | `Assets/Game/Authoring/ProBuilder/` |
| 손그림 재질 아틀라스 | 이미지 생성 (출처 `References/GeneratedV6/Provenance.md`) | `Assets/External/Staging/GeneratedV6/` |
| 배경 음악·효과음 | 실행 중 코드로 합성 | `Presentation/Audio/GameAudio.cs` |

무료 경찰 에셋(BitGem Police Officer 107256)은 다운로드 인증 문제로 임포트하지 못했다. 화면의 경찰은 자체 모델이다.

## 안전하게 교체하는 방법

1. 새 에셋을 `Assets/External/Staging`에 임포트하고 크기, Pivot, 재질(URP), 리그를 확인한다.
2. `GameScene`의 `Muhanok Bootstrap`에서 원숭이·경찰·수레·곡괭이·VFX 프리팹 필드를 지정한다. 코드 수정 없이 아트만 바뀐다.
3. 캐릭터 Animator state 이름을 `Content/Animations`의 `AN_Monkey_*`, `AN_Guard_*`에 맞추거나 Animator Override Controller로 연결한다. 게임 이동은 root motion에 의존하지 않는다.
4. 청크 프리팹의 `Entry`, `Exit`, `Lane_*`, `ObstacleSockets`, `ObstacleController`는 유지하고 `EnvironmentRoot`만 교체한다. 길이 24m, 레인 X -2.2 / 0 / +2.2를 유지한다.
5. 장애물은 `ObstacleController.Kind`를 유지하고 시각 메시만 교체한다.
6. 교체 후 EditMode/PlayMode 테스트를 실행하고 화면을 확인한다.
