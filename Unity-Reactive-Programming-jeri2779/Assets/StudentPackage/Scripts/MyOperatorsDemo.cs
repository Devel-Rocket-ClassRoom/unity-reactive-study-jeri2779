using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyOperatorsDemo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_DoubleClickText;

    [SerializeField]
    private TMP_InputField m_SearchInput;

    [SerializeField]
    private TextMeshProUGUI m_SearchResultText;

    [SerializeField]
    private Button m_TapButton;

    [SerializeField]
    private TextMeshProUGUI m_TapText;

    [SerializeField]
    private TextMeshProUGUI m_CooldownText;

         private readonly Subject<Unit> m_ClickSource = new();
    private int m_TapCount;

    private void Start()
    {
        SetupDoubleClick();
        SetupSearchDebounce();
        SetupTapAndCooldown();
    }

   
    [ContextMenu("Simulate Click")]
    public void SimulateClick()
    {
        m_ClickSource.OnNext(Unit.Default);
    }

    // ── 틀: m_ClickSource 를 Chunk/Debounce 로 묶어 더블클릭 판정 ──
    // 참고: ExtensionExamples/01_DoubleClickReactiveExample (Subject + Chunk + Debounce + Where)
    private void SetupDoubleClick()
    {

        var clickStream = Observable
            .EveryUpdate()
            .Where(_ => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            .Share();
        clickStream
            .Chunk(clickStream.Debounce(TimeSpan.FromMilliseconds(500)))
            .Where(clicks => clicks.Length >= 2)
            .Subscribe(clicks => SetText(m_DoubleClickText, $"더블 클릭 {clicks.Length}"))
            .AddTo(this);
     
        // m_ClickSource
        //     // TODO 1) .Chunk(m_ClickSource.Debounce(TimeSpan.FromMilliseconds(250)))  ← 연속 클릭을 침묵 기준으로 묶기
        //     // TODO 2) .Where(clicks => clicks.Length >= 2)                            ← 2회 이상만 더블클릭
        //     .Subscribe(_ => SetText(m_DoubleClickText, "더블클릭!"))
        //     .AddTo(this);
    }

    // ── 틀: 입력창 타이핑이 멈춘 뒤에만 검색 실행(Debounce) ──
    // 참고: ExtensionExamples/11_DebouncedSearchExample (Debounce + Where)
    private void SetupSearchDebounce()
    {
        m_SearchInput
            .onValueChanged.AsObservable()
            .Debounce(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Where(query => !string.IsNullOrWhiteSpace(query))
            .Subscribe(query => SetText(m_SearchResultText, $"검색 결과: {query}"))
            .AddTo(this);
   
    }

    // ── 틀: 버튼 연타해도 쿨다운 동안 1번만 발동(ThrottleFirst) ──
    // 참고: ExtensionExamples/04_SkillCooldownExample (ThrottleFirst)
    private void SetupTapAndCooldown()
    {
        var taps = m_TapButton.OnClickAsObservable().Share();
            
            taps.Chunk(TimeSpan.FromMilliseconds(500), 3)
                .Where(xs => xs.Length >= 3)
                .Subscribe(_ => SetText(m_TapText, $"발동 {++m_TapCount}회"))
                .AddTo(this);

            taps.ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => SetText(m_CooldownText, $"발행"))
                .AddTo(this);
    }

    private void OnDestroy()
    {
        m_ClickSource.Dispose();
    }

    private static void SetText(TextMeshProUGUI label, string text)
    {
        if (label != null)
            label.text = text;
    }
}
