using System;
using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 타입 ① - Observable.Create: 가장 근본적인 "직접 만드는" 방식
    //
    // 팩토리(Range/Timer)는 정해진 패턴만 만든다. Create는 OnNext/OnCompleted/OnErrorResume를
    // 내가 원하는 순서·타이밍으로 직접 호출해 임의의 스트림을 정의한다.
    // 반환하는 IDisposable은 "구독이 해제될 때 정리할 작업"(타이머 해제, 리스너 제거 등).
    public class CreateCustomSample : MonoBehaviour
    {
        private void Start()
        {
            Observable
                .Create<int>(observer =>
                {
                    // 구독되는 순간 내가 직접 값을 흘린다
                    observer.OnNext(10);
                    observer.OnNext(20);
                    observer.OnNext(30);
                    observer.OnCompleted(); // 스트림 종료 선언

                    // 구독 해제 시 실행될 정리 작업을 반환(여기선 없음)
                    return Disposable.Empty;
                })
                .Subscribe(
                    x => Debug.Log($"[Create] {x}"),
                    result => Debug.Log($"[Create] 완료: {result}")
                )
                .AddTo(this);
        }
    }
}
