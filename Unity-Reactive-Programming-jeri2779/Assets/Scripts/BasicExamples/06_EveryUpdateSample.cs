using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서 02 §3 팁 - 유니티 전용 팩토리
    //
    // 유니티의 "매 프레임"도 팩토리로 스트림이 된다: Observable.EveryUpdate().
    // 전역 무한 스트림이라 수명 관리가 특히 중요하다(반드시 AddTo 또는 Dispose).
    //
    // 여기서는 연산자 합성까지 살짝 맛본다:
    //  - 매 프레임 중 스페이스가 "눌린 프레임"만 통과시키고(Where)
    //  - 누른 횟수로 변환(Select)해서 로그
    public class EveryUpdateSample : MonoBehaviour
    {
        private int m_PressCount;

        private void Start()
        {
            Observable
                .EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space)) // 눌린 프레임만 통과
                .Select(_ => ++m_PressCount) // 누적 횟수로 변환
                .Subscribe(count => Debug.Log($"[EveryUpdate] Space {count}회"))
                .AddTo(this); // 전역 무한 스트림 → 수명 관리 필수
        }
    }
}
