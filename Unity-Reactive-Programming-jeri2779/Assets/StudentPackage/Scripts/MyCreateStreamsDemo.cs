using System;
using System.Collections.Generic;
using R3;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MyCreateStreamsDemo : MonoBehaviour
{
    [SerializeField]
    private Button m_RangeButton;

    [SerializeField]
    private Button m_TimerButton;

    [SerializeField]
    private Button m_IntervalToggleButton;

    [SerializeField]
    private TextMeshProUGUI m_IntervalToggleLabel;

    [SerializeField]
    private Button m_SubjectButton;

    [SerializeField]
    private Button m_EventButton;

    [SerializeField]
    private TextMeshProUGUI m_LogText;

    private readonly Subject<string> m_Subject = new();

    private event Action<int> LegacyScoreEvent;

    private readonly Queue<string> m_LogLines = new();
    private IDisposable m_IntervalSubscription;
    private int m_SubjectCount;
    private int m_LegacyScore;

    private void Start()
    {
        m_RangeButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                Observable
                .Range(1,5)
                .Subscribe
                (
                    x => Log($"Range {x}"),
                    result => Log ($"Result {result}")

                );
                
            })
            .AddTo(this);
        
        m_TimerButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                Log("Timer Start");
                Observable
                    .Timer(TimeSpan.FromSeconds(2))
                    .Subscribe(_ => Debug.Log("Timer Subscribe"))
                    .AddTo(this);
  
            })
        .AddTo(this);

        m_IntervalToggleButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                if(m_IntervalSubscription == null)
                {

                    int tick = 0;
                    m_IntervalSubscription = Observable
                        .Interval(TimeSpan.FromSeconds(2))
                        .Subscribe(_ => Log($"Interval Tick {tick++}"))
                        .AddTo(this);
                    Log("Interval Start");
                    SetToggleLabel("Interval Stop");
                }
                else
                {
                    m_IntervalSubscription.Dispose();
                    m_IntervalSubscription = null;
                    Log("Interval Stop");
                    SetToggleLabel("Interval Start");
                }
            })
        .AddTo(this);

        m_Subject.Subscribe(msg => Log($"Subject Reception{msg}")).AddTo(this);
        m_SubjectButton
            .OnClickAsObservable()
            .Subscribe(_ => m_Subject.OnNext($"msg {m_SubjectCount++}"))
            .AddTo(this);
 
        Observable.FromEvent<int>
        (
            h => LegacyScoreEvent += h,
            h => LegacyScoreEvent -= h
        ).Subscribe(score => Log($"FromEvent 수신: 점수 {score}"))
        .AddTo(this);

        m_EventButton
            .OnClickAsObservable()
            .Subscribe(_ => LegacyScoreEvent?.Invoke(m_LegacyScore += 10))
            .AddTo(this);
 

    }

    private void SetToggleLabel(string text)
    {
        if (m_IntervalToggleLabel != null)
            m_IntervalToggleLabel.text = text;
    }

    private void Log(string message)
    {
        m_LogLines.Enqueue(message);
        while (m_LogLines.Count > 7)
            m_LogLines.Dequeue();
        if (m_LogText != null)
            m_LogText.text = string.Join("\n", m_LogLines);
    }

    private void OnDestroy()
    {
        m_Subject.Dispose();
    }
}
