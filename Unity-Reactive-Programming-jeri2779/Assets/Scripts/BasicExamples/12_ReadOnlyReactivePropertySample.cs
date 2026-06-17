using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 타입 ⑥ - ReadOnlyReactiveProperty: 읽기 전용으로 노출
    //
    // 문서의 ReactiveProperty는 외부에서 .Value에 대입까지 가능했다.
    // 쓰기는 클래스 내부만, 외부에는 "구독·읽기"만 허용하려면 ReadOnly로 노출한다.
    // (Subject를 Observable로 캡슐화한 것과 같은 발상의 ReactiveProperty 버전.)
    public class ReadOnlyReactivePropertySample : MonoBehaviour
    {
        // 내부용: 읽기/쓰기 모두 가능
        private readonly ReactiveProperty<int> m_Gold = new(0);

        // 외부용: 읽기/구독만 가능(.Value 대입 불가)
        public ReadOnlyReactiveProperty<int> Gold => m_Gold;

        private void Start()
        {
            Gold.Subscribe(g => Debug.Log($"[ReadOnlyRP] Gold: {g}")).AddTo(this);
        }

        [ContextMenu("Earn 100 Gold")]
        public void EarnGold()
        {
            m_Gold.Value += 100; // 쓰기는 내부에서만
        }

        private void OnDestroy()
        {
            m_Gold.Dispose();
        }
    }
}
