using System;
using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서 02 §6 - FromEvent: 기존 이벤트를 스트림으로
    //
    // 이미 있는 C# event나 서드파티 콜백을 스트림으로 감싸는 어댑터.
    // 좋은 점: 구독을 Dispose하면 -= 가 자동 호출된다(수동 해제 누락 위험 제거).
    // 기존 event 기반 코드를 점진적으로 리액티브에 합류시키는 다리 역할.
    public class FromEventBridgeSample : MonoBehaviour
    {
        // 기존 코드에 이미 있다고 가정하는 레거시 event
        private event Action<int> LegacyScoreEvent;

        private int m_Score;

        private void Start()
        {
            Observable
                .FromEvent<int>(
                    handler => LegacyScoreEvent += handler, // 구독 시작 시 +=
                    handler => LegacyScoreEvent -= handler // 구독 해제(Dispose) 시 -= (자동!)
                )
                .Subscribe(score => Debug.Log($"[FromEvent] 점수: {score}"))
                .AddTo(this); // AddTo가 파괴 시 Dispose → 위 -= 가 자동 실행됨
        }

        // 레거시 event를 평소처럼 발행 → FromEvent로 감싼 스트림이 수신
        [ContextMenu("Raise Legacy Event +10")]
        public void RaiseLegacyEvent()
        {
            m_Score += 10;
            LegacyScoreEvent?.Invoke(m_Score);
        }
    }
}
