using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서 02 §5 - ReactiveProperty: 값을 보유하는 스트림
    //
    // 게임 상태(체력·골드·레벨)는 "사건"이 아니라 "현재값이 있는데 가끔 바뀌는 것".
    // Subject와의 차이 3가지
    //  1. 현재값을 보유한다(.Value로 언제든 읽기).
    //  2. 구독 즉시 현재값이 한 번 흐른다(늦게 합류한 UI도 첫 프레임부터 올바른 값).
    //  3. 같은 값이면 통지하지 않는다(기본 중복 억제).
    //
    // 인스펙터에서 초기값을 편집하려면 SerializableReactiveProperty<T>를 쓴다
    // (일반 ReactiveProperty<T>는 [SerializeField] 직렬화 불가).
    public class ReactivePropertyHpSample : MonoBehaviour
    {
        [SerializeField]
        private SerializableReactiveProperty<int> hp = new(100); // 초기값 100

        private void Start()
        {
            // 구독 "즉시" 현재값(100)이 한 번 흐르고, 이후 바뀔 때마다 흐른다.
            hp.Subscribe(current => Debug.Log($"[ReactiveProperty] HP: {current}")).AddTo(this);
        }

        [ContextMenu("Take Damage 10")]
        public void TakeDamage10()
        {
            TakeDamage(10);
        }

        [ContextMenu("Heal 10")]
        public void Heal10()
        {
            hp.Value += 10;
        }

        public void TakeDamage(int amount)
        {
            // .Value에 대입하면 구독자에게 자동 통지(같은 값이면 통지 없음)
            hp.Value = Mathf.Max(0, hp.Value - amount);
        }

        private void OnDestroy()
        {
            hp.Dispose();
        }
    }
}
