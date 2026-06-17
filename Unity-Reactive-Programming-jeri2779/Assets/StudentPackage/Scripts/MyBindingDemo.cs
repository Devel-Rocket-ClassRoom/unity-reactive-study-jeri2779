using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyBindingDemo : MonoBehaviour
{
    [SerializeField]
    private Button m_DamageButton;

    [SerializeField]
    private Button m_HealButton;

    [SerializeField]
    private Button m_UseMpButton;

    [SerializeField]
    private Button m_RestoreMpButton;

    [SerializeField]
    private Button m_SkillButton;

    [SerializeField]
    private TextMeshProUGUI m_HpText;

    [SerializeField]
    private TextMeshProUGUI m_MpText;

    [SerializeField]
    private TextMeshProUGUI m_HpStateText;

    [SerializeField]
    private TextMeshProUGUI m_SkillLogText;

    [SerializeField]
    private SerializableReactiveProperty<int> m_Hp = new(100);

    [SerializeField]
    private SerializableReactiveProperty<int> m_Mp = new(50);

    private const int SkillMpCost = 20;

    private void Start()
    {
        // ── (A) 쓰기: 버튼 클릭 → ReactiveProperty 값 변경 (plumbing, 그대로 사용) ──
        m_DamageButton.OnClickAsObservable().Subscribe(_ => m_Hp.Value -= 10).AddTo(this);
        m_HealButton.OnClickAsObservable().Subscribe(_ => m_Hp.Value += 10).AddTo(this);
        m_UseMpButton.OnClickAsObservable().Subscribe(_ => m_Mp.Value -= 10).AddTo(this);
        m_RestoreMpButton.OnClickAsObservable().Subscribe(_ => m_Mp.Value += 10).AddTo(this);

        // ── (B) 읽기: 값이 바뀌면 자동으로 텍스트 갱신 (plumbing, 그대로 사용) ──
        m_Hp.Subscribe(hp => SetText(m_HpText, $"HP {hp}")).AddTo(this);
        m_Mp.Subscribe(mp => SetText(m_MpText, $"MP {mp}")).AddTo(this);

        // ── (C) 틀: HP 값 → 상태 문자열로 변환해 표시 ──
        // 참고: ExtensionExamples/05_ReactiveHealthStateExample (Select + DistinctUntilChanged)
        m_Hp
            // TODO 1) .Select(hp => hp <= 0 ? "사망" : hp <= 30 ? "위험" : "정상")
            // TODO 2) .DistinctUntilChanged()   ← 상태가 실제로 바뀔 때만
            .Subscribe(_ => SetText(m_HpStateText, "TODO: 상태"))
            .AddTo(this);

        // ── (D) 틀: MP가 스킬 비용 이상일 때만 스킬 버튼 동작 ──
        // 참고: ExtensionExamples/09_CombineLatestCanAttackExample (CombineLatest)
        m_SkillButton
            .OnClickAsObservable()
            // TODO 1) .Where(_ => m_Mp.Value >= SkillMpCost)   ← MP 부족하면 무시
            .Subscribe(_ =>
            {
                // TODO 2) m_Mp.Value -= SkillMpCost; 후 m_SkillLogText 에 사용 로그 남기기
                SetText(m_SkillLogText, "TODO: 스킬 사용");
            })
            .AddTo(this);
    }

    private static void SetText(TextMeshProUGUI label, string text)
    {
        if (label != null)
            label.text = text;
    }

    private void OnDestroy()
    {
        m_Hp.Dispose();
        m_Mp.Dispose();
    }
}
