using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서 02 §4 - Subject: 손으로 미는 발행기
    //
    //  - Subject<T>는 Observer이자 Observable을 겸한다(OnNext로 밀고, Subscribe로 내준다).
    //  - 옵저버 패턴 대응: event 선언 ≈ Subject 필드, Invoke ≈ OnNext.
    //  - 외부에는 Observable<T>로만 노출하는 게 관례(바깥에서 OnNext 못 부르게 캡슐화).
    //  - Subject는 핫(hot) 스트림: 구독 전에 지나간 값은 못 받는다.
    public class SubjectScoreSample : MonoBehaviour
    {
        private readonly Subject<int> m_ScoreSubject = new();

        // 외부 노출은 Observable로만
        public Observable<int> OnScoreChanged => m_ScoreSubject;

        private int m_Score;

        private void Start()
        {
            OnScoreChanged.Subscribe(score => Debug.Log($"[Subject] 점수: {score}")).AddTo(this);
        }

        [ContextMenu("Add Score 10")]
        public void AddScore10()
        {
            AddScore(10);
        }

        public void AddScore(int amount)
        {
            m_Score += amount;
            m_ScoreSubject.OnNext(m_Score); // 알림 발행(event의 Invoke에 해당)
        }

        private void OnDestroy()
        {
            m_ScoreSubject.Dispose(); // 발행기도 IDisposable
        }
    }
}
