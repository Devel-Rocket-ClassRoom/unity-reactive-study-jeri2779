# BasicExamples

R3의 기본 개념과 핵심 타입을 작은 MonoBehaviour 예제로 확인하는 폴더입니다.

이 폴더의 스크립트는 모두 `BasicExamples` 네임스페이스를 사용합니다. GameObject에 붙이고 Play 모드에서 Console 로그를 확인하거나, `[ContextMenu]`가 있는 예제는 컴포넌트 메뉴에서 직접 실행할 수 있습니다.

## 구성 기준

| 구분 | 파일 | 내용 |
| --- | --- | --- |
| 문서 01 성격 | `01_PollingVsReactiveHpExample.cs` | 폴링 방식과 리액티브 방식의 HP 변경 감지 비교 |
| 문서 02 기본 | `01_FirstSubscribeSample.cs` | 첫 구독, `Subscribe`, 값/에러/완료 채널 |
| 문서 02 기본 | `02_FactoryObservableSample.cs` | `Range`, `Timer`, `Interval`, `Return` |
| 문서 02 기본 | `03_SubjectScoreSample.cs` | `Subject<T>`로 직접 이벤트 발행 |
| 문서 02 기본 | `04_ReactivePropertyHpSample.cs` | 값을 보유하는 `ReactiveProperty` |
| 문서 02 기본 | `05_FromEventBridgeSample.cs` | 기존 C# `event`를 `Observable`로 변환 |
| 문서 02 기본 | `06_EveryUpdateSample.cs` | Unity의 매 프레임 흐름을 `EveryUpdate`로 처리 |
| 기본 확장 타입 | `07_CreateCustomSample.cs` | `Observable.Create`로 직접 스트림 생성 |
| 기본 확장 타입 | `08_EdgeStreamsSample.cs` | `Empty`, `Never`, `Throw`, `Defer` |
| 기본 확장 타입 | `09_SubjectVariantsSample.cs` | `Subject`, `BehaviorSubject`, `ReplaySubject` 비교 |
| 기본 확장 타입 | `10_FrameBasedSample.cs` | `NextFrame`, `TimerFrame`, `IntervalFrame` |
| 기본 확장 타입 | `11_EveryValueChangedSample.cs` | 일반 필드 변경을 스트림으로 감지 |
| 기본 확장 타입 | `12_ReadOnlyReactivePropertySample.cs` | 읽기 전용 `ReadOnlyReactiveProperty` 노출 |

## 주의

현재 `01_` 번호가 붙은 파일이 두 개 있습니다. 하나는 문서 01 성격의 비교 예제이고, 하나는 문서 02의 첫 구독 예제입니다.
