# WhackAMole 코드 해설

> Unity 6 + **VContainer 1.18.0** 기반의 두더지 잡기(Whack-A-Mole) 미니게임.
> 의존성 주입(DI)을 활용해 **게임 로직 / 데이터 / 표현(View) / 오디오**를 분리하는 것을 학습하기 위한 예제입니다.
> 네임스페이스는 모두 `DIStudy.WhackAMole` 입니다.

---

## 1. 전체 구조 한눈에 보기

```
[ MyWhackAMoleLifetimeScope ]  ← DI 컨테이너 (composition root)
        │  등록(Register)
        ├── MyGameConfig            (값/설정, 인스펙터에서 입력)
        ├── IScoreService           → MyScoreService                 (현재 점수)
        ├── IHighScoreService       → MyPlayerPrefsHighScoreService  (최고 점수, 영구 저장)
        ├── IAudioService           → MyAudioManager                 (효과음, 씬의 컴포넌트)
        ├── MyMoleSpawner           (두더지 생성, 씬의 컴포넌트)
        ├── MyGameHudController      (UI 표시, 씬의 컴포넌트)
        └── MyGameDirector          (게임 진행 EntryPoint, 순수 C# 클래스)

[ 런타임 흐름 ]
  Director(라운드 시작/타이머/종료)
      └─ RoundStarted 이벤트 → Spawner 가 두더지를 띄움
                                   └─ MyMole(개별 두더지) 등장/대기/퇴장
                                          └─ 클릭 시 Hit() → 점수 + 효과음
      └─ TimeRemaining 0 → RoundEnded → HUD 가 결과 표시
```

**핵심 설계 원칙**
- **MonoBehaviour 가 아닌 순수 로직(`MyGameDirector`, 서비스들)** 은 일반 C# 클래스로 만들고 생성자 주입을 사용한다.
- **씬에 존재해야 하는 객체(`MyMoleSpawner`, `MyGameHudController`, `MyAudioManager`)** 는 `[Inject]` 메서드 주입을 사용한다.
- 데이터(`MyGameConfig`), 표현(HUD/오디오), 진행(Director)을 **인터페이스로 분리**해 서로 직접 알지 못하게 한다.

---

## 2. 파일별 역할 요약

| 파일 | 종류 | 역할 |
|------|------|------|
| `MyWhackAMoleLifetimeScope.cs` | LifetimeScope | DI 등록(composition root) |
| `MyGameConfig.cs` | 데이터 | 라운드 시간·점수 등 설정값 |
| `MyGameDirector.cs` | EntryPoint | 라운드 시작/타이머/종료, 게임 흐름 |
| `IScoreService` / `MyScoreService` | 서비스 | 현재 라운드 점수 |
| `IHighScoreService` / `MyPlayerPrefsHighScoreService` | 서비스 | 최고 점수 영구 저장 |
| `IAudioService` / `MyAudioManager` | 서비스 | 효과음 재생 |
| `MyMoleSpawner.cs` | MonoBehaviour | 빈 구멍에 두더지 생성/관리 |
| `MyMole.cs` | MonoBehaviour | 개별 두더지의 등장·대기·피격·퇴장 |
| `MyMoleClickRouter.cs` | MonoBehaviour | 마우스 클릭 → 두더지 레이캐스트 전달 |
| `MyGameHudController.cs` | MonoBehaviour | 점수/시간/상태 UI 표시 |

---

## 3. DI 컨테이너 — `MyWhackAMoleLifetimeScope`

```csharp
public class MyWhackAMoleLifetimeScope : LifetimeScope
{
    [SerializeField]
    private MyGameConfig m_Config = new MyGameConfig();

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(m_Config);

        builder.Register<IScoreService, MyScoreService>(Lifetime.Singleton);
        builder.Register<IHighScoreService, MyPlayerPrefsHighScoreService>(Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<MyAudioManager>().As<IAudioService>();
        builder.RegisterComponentInHierarchy<MyMoleSpawner>();
        builder.RegisterComponentInHierarchy<MyGameHudController>();

        builder.RegisterEntryPoint<MyGameDirector>().AsSelf();
    }
}
```

**해설**
- `LifetimeScope` 를 상속하면 이 GameObject 가 DI 컨테이너의 뿌리(composition root)가 된다.
- `RegisterInstance(m_Config)` : 인스펙터에서 설정한 **이미 만들어진 인스턴스**를 그대로 등록. 설정값은 한 벌만 공유.
- `Register<인터페이스, 구현>(Lifetime.Singleton)` : 인터페이스로 요청하면 구현체를 주입. **Singleton** 이므로 컨테이너 전체에서 하나만 생성·공유된다.
  - 인터페이스로 등록한 덕분에 `MyScoreService` 를 다른 구현으로 교체해도 사용하는 쪽 코드는 바뀌지 않는다(느슨한 결합).
- `RegisterComponentInHierarchy<T>()` : **씬(하이어라키)에 이미 배치된 컴포넌트**를 찾아 등록.
  - `MyAudioManager` 는 `.As<IAudioService>()` 로 인터페이스 타입으로 노출 → 사용하는 쪽은 `IAudioService` 만 안다.
- `RegisterEntryPoint<MyGameDirector>().AsSelf()` :
  - `MyGameDirector` 가 구현한 VContainer 인터페이스(`IStartable`, `ITickable`, `IDisposable`)를 컨테이너가 생명주기에 맞춰 자동 호출.
  - `.AsSelf()` 를 붙여 `MyGameDirector` **구체 타입으로도** 주입받을 수 있게 한다(HUD/Spawner 가 Director 를 직접 참조).

---

## 4. 데이터 — `MyGameConfig`

```csharp
[Serializable]
public class MyGameConfig
{
    [SerializeField] private float m_RoundDuration;   // 한 라운드 길이(초)
    [SerializeField] private int   m_MaxActiveMoles;  // 동시에 떠 있을 수 있는 두더지 최대 수
    [SerializeField] private float m_MoleUpDuration;  // 두더지가 떠 있는 시간
    [SerializeField] private float m_SpawnInterval;   // 생성 시도 간격(초)
    [SerializeField] private int   m_ScorePerHit;     // 한 번 잡을 때 점수

    public float RoundDuration  => m_RoundDuration;
    // ... 나머지 읽기 전용 프로퍼티
}
```

**해설**
- `[Serializable]` + `[SerializeField]` 로 **인스펙터에서 값 입력**이 가능하고, `LifetimeScope` 가 이를 들고 있다가 `RegisterInstance` 로 주입한다.
- 모든 프로퍼티가 **읽기 전용(get-only)** → 게임 도중 설정이 변하지 않음을 보장(불변 설정값).
- 매직 넘버(예: `5초`, `3마리`)를 코드에 흩뿌리지 않고 한 곳에 모은 **설정 객체** 패턴.

---

## 5. 게임 진행 — `MyGameDirector` (EntryPoint)

```csharp
public sealed class MyGameDirector : IStartable, ITickable, IDisposable
{
    private readonly IScoreService     m_Score;
    private readonly IHighScoreService m_HighScore;
    private readonly MyGameConfig      m_Config;

    public bool  IsRunning     { get; private set; }
    public float TimeRemaining { get; private set; }

    public event Action<float>          TimeRemainingChanged;
    public event Action                 RoundStarted;
    public event Action<int, int, bool> RoundEnded;     // (최종점수, 최고점수, 신기록 여부)

    public MyGameDirector(IScoreService score, IHighScoreService highScore, MyGameConfig config) { ... }
```

**해설 — 생성자 주입**
- MonoBehaviour 가 아니므로 **생성자로 의존성을 받는다**. VContainer 가 등록된 서비스를 자동으로 넣어준다.
- 필드가 모두 `readonly` → 주입 후 교체 불가(안전).

**VContainer 생명주기 인터페이스**
- `IStartable.Start()` : 게임 시작 시 1회 호출 → 최고점수 로드 후 첫 라운드 시작.
- `ITickable.Tick()` : 매 프레임 호출(MonoBehaviour 의 `Update` 대체) → 타이머 감소.
- `IDisposable.Dispose()` : 컨테이너 종료 시 호출 → 진행 중이었다면 점수를 한 번 더 저장(중간 종료 보호).

**흐름**
```
Start() ─→ HighScore.Load() ─→ StartNewRound()
StartNewRound() : 점수 0으로 초기화 → 남은 시간 = RoundDuration → IsRunning=true
                  → RoundStarted 이벤트 발행 (Spawner 가 받아서 필드 리셋)
Tick() : 매 프레임 TimeRemaining -= deltaTime
         → 0 초과면 TimeRemainingChanged 발행 (HUD 갱신)
         → 0 이하면 EndRound()
EndRound() : IsRunning=false → 최고점수 갱신 시도 → RoundEnded 발행 (HUD 결과 표시)
```

> Director 는 **씬·UI·두더지를 직접 모른다.** 오직 `event` 로 "라운드가 시작/종료됐다", "시간이 바뀌었다"만 알리고, 누가 듣는지는 신경 쓰지 않는다 → 관찰자(Observer) 패턴으로 결합도를 낮춤.

---

## 6. 점수 서비스

### `IScoreService` / `MyScoreService` — 현재 라운드 점수

```csharp
public interface IScoreService
{
    int CurrentScore { get; }
    event Action<int> ScoreChanged;
    void Add(int amount);
    void Restore(int value);
}
```

```csharp
public sealed class MyScoreService : IScoreService
{
    public int CurrentScore { get; private set; }
    public event Action<int> ScoreChanged;

    public void Add(int amount)
    {
        if (amount == 0) return;          // 0점은 이벤트 발행 안 함(불필요한 갱신 방지)
        CurrentScore += amount;
        ScoreChanged?.Invoke(CurrentScore);
    }

    public void Restore(int value)        // 라운드 시작 시 0으로 되돌릴 때 사용
    {
        CurrentScore = value;
        ScoreChanged?.Invoke(CurrentScore);
    }
}
```

- 점수가 바뀌면 `ScoreChanged` 이벤트로 알림 → HUD 가 구독해 화면 갱신.
- `Add` 는 두더지(`MyMole`)가, `Restore` 는 Director 가 호출.

### `IHighScoreService` / `MyPlayerPrefsHighScoreService` — 최고 점수(영구 저장)

```csharp
public sealed class MyPlayerPrefsHighScoreService : IHighScoreService
{
    private const string HighScoreKey = "DIStudy.WhackAMole.HighScore";

    public int HighScore { get; private set; }
    public event Action<int> HighScoreChanged;

    public int Load()                     // PlayerPrefs 에서 불러오기
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        HighScoreChanged?.Invoke(HighScore);
        return HighScore;
    }

    public bool TrySave(int score)        // 기존 기록보다 클 때만 저장 → 신기록이면 true
    {
        if (score <= HighScore) return false;
        HighScore = score;
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
        HighScoreChanged?.Invoke(HighScore);
        return true;
    }
}
```

- Unity 의 `PlayerPrefs` 로 게임을 꺼도 유지되는 최고 점수를 관리.
- **인터페이스로 분리**했기 때문에 나중에 서버/파일 저장 방식으로 교체해도 `MyGameDirector` 는 그대로.

---

## 7. 오디오 — `IAudioService` / `MyAudioManager`

```csharp
[RequireComponent(typeof(AudioSource))]
public class MyAudioManager : MonoBehaviour, IAudioService
{
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private Vector2 m_Volume;   // (최소, 최대) 볼륨
    [SerializeField] private Vector2 m_Pitch;    // (최소, 최대) 피치

    public void PlaySoundEffect(AudioClip clip)
    {
        if (m_AudioSource == null || clip == null) return;
        m_AudioSource.pitch = Random.Range(m_Pitch.x, m_Pitch.y);          // 피치 랜덤
        m_AudioSource.PlayOneShot(clip, Random.Range(m_Volume.x, m_Volume.y)); // 볼륨 랜덤
    }
}
```

- 씬에 존재해야 하는 컴포넌트라 `RegisterComponentInHierarchy<MyAudioManager>().As<IAudioService>()` 로 등록.
- 호출하는 쪽(`MyMole`)은 `IAudioService` 만 알고, "효과음을 재생한다"만 요청 → 오디오 구현 세부사항과 분리.
- 피치/볼륨을 **범위 내 랜덤**으로 줘서 같은 효과음이 단조롭지 않게 한다.

---

## 8. 두더지 생성 — `MyMoleSpawner`

```csharp
public class MyMoleSpawner : MonoBehaviour
{
    [SerializeField] private MyMole m_MolePrefab;
    [SerializeField] private Transform[] m_Holes;     // 구멍 위치들

    private IObjectResolver m_Resolver;               // 두더지 프리팹 주입용
    private MyGameConfig    m_Config;
    private MyGameDirector  m_Director;

    private bool[] m_Occupied;     // 각 구멍이 차 있는지
    private int    m_ActiveCount;  // 현재 떠 있는 두더지 수
    private float  m_Timer;
    private int    m_LastHole = -1;// 직전에 쓴 구멍(연속 같은 구멍 방지)

    [Inject]
    public void Construct(IObjectResolver resolver, MyGameConfig config, MyGameDirector director) { ... }
```

**해설 — 메서드 주입**
- 씬에 배치되는 MonoBehaviour 이므로 생성자 대신 `[Inject]` 메서드로 주입.
- **`IObjectResolver` 를 주입받는 이유가 핵심**:

```csharp
private void Spawn()
{
    int hole = PickFreeHole();
    if (hole < 0) return;

    m_Occupied[hole] = true;
    m_ActiveCount++;

    MyMole mole = m_Resolver.Instantiate(m_MolePrefab);  // ★ 일반 Instantiate 가 아님
    mole.Finished += OnMoleFinished;
    mole.Activate(hole, m_Holes[hole].position);
}
```

> `Object.Instantiate` 로 프리팹을 만들면 **DI 주입이 일어나지 않는다.**
> `m_Resolver.Instantiate(prefab)` 를 써야 새로 생긴 두더지 인스턴스의 `[Inject] Construct(...)` 가 호출되어 점수·오디오·설정이 주입된다. 이것이 "런타임 동적 생성 + DI"의 표준 패턴.

**구멍 선택 로직 `PickFreeHole()`**
1. 비어 있고 **직전 구멍이 아닌** 후보를 모은다(같은 자리 연속 방지).
2. 후보가 없으면(전부 직전 구멍뿐) 비어 있는 구멍 전부를 후보로.
3. 그래도 없으면 `-1`(모든 구멍이 참).
4. 후보 중 무작위 선택, 선택값을 `m_LastHole` 에 기록.

**라운드 연동**
- `Start()` 에서 `m_Director.RoundStarted += ResetField` 구독 → 새 라운드마다 점유 상태 초기화.
- `Update()` 는 `m_Director.IsRunning` 일 때만 동작, `SpawnInterval` 간격으로 `MaxActiveMoles` 한도 안에서 생성.
- 두더지가 끝나면(`Finished` 이벤트) `OnMoleFinished` 가 구멍을 다시 비워준다.
- `OnDestroy()` 에서 이벤트 구독 해제 → **메모리 누수/잘못된 콜백 방지**.

---

## 9. 개별 두더지 — `MyMole`

게임에서 가장 복잡한 클래스. **등장 → 대기 → (피격 or 시간초과) → 퇴장/소멸**의 생명주기를 UniTask 비동기로 구현.

```csharp
public class MyMole : MonoBehaviour
{
    [SerializeField] private AudioClip m_HitClip;
    [SerializeField] private float m_RiseHeight = 0.1f;   // 떠오를 높이
    [SerializeField] private float m_MoveSpeed  = 8f;     // 이동 속도
    [SerializeField] private float m_HitSinkDepth = 0.6f; // 잡혔을 때 내려가는 깊이
    [SerializeField] private float m_HitDuration  = 0.1f; // 잡힘 연출 시간
    [SerializeField] private float m_HiddenDepth  = 0.8f; // 숨은 상태에서 지면 아래로 내려가는 깊이

    private IScoreService m_Score;
    private IAudioService m_Audio;
    private MyGameConfig  m_Config;

    private bool m_Resolved;   // 주입 완료 여부
    private bool m_Finished;   // 이미 끝났는지(중복 처리 방지)
    private bool m_Despawned;  // 이미 소멸 처리했는지

    public event Action<MyMole> Finished;   // Spawner 에게 "나 끝남" 알림
    public int HoleIndex { get; private set; }

    [Inject]
    public void Construct(IScoreService score, IAudioService audio, MyGameConfig config)
    {
        m_Score = score; m_Audio = audio; m_Config = config;
        m_Resolved = true;
    }
```

### 9-1. 활성화와 생명주기

```csharp
public void Activate(int holeIndex, Vector3 downPosition)
{
    HoleIndex = holeIndex;
    transform.position = downPosition + Vector3.down * m_HiddenDepth;  // 숨은 위치에서 시작

    if (!m_Resolved) { Debug.LogWarning("Mole주입되지 않았습니다"); return; }

    m_Cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
    LifeCycle(downPosition, m_Cts.Token).Forget();
}

private async UniTaskVoid LifeCycle(Vector3 downPosition, CancellationToken ct)
{
    Vector3 upPosition = downPosition + Vector3.up * m_RiseHeight;

    await MoveTo(upPosition, ct);                                       // 1) 위로 떠오름
    await UniTask.Delay(TimeSpan.FromSeconds(m_Config.MoleUpDuration),  // 2) 떠 있는 시간만큼 대기
                        cancellationToken: ct);
    m_Finished = true;
    await MoveTo(downPosition + Vector3.down * m_HiddenDepth, ct);      // 3) 지면 아래로 내려감
    Despawn();                                                          // 4) 소멸
}
```

- **CancellationToken** 두 개를 연결(link):
  - `GetCancellationTokenOnDestroy()` : 오브젝트가 파괴되면 자동 취소(예외 안전).
  - `m_Cts` : 두더지를 **잡았을 때 수동 취소**용.
- `MoveTo` 는 목표 지점까지 `Vector3.MoveTowards` 로 한 프레임씩 이동하는 코루틴 형태의 UniTask:

```csharp
private async UniTask MoveTo(Vector3 target, CancellationToken ct)
{
    while ((transform.position - target).sqrMagnitude > 0.0001f)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, m_MoveSpeed * Time.deltaTime);
        await UniTask.Yield(PlayerLoopTiming.Update, ct);   // 다음 프레임까지 양보
    }
}
```
  - `sqrMagnitude` 로 거리 비교(제곱근 계산 생략 → 약간의 최적화).

### 9-2. 피격 처리

```csharp
public void Hit()
{
    if (m_Finished || !m_Resolved) return;   // 이미 끝났거나 주입 전이면 무시

    m_Finished = true;
    m_Cts?.Cancel();                         // 진행 중인 LifeCycle(대기/내려감) 취소
    m_Score.Add(m_Config.ScorePerHit);       // 점수 추가
    m_Audio.PlaySoundEffect(m_HitClip);      // 효과음

    HitReaction(this.GetCancellationTokenOnDestroy()).Forget();  // 잡힘 연출 시작
}

private async UniTask HitReaction(CancellationToken ct)
{
    Vector3 start = transform.position;
    Vector3 end   = start + Vector3.down * m_HitSinkDepth;
    Vector3 baseScale = transform.localScale;

    float t = 0f;
    while (t < m_HitDuration)                 // 짧은 시간 동안
    {
        t += Time.deltaTime;
        float k = t / m_HitDuration;
        transform.position   = Vector3.Lerp(start, end, k);                 // 쑥 내려가고
        transform.localScale = Vector3.Lerp(baseScale, baseScale * 0.5f, k);// 작아지는 연출
        await UniTask.Yield(PlayerLoopTiming.Update, ct);
    }
    Despawn();
}
```

- `m_Finished` / `m_Resolved` 가드로 **중복 클릭·미주입 상태**를 막는다.
- 피격 시 기존 `LifeCycle` 을 취소하고 별도의 `HitReaction`(내려가며 축소) 연출로 분기.

### 9-3. 소멸과 정리

```csharp
private void Despawn()
{
    if (m_Despawned) return;     // 한 번만 실행되도록 가드
    m_Despawned = true;
    Finished?.Invoke(this);      // Spawner 에게 알림 → 구멍 비우기
    Destroy(gameObject);
}

private void OnDestroy() => m_Cts?.Dispose();   // 토큰 소스 자원 해제
```

- `Despawn` 은 **시간 초과로 내려간 경우와 피격으로 사라진 경우 양쪽 모두** 도달하는 단일 종료 지점.
- `m_Despawned` 가드로 두 경로가 겹쳐도 `Finished`/`Destroy` 가 두 번 실행되지 않게 한다.

> **상태 플래그 3종(`m_Resolved`/`m_Finished`/`m_Despawned`) 정리**
> | 플래그 | 의미 | 막는 것 |
> |--------|------|---------|
> | `m_Resolved` | 의존성 주입 완료 | 주입 전 동작 |
> | `m_Finished` | 라운드/피격으로 끝남 | 중복 점수·중복 클릭 |
> | `m_Despawned` | 소멸 처리 완료 | 중복 `Destroy`/이벤트 |

---

## 10. 클릭 입력 — `MyMoleClickRouter`

```csharp
public class MyMoleClickRouter : MonoBehaviour
{
    [SerializeField] private LayerMask m_ClickMask = ~0;   // 기본: 모든 레이어

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return; // 좌클릭 순간만
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return; // UI 위 클릭 무시
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_ClickMask)) return;

        MyMole mole = hit.collider.GetComponentInParent<MyMole>();   // 맞은 콜라이더의 부모에서 두더지 찾기
        if (mole != null) mole.Hit();
    }
}
```

**해설**
- **새 Input System**(`Mouse.current`)을 사용. `wasPressedThisFrame` 으로 누른 그 프레임만 처리.
- `IsPointerOverGameObject()` : **UI(버튼 등) 위를 클릭한 경우는 게임 클릭으로 치지 않음** (재시작 버튼 클릭이 두더지 클릭으로 오인되는 것 방지).
- 화면 좌표를 레이로 변환해 두더지 콜라이더를 맞히면 `mole.Hit()` 호출.
- `GetComponentInParent<MyMole>()` : 콜라이더가 자식 오브젝트에 있어도 부모의 `MyMole` 을 찾아낸다.
- 이 클래스는 **DI 와 무관**(서비스 주입 없음). 순수하게 입력 → 두더지 전달만 담당.

---

## 11. UI 표시 — `MyGameHudController`

```csharp
public class MyGameHudController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ScoreText, m_HighScoreText, m_TimeText, m_StatusText;
    [SerializeField] private Button m_RestartButton;

    private IScoreService     m_Score;
    private IHighScoreService m_HighScore;
    private MyGameDirector    m_Director;

    [Inject]
    public void Construct(IScoreService score, IHighScoreService highScore, MyGameDirector director) { ... }
```

**해설 — 이벤트 구독으로 갱신**
- `Start()` 에서 서비스/디렉터의 이벤트를 **구독**하고, 초기값을 한 번 그려 화면을 맞춘다:

```csharp
m_Score.ScoreChanged          += OnScoreChanged;
m_HighScore.HighScoreChanged  += OnHighScoreChanged;
m_Director.TimeRemainingChanged += OnTimeChanged;
m_Director.RoundStarted       += OnRoundStarted;
m_Director.RoundEnded         += OnRoundEnded;
m_RestartButton.onClick.AddListener(m_Director.StartNewRound);   // 재시작 버튼 → 새 라운드
```

- HUD 는 **자신이 값을 계산하지 않는다.** 서비스가 "바뀌었다"고 알리면 그 값을 받아 텍스트만 갱신하는 **수동적 View**.
- `OnTimeChanged` 는 `Mathf.CeilToInt` 로 남은 시간을 올림해 정수 초로 표시.
- `OnRoundEnded` 에서 신기록 여부에 따라 다른 안내 문구 표시.
- `SetStatus` 는 `m_StatusText` 가 **없어도 동작**(null 체크) → 상태 텍스트는 선택 요소.
- `OnDestroy()` 에서 **모든 이벤트 구독 해제** → 파괴된 HUD 가 콜백받아 터지는 일 방지.

> ⚠️ 학습 포인트: `Start()` 의 첫머리에서 `m_Director == null` 이면 "주입 실패"를 표시하고 빠져나오지만, 이후 `OnDestroy()` 는 null 체크 없이 구독 해제를 시도한다. 주입이 정상이라는 전제하에서는 문제없지만, 주입 실패 케이스까지 방어하려면 `OnDestroy` 에도 null 가드를 두는 편이 안전하다.

---

## 12. 이 예제에서 배우는 DI 핵심 정리

1. **두 가지 주입 방식**
   - 순수 C# 클래스(`MyGameDirector`, 서비스) → **생성자 주입**
   - 씬의 MonoBehaviour(`Spawner`, `HUD`, `Mole`, `AudioManager`) → **`[Inject]` 메서드 주입**

2. **인터페이스 분리로 교체 가능성 확보**
   - `IScoreService`, `IHighScoreService`, `IAudioService` → 구현을 바꿔도 사용처 불변.

3. **런타임 동적 생성 + DI**
   - 프리팹 인스턴스에도 의존성을 넣으려면 `IObjectResolver.Instantiate` 를 써야 한다(일반 `Instantiate` ✕).

4. **EntryPoint 생명주기**
   - `IStartable`/`ITickable`/`IDisposable` 로 MonoBehaviour 없이 게임 흐름을 관리.

5. **이벤트(Observer)로 결합도 최소화**
   - Director·서비스는 발행만, HUD·Spawner 는 구독만 → 서로의 존재를 직접 모름.

6. **상태 플래그 + CancellationToken 으로 비동기 안전성**
   - 중복 처리 방지 가드와 토큰 취소로 파괴/중복 시나리오를 견고하게 처리.
