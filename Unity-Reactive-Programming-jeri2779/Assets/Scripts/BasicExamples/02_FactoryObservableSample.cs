using System;
using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서 02 §3 - 팩토리: 시간·횟수에서 스트림 만들기
    //
    // Range / Timer / Interval / Return 네 가지 대표 팩토리.
    // Timer/Interval은 코루틴 WaitForSeconds/InvokeRepeating의 리액티브 대응물.
    // 차이는 "반환값이 스트림"이라 뒤에 연산자를 이어 붙일 수 있다는 것.
    public class FactoryObservableSample : MonoBehaviour
    {
        private IDisposable m_IntervalSubscription;

        private void Start()
        {
            // 1) Range - 1부터 4개(1,2,3,4) 즉시 발행 후 완료.  --1-2-3-4-|
            Observable
                .Range(1, 4)
                .Subscribe(
                    x => Debug.Log($"[Range] {x}"),
                    result => Debug.Log($"[Range] 완료: {result}")
                )
                .AddTo(this);

            // 2) Timer - 2초 뒤 한 번 발행 후 완료(일회성 지연).  ------o|
            Observable
                .Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ => Debug.Log("[Timer] 2초 경과!"))
                .AddTo(this);

            // 4) Return - 값 하나 발행 후 완료.  o|
            Observable.Return(42).Subscribe(x => Debug.Log($"[Return] {x}")).AddTo(this);

            // 3) Interval - 1초마다 무한 발행. 스스로 끝나지 않으므로 Dispose로만 멈춘다.  --o--o--o-->
            m_IntervalSubscription = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => Debug.Log("[Interval] tick"));
            // 무한 스트림은 AddTo로 묶거나 직접 Dispose로 반드시 정리한다.
        }

        // 인스펙터 우클릭으로 Interval을 수동 정지 → "Dispose가 곧 구독 해제"를 체감
        [ContextMenu("Stop Interval")]
        public void StopInterval()
        {
            m_IntervalSubscription?.Dispose();
            m_IntervalSubscription = null;
            Debug.Log("[Interval] Dispose됨 - tick 멈춤");
        }

        private void OnDestroy()
        {
            m_IntervalSubscription?.Dispose();
        }
    }
}
