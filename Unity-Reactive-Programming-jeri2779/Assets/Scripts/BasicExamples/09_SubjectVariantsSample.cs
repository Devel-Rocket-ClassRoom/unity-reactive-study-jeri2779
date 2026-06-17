using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 타입 ③ - Subject 변형들: Subject vs BehaviorSubject vs ReplaySubject
    //
    // 문서는 기본 Subject(핫, 늦게 합류하면 지나간 값 못 받음)만 다뤘다.
    // "늦게 구독한 사람"에게 무엇을 보여줄지가 셋의 차이다.
    //
    //  - Subject         : 구독 이후의 값만.            (지나간 값 0개)
    //  - BehaviorSubject : 구독 즉시 "마지막 값 1개"부터. (ReactiveProperty와 사촌)
    //  - ReplaySubject   : 구독 즉시 "지나간 값 전부".    (히스토리 재생)
    public class SubjectVariantsSample : MonoBehaviour
    {
        private readonly Subject<int> m_Subject = new();
        private readonly BehaviorSubject<int> m_Behavior = new(0); // 초기값 필요
        private readonly ReplaySubject<int> m_Replay = new();

        private void Start()
        {
            // 먼저 값들을 흘려보낸 "뒤에" 구독한다 → 차이가 드러난다
            m_Subject.OnNext(1);
            m_Subject.OnNext(2);
            m_Behavior.OnNext(1);
            m_Behavior.OnNext(2);
            m_Replay.OnNext(1);
            m_Replay.OnNext(2);

            // 값이 다 지나간 "뒤늦은" 구독
            m_Subject.Subscribe(x => Debug.Log($"[Subject] {x}")).AddTo(this); // 아무것도 안 옴
            m_Behavior.Subscribe(x => Debug.Log($"[Behavior] {x}")).AddTo(this); // 2 (마지막값)
            m_Replay.Subscribe(x => Debug.Log($"[Replay] {x}")).AddTo(this); // 1, 2 (전부)

            // 구독 이후 새 값은 셋 다 동일하게 받는다
            m_Subject.OnNext(3);
            m_Behavior.OnNext(3);
            m_Replay.OnNext(3);
        }

        private void OnDestroy()
        {
            m_Subject.Dispose();
            m_Behavior.Dispose();
            m_Replay.Dispose();
        }
    }
}
