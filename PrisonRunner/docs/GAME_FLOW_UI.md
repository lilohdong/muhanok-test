# 게임 흐름과 화면

## 화면 흐름

```
타이틀 ──클릭──▶ 메인 메뉴 ┬ 게임 시작 → (처음이면 프로필 입력) → 탈출 준비(플레이 방식·카메라 확인) → 로딩 → 플레이
                           ├ 랭킹
                           ├ 상점
                           ├ 프로필
                           ├ 설정 (F1)
                           └ 게임 종료
플레이 → 일시정지: 계속하기 / 설정 / 메인으로 (기록 없이 종료)
플레이 → 결과(랭킹 화면): 다시 하기 / 메인으로
```

| 화면 | 코드 | 그리는 방식 |
| --- | --- | --- |
| 타이틀 | `Presentation/UI/TitleScreen.cs` | `HudSkin` |
| 메인 메뉴 | `Presentation/UI/Overlay/MainMenuScreen.cs` | `OverlaySkin` |
| 프로필 | `Presentation/UI/Overlay/ProfileScreen.cs` | `OverlaySkin` |
| 상점 | `Presentation/UI/Overlay/ShopScreen.cs` | `OverlaySkin` |
| 랭킹 / 결과 | `Presentation/UI/Overlay/LeaderboardScreen.cs` | `OverlaySkin` |
| 탈출 준비 | `Presentation/UI/SetupScreen.cs` | `HudSkin` |
| 플레이 HUD | `Presentation/UI/RunHud.cs` + `Overlay/ProgressHud.cs`(거리 미터·구간 바) | 둘 다 |
| 일시정지 | `Presentation/UI/PauseScreen.cs` | `HudSkin` |

`HUDPresenter`가 현재 상태와 메뉴 페이지(`MenuPage`)에 맞는 화면 하나만 그립니다. 두 스킨은 같은 팔레트(`Overlay/OverlaySkin.cs`의 `Palette`: 나무·금색)를 씁니다. 제목과 버튼은 Black Han Sans, `HudSkin`의 본문은 읽기 쉬운 맑은 고딕입니다.

## 플레이어 데이터

PC당 프로필 하나를 둡니다. 모두 `PlayerProgressStore`(Infrastructure)가 관리하며, 나중에 서버 저장소로 바꿀 때 이 클래스만 교체합니다.

| 파일 (`%USERPROFILE%\AppData\LocalLow\<회사>\<제품>\Player\`) | 내용 |
| --- | --- |
| `profile.json` | 이름, 몸무게(선택), 기본 플레이 방식 |
| `wallet.json` | 보유 코인, 해금한 아이템 |
| `records.json` | 모든 판의 기록(거리 순, 최대 500개)과 누적 통계(플레이 횟수, 최고 기록, 총 거리, 총 칼로리) |

- **랭킹**: 거리 기준입니다. 전체/오늘 탭이 있고, 결과 화면은 오늘 탭으로 열려 이번 판이 오늘 몇 위인지 바로 보여 줍니다.
- **상점**: 판에서 모은 코인이 지갑에 쌓이고, 상점에서 아이템을 해금하면 맵에 나옵니다. 가격은 `Domain/PowerUps.cs`의 `ItemCatalog`.
- **프로필**: 이름·몸무게·기본 플레이 방식과 누적 기록. 탈출 준비 화면에서 고른 플레이 방식은 기본값으로 저장됩니다.

## 개발자 모드 (전시용)

타이틀이나 메뉴에서 **Ctrl+Shift+F12**로 켜고 끕니다. 설정에 저장되어 다음 실행에도 유지됩니다. 켜져 있으면 메인 메뉴 왼쪽 아래에 표시가 나옵니다.

| | 일반 | 개발자 모드 |
| --- | --- | --- |
| 이름 | 프로필 이름 | 탈출 준비 화면에서 판마다 입력 (비우면 "플레이어 N") |
| 아이템 | 상점에서 해금한 것만 | 전부 |
| 코인 | 지갑에 적립 | 적립하지 않음 |
| 랭킹 메뉴 기본 탭 | 전체 | 오늘 |
| 결과 화면 버튼 | 다시 하기 / 메인으로 | 다시 하기 / 다음 플레이어 |
| Ctrl+F10 | 동작 안 함 | 오늘 기록 초기화 |

## 화면 확인 (개발 빌드)

**Muhanok → Build Windows Demo**로 빌드한 뒤 아래처럼 실행하면 모든 화면을 차례로 찍어 저장하고 종료합니다. `--muhanok-data`로 임시 폴더를 주면 실제 플레이어 데이터는 건드리지 않습니다.

```
Builds\Windows\Muhanok.exe -screen-width 1280 -screen-height 720 -screen-fullscreen 0 --muhanok-uicapture --muhanok-out <캡처 폴더> --muhanok-data <임시 데이터 폴더>
```
