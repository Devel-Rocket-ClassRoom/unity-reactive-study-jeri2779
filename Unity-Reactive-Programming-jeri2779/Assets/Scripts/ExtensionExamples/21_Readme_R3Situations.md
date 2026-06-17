# ExtensionExamples

R3를 실제 Unity 게임 상황에 적용해 보는 응용 예제 폴더입니다.

이 폴더의 스크립트는 모두 `ExtensionExamples` 네임스페이스를 사용합니다. 기본 API의 문법을 익히는 목적은 `BasicExamples` 쪽에 두고, 이 폴더는 "게임 로직에서 이런 상황을 R3로 어떻게 표현할 수 있는가"에 초점을 둡니다.

GameObject에 예제 스크립트를 붙이고 Play 모드에서 Console 로그를 확인하면 됩니다. `[ContextMenu]`가 있는 예제는 컴포넌트 메뉴에서 직접 이벤트를 발생시킬 수 있습니다.

| File | Situation | Main R3 idea |
| --- | --- | --- |
| `01_DoubleClickReactiveExample.cs` | Double click detection | `Subject`, `Chunk`, `Debounce` |
| `02_FrameUpdateMovementExample.cs` | Move every frame | `EveryUpdate`, `AddTo` |
| `03_ReactiveInputMoveExample.cs` | WASD movement | `EveryUpdate`, `Select`, `Where` |
| `04_SkillCooldownExample.cs` | Skill cooldown | `Subject`, `ThrottleFirst` |
| `05_ReactiveHealthStateExample.cs` | HP, damage, death | `ReactiveProperty`, `Pairwise`, `DistinctUntilChanged` |
| `06_QuestProgressExample.cs` | Kill quest progress | `Subject`, `Where`, `Take` |
| `07_GameEventBusExample.cs` | Simple event bus | `Subject`, event filtering |
| `08_MergeDamageSourcesExample.cs` | Multiple damage sources | `Merge` |
| `09_CombineLatestCanAttackExample.cs` | Attack availability | `CombineLatest` |
| `10_PositionChangedExample.cs` | Movement threshold logging | `EveryUpdate`, `Pairwise` |
| `11_DebouncedSearchExample.cs` | Search input delay | `Debounce` |
| `12_ComboInputExample.cs` | Combo input window | `Chunk`, `Debounce` |
| `13_ReactiveSpawnerExample.cs` | Frame-based spawning | `IntervalFrame`, `Take` |
| `14_ReactiveGameStateExample.cs` | Game state changes | `ReactiveProperty`, `DistinctUntilChanged` |
| `15_UnityEventObservableExample.cs` | Inspector UnityEvent bridge | `AsObservable` |
| `16_ProximityAlertExample.cs` | Near/far alert | `EveryUpdate`, `DistinctUntilChanged` |
| `17_DebouncedAutoSaveExample.cs` | Auto-save after changes settle | `Skip`, `Debounce` |
| `18_LevelUpByExpExample.cs` | Level-up detection | `Select`, `Pairwise` |
| `19_PauseFilteredTickExample.cs` | Pause-aware ticking | `IntervalFrame`, `Where` |
| `20_TemporaryBuffExample.cs` | Temporary buff expiry | `ReactiveProperty`, `Timer` |
| `21_LowHpWarningExample.cs` | Low HP one-shot threshold | `Select`, `DistinctUntilChanged` |
| `22_ReactiveDoorExample.cs` | Key/door interaction | `Subject`, `Where` |

## 폴더 역할

| Folder | Role |
| --- | --- |
| `BasicExamples` | R3 기본 개념, 기본 생성법, 핵심 타입 학습 |
| `ExtensionExamples` | 입력, 이동, 전투, 퀘스트, 상태 전이 같은 게임 상황별 응용 |
