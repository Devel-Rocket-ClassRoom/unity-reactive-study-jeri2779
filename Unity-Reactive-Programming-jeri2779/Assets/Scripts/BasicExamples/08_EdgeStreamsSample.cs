using System;
using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 타입 ② - 경계(edge) 스트림: Empty / Never / Throw / Defer
    //
    // 연산자 합성·테스트에서 "값이 없는/끝나지 않는/실패하는" 스트림이 종종 필요하다.
    public class EdgeStreamsSample : MonoBehaviour
    {
        private void Start()
        {
            // Empty - 값 없이 즉시 완료.  -|
            Observable
                .Empty<int>()
                .Subscribe(
                    _ => Debug.Log("[Empty] 값(안 옴)"),
                    result => Debug.Log($"[Empty] 즉시 완료: {result}")
                )
                .AddTo(this);

            // Never - 값도 완료도 영원히 없음.  ----->  (타임아웃 테스트 등에 사용)
            Observable
                .Never<int>()
                .Subscribe(_ => Debug.Log("[Never] (절대 안 불림)"))
                .AddTo(this);
            Debug.Log("[Never] 구독했지만 아무 일도 일어나지 않음");

            // Throw - 즉시 에러로 끝나는 스트림.  -X
            Observable
                .Throw<int>(new InvalidOperationException("의도된 에러"))
                .Subscribe(
                    _ => Debug.Log("[Throw] 값(안 옴)"),
                    result => Debug.Log($"[Throw] 완료(실패): {result}")
                )
                .AddTo(this);

            // Defer - 구독되는 "그 순간"에 비로소 스트림을 만든다(지연 생성, 콜드).
            int seed = 0;
            var deferred = Observable.Defer(() =>
            {
                seed++; // 구독할 때마다 새로 평가됨
                return Observable.Return(seed);
            });
            deferred.Subscribe(x => Debug.Log($"[Defer] 1차 구독: {x}")).AddTo(this); // 1
            deferred.Subscribe(x => Debug.Log($"[Defer] 2차 구독: {x}")).AddTo(this); // 2
        }
    }
}
