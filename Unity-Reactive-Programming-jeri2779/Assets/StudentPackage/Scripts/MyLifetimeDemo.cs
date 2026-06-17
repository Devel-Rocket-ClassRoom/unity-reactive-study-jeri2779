using System.Text;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MyLifetimeDemo : MonoBehaviour
{
    [SerializeField]
    private Button m_AddSubscriptionButton;

    [SerializeField]
    private Button m_DisposeAllButton;

    [SerializeField]
    private Button m_LeakButton;

    [SerializeField]
    private Button m_TrackerButton;

    [SerializeField]
    private Button m_DestroyProbeButton;

    [SerializeField]
    private GameObject m_ProbeObject;

    [SerializeField]
    private TextMeshProUGUI m_StatusText;

    [SerializeField]
    private TextMeshProUGUI m_TrackerText;

    private readonly CompositeDisposable m_Disposables = new();
    private int m_AddedCount;
    private int m_LeakedCount;

    private void Awake()
    {
        // 디버깅 도구 — 평소엔 끄는 게 정석. 누수 추적이 이 데모의 목적이라 켠다.
        ObservableTracker.EnableTracking = true;
        ObservableTracker.EnableStackTrace = true; // 구독마다 스택 캡처(무거움). 출처 추적 끝나면 끄기.
    }

    private void Start()
    {
        // ── 틀: 구독 수명 관리 시연 ──
        // 참고: BasicExamples/14_ObservableTrackerSample (CompositeDisposable + ObservableTracker)

        // (A) 구독 추가: 클릭 1번 = 관리되는 구독 1개 생성(묶음 m_Disposables에 등록)
        m_AddSubscriptionButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                int id = ++m_AddedCount;
                Observable
                    .Interval(TimeSpan.FromSeconds(5))
                    .Subscribe(_ => Debug.Log($"LifeTimeDemo : #{id} tick"))
                    .AddTo(m_Disposables); // 묶음에 등록 → 나중에 한 번에 해제 가능
                UpdateStatus($"구독 #{id} 추가");
            })
            .AddTo(this);

        // (B) 전체 해제: 묶음에 등록된 구독을 한 번에 정리(Clear는 묶음 재사용 가능)
        m_DisposeAllButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                m_Disposables.Clear(); // TODO: Dispose() 와의 차이도 알아보기(Dispose 후엔 재등록 불가)
                UpdateStatus("전체 해제");
            })
            .AddTo(this);

        // (C) 누수(나쁜 예): AddTo 없이 구독 → 오브젝트가 사라져도 안 멈춤
        m_LeakButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                int id = ++m_LeakedCount;
                // ★ AddTo / Disposables 등록을 하지 않음 → 아무도 이 구독을 정리할 수 없다(누수).
                //    전체 해제를 눌러도 이 로그는 계속 찍힌다 = "안 멈추는 구독" 체감 포인트.
                Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(_ => Debug.Log($"[누수] #{id} tick — 전체 해제해도 안 멈춤!"));
                UpdateStatus("누수 발생!");
            })
            .AddTo(this);

        // (D) 추적 현황 출력 — ObservableTracker는 "가끔 점검"용. 여기 한 곳에서만 호출한다.
        m_TrackerButton.OnClickAsObservable().Subscribe(_ => DumpTracker()).AddTo(this);

        // (E) 프로브 오브젝트 파괴 → AddTo(this 가 아닌 대상)였던 구독이 함께 정리되는지 관찰
        m_DestroyProbeButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (m_ProbeObject != null)
                {
                    Destroy(m_ProbeObject);
                }
                UpdateStatus("프로브 파괴");
            })
            .AddTo(this);

        UpdateStatus("준비됨");
    }

    // ObservableTracker = 디버깅 점검용(전역). 추적 버튼 누를 때만 호출한다.
    // 주의: 여기 숫자는 "이 데모"가 아니라 "씬 전체" 활성 구독 수다.
    private void DumpTracker()
    {
        var sb = new StringBuilder();
        int count = 0;
        ObservableTracker.ForEachActiveTask(state =>
        {
            count++;
            if (count <= 8)
                sb.AppendLine($"#{state.TrackingId} {state.FormattedType}");
        });
        if (count > 8)
            sb.AppendLine($"... 외 {count - 8}개");
        if (m_TrackerText != null)
            m_TrackerText.text = $"씬 전체 구독 {count}개 (참고용)\n{sb}";
    }

    private void UpdateStatus(string lastAction)
    {
        if (m_StatusText != null)
            m_StatusText.text = $"관리 중 {m_Disposables.Count}개 · 누수 {m_LeakedCount}개 | {lastAction}";
    }

    private void OnDestroy()
    {
        m_Disposables.Dispose();
    }
}
