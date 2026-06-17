using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 타입 ⑤ - EveryValueChanged: "리액티브가 아닌 값"을 스트림으로
    //
    // ReactiveProperty/Subject로 못 바꾸는 값(서드파티 필드, transform.position 등)을
    // 매 프레임 들여다보다가 "바뀌었을 때만" 밀어준다. 폴링을 R3가 대신 해주는 셈.
    // (내부적으로 매 프레임 비교하므로 남발은 금물 - 정말 reactive화 불가한 값에만.)
    public class EveryValueChangedSample : MonoBehaviour
    {
        // 일부러 평범한 필드(리액티브 아님). 인스펙터/코드 어디서 바꿔도 감지된다.
        [SerializeField]
        private int plainValue;

        private void Start()
        {
            Observable
                .EveryValueChanged(this, self => self.plainValue)
                .Subscribe(v => Debug.Log($"[EveryValueChanged] plainValue 변경 감지: {v}"))
                .AddTo(this);
        }

        [ContextMenu("Increment plainValue")]
        public void Increment()
        {
            plainValue++; // 다음 프레임에 위 구독이 변경을 감지해 로그
        }
    }
}
