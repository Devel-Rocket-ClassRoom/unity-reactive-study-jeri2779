using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyAsyncDemo : MonoBehaviour
{
    [SerializeField]
    private Button m_SequentialButton;

    [SerializeField]
    private Button m_DropButton;

    [SerializeField]
    private TextMeshProUGUI m_LogText;

    private readonly Queue<string> m_LogLines = new();
    private int m_SequentialCount;
    private int m_DropCount;

    private void Start()
    {
        // ── 틀: 비동기 작업을 SubscribeAwait + AwaitOperation 으로 처리 ──
        // 참고: BasicExamples/13_AsyncStreamSample (SubscribeAwait Sequential / Drop)

        // (A) Sequential: 연타해도 들어온 순서대로 하나씩 끝까지 처리(밀림, 손실 없음)
        m_SequentialButton
            .OnClickAsObservable()
            .SubscribeAwait(
                async (_, ct) =>
                {
                    int id = ++m_SequentialCount;
                    Log($"[Sequential] #{id} 시작");
                    await FakeLoadAsync(ct);
                    Log($"[Sequential] #{id} 완료");
                },
                AwaitOperation.Sequential
            )
            .AddTo(this);

        // (B) Drop: 작업 진행 중 들어온 클릭은 버림(중복 클릭 무시)
        m_DropButton
            .OnClickAsObservable()
            .SubscribeAwait(
                async (_, ct) =>
                {
                    int id = ++m_DropCount;
                    Log($"[Drop] #{id} 시작");
                    await FakeLoadAsync(ct);
                    Log($"[Drop] #{id} 완료");
                    // TODO) AwaitOperation 을 Switch 로 바꿔 동작 차이도 비교해보기
                },
                AwaitOperation.Drop
            )
            .AddTo(this);
    }

    private static async UniTask FakeLoadAsync(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.5), cancellationToken: ct);
    }

    private void Log(string message)
    {
        m_LogLines.Enqueue(message);
        while (m_LogLines.Count > 6)
            m_LogLines.Dequeue();
        if (m_LogText != null)
            m_LogText.text = string.Join("\n", m_LogLines);
    }
}
