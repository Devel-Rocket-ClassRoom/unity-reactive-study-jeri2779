using System.Text;
using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 주제 - 구독 수명 관리: AddTo / CompositeDisposable / ObservableTracker
    //
    // 무한 스트림(Interval 등)은 Dispose하지 않으면 오브젝트가 사라져도 계속 살아 누수가 된다.
    // 두 가지 정리 도구:
    //   - AddTo(this)          : MonoBehaviour 파괴 시 자동 Dispose (가장 흔한 방법)
    //   - CompositeDisposable  : 여러 구독을 한 묶음으로 모아 한 번에 Dispose
    // ObservableTracker는 "지금 살아있는 구독이 몇 개인지" 들여다보는 디버깅 도구다.
    public class ObservableTrackerSample : MonoBehaviour
    {
        private readonly CompositeDisposable m_Disposables = new();

        private void Awake()
        {
            // 추적 켜기 (디버깅 전용 — 빌드에서는 보통 끈다)
            ObservableTracker.EnableTracking = true;
            ObservableTracker.EnableStackTrace = true;
        }

        private void Start()
        {
            Debug.Log("[Tracker] 시작 — 구독 3개를 CompositeDisposable로 묶는다");

            // 1) CompositeDisposable에 모아두기 — m_Disposables.Dispose() 한 번이면 셋 다 정리됨
            Observable
                .Interval(System.TimeSpan.FromSeconds(1))
                .Subscribe(_ => { /* tick A */ })
                .AddTo(m_Disposables);

            Observable
                .Interval(System.TimeSpan.FromSeconds(2))
                .Subscribe(_ => { /* tick B */ })
                .AddTo(m_Disposables);

            Observable.EveryUpdate().Subscribe(_ => { /* per frame */ }).AddTo(m_Disposables);

            DumpActiveCount("3개 구독 직후");

            // 2) 일부러 누수 만들기: AddTo를 안 붙이면 이 오브젝트가 파괴돼도 안 멈춘다(나쁜 예).
            //    아래 줄의 주석을 풀면 추적 개수가 줄지 않는 걸로 누수를 확인할 수 있다.
            // Observable.Interval(System.TimeSpan.FromSeconds(1)).Subscribe(_ => { });
        }

        // 인스펙터 우클릭 → 현재 살아있는 구독 목록을 콘솔에 출력
        [ContextMenu("Dump Active Subscriptions")]
        public void DumpActive()
        {
            DumpActiveCount("수동 확인");
        }

        // 인스펙터 우클릭 → 묶음 전체 해제. 이후 Dump하면 개수가 줄어든다.
        [ContextMenu("Dispose All")]
        public void DisposeAll()
        {
            m_Disposables.Dispose(); // 묶은 구독 전부 한 번에 해제
            Debug.Log("[Tracker] CompositeDisposable.Dispose() 호출 — 묶음 전체 해제");
            DumpActiveCount("Dispose 직후");
        }

        private static void DumpActiveCount(string label)
        {
            var sb = new StringBuilder();
            int count = 0;
            ObservableTracker.ForEachActiveTask(state =>
            {
                count++;
                if (count <= 8)
                    sb.AppendLine($"#{state.TrackingId} {state.FormattedType}");
            });
            Debug.Log($"[Tracker] {label}: 활성 구독 {count}개\n{sb}");
        }

        private void OnDestroy()
        {
            m_Disposables.Dispose();
        }
    }
}
