using System;
using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서 02 §2 - 첫 구독: Subscribe와 Observer 계약(값/에러/완료 세 채널)
    //
    // 핵심 2가지
    //  - Subscribe는 "등록"이지 "실행"이 아니다. 스트림에 값이 흐를 때 콜백이 불린다.
    //  - Subscribe의 반환값은 IDisposable(= 구독 그 자체). 수명 관리의 핵심.
    public class FirstSubscribeSample : MonoBehaviour
    {
        private void Start()
        {
            // 가장 작은 R3 코드: 1,2,3을 내보내는 콜드 스트림을 구독
            Observable
                .Range(1, 3)
                .Subscribe(x => Debug.Log($"[FirstSubscribe] 받음: {x}"))
                .AddTo(this); // 이 GameObject가 파괴되면 자동 해제

            SubscribeWithThreeChannels();
        }

        // 값(OnNext) · 에러(OnErrorResume) · 완료(OnCompleted) 세 채널을 모두 받는 풀 버전
        private void SubscribeWithThreeChannels()
        {
            Observable
                .Range(1, 3)
                .Subscribe(
                    x => Debug.Log($"[3채널] OnNext: {x}"), // 값
                    ex => Debug.LogError($"[3채널] OnErrorResume: {ex}"), // 에러(R3는 구독을 끊지 않음)
                    result => Debug.Log($"[3채널] OnCompleted: {result}") // 완료(Success/Failure)
                )
                .AddTo(this);
        }
    }
}
