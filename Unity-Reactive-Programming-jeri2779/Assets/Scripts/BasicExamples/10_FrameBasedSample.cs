using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 타입 ④ - 프레임 기반 팩토리: TimerFrame / IntervalFrame / NextFrame
    //
    // 문서의 Timer/Interval은 "초(시간) 단위"였다. R3.Unity는 "프레임 단위" 대응물도 준다.
    // 게임 로직은 시간보다 프레임 기준이 자연스러운 경우가 많다(N프레임 뒤, 매 프레임 등).
    public class FrameBasedSample : MonoBehaviour
    {
        private void Start()
        {
            // NextFrame - 딱 다음 프레임에 한 번.  (코루틴 yield return null 의 리액티브 대응)
            Observable
                .NextFrame()
                .Subscribe(_ => Debug.Log("[NextFrame] 다음 프레임 도달"))
                .AddTo(this);

            // TimerFrame - 60프레임 뒤 한 번 발행 후 완료.
            Observable
                .TimerFrame(60)
                .Subscribe(_ => Debug.Log("[TimerFrame] 60프레임 경과"))
                .AddTo(this);

            // IntervalFrame - 30프레임마다 무한 발행(Dispose로만 멈춤).
            Observable
                .IntervalFrame(30)
                .Subscribe(count => Debug.Log($"[IntervalFrame] 30프레임마다: {count}"))
                .AddTo(this);
        }
    }
}
