using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace BasicExamples
{
    // 문서에 없는 주제 - 비동기 스트림: SubscribeAwait + AwaitOperation
    //
    // 일반 Subscribe는 OnNext 콜백이 동기로 끝난다고 가정한다.
    // 값 하나마다 "끝나는 데 시간이 걸리는 비동기 작업"(로딩, 네트워크)을 실행하려면
    // SubscribeAwait를 쓰고, 값이 몰려올 때 어떻게 처리할지 AwaitOperation으로 정한다.
    //   - Sequential : 앞 작업이 끝날 때까지 줄 세워 하나씩 처리(요청 손실 없음, 밀림)
    //   - Drop       : 작업 중에 들어온 값은 버림(중복 클릭 무시에 적합)
    //   - Switch     : 새 값이 오면 진행 중 작업을 취소하고 새 것으로 교체(검색 자동완성)
    public class AsyncStreamSample : MonoBehaviour
    {
        private readonly Subject<int> m_Sequential = new();
        private readonly Subject<int> m_Drop = new();
        private int m_SeqId;
        private int m_DropId;

        private void Start()
        {
            // 1) Sequential - 들어온 순서대로 한 번에 하나씩. 빠르게 눌러도 전부 처리되지만 밀린다.
            m_Sequential
                .SubscribeAwait(
                    async (id, ct) =>
                    {
                        Debug.Log($"[Sequential] #{id} 시작");
                        await FakeLoadAsync(ct);
                        Debug.Log($"[Sequential] #{id} 완료");
                    },
                    AwaitOperation.Sequential
                )
                .AddTo(this);

            // 2) Drop - 작업이 진행 중이면 그 사이 들어온 값은 무시(드롭).
            m_Drop
                .SubscribeAwait(
                    async (id, ct) =>
                    {
                        Debug.Log($"[Drop] #{id} 시작");
                        await FakeLoadAsync(ct);
                        Debug.Log($"[Drop] #{id} 완료");
                    },
                    AwaitOperation.Drop
                )
                .AddTo(this);

            // 0.2초 간격으로 5번 빠르게 발행 → 두 방식 차이를 콘솔에서 비교
            FireBurstAsync(destroyCancellationToken).Forget();
        }

        private async UniTask FireBurstAsync(CancellationToken ct)
        {
            for (int i = 0; i < 5; i++)
            {
                m_Sequential.OnNext(++m_SeqId);
                m_Drop.OnNext(++m_DropId);
                await UniTask.Delay(TimeSpan.FromSeconds(0.2), cancellationToken: ct);
            }
            // Sequential: 5개 전부 완료 로그가 차례로 찍힘
            // Drop:       1번 작업 도는 동안 2~5번은 드롭 → 완료 로그가 1~2개만 찍힘
        }

        // 1.5초 걸리는 가짜 비동기 작업(로딩 시뮬레이션)
        private static async UniTask FakeLoadAsync(CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5), cancellationToken: ct);
        }

        private void OnDestroy()
        {
            m_Sequential.Dispose();
            m_Drop.Dispose();
        }
    }
}
