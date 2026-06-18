using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace DIStudy.CoinClicker.Student
{
    public class MyGameHudController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_ScoreText;

        [SerializeField]
        private TextMeshProUGUI m_StatusText;

        [SerializeField]
        private Button m_SaveButton;

        [SerializeField]
        private Button m_ResetButton;

        private IScoreService m_Score;
        private ISaveService m_Save;

        [Inject]
        public void Construct(IScoreService score, ISaveService save)
        {
            m_Score = score;
            m_Save = save;
        }

        private void Start()
        {
            if (m_Score == null)
            {
                SetStatus("<color=#ff5555>주입 실패</color> — LifetimeScope 등록을 확인하세요.");
                return;
            }

            m_Score.ScoreChanged += OnScoreChanged;
            m_Save.Saved += OnSaved;

            if (m_SaveButton != null)
                m_SaveButton.onClick.AddListener(OnSaveClicked);
            if (m_ResetButton != null)
                m_ResetButton.onClick.AddListener(OnResetClicked);

            OnScoreChanged(m_Score.CurrentScore);
            SetStatus($"주입된 서비스: {m_Score.GetType().Name}, {m_Save.GetType().Name}");
        }

        private void OnDestroy()
        {
            if (m_Score != null)
                m_Score.ScoreChanged -= OnScoreChanged;
            if (m_Save != null)
                m_Save.Saved -= OnSaved;
        }

        private void OnScoreChanged(int score)
        {
            if (m_ScoreText != null)
                m_ScoreText.text = $"점수: {score}";
        }

        private void OnSaved(int score)
        {
            SetStatus($"저장됨 — {score}점 (시각 {Time.time:F1}s)");
        }

        private void OnSaveClicked()
        {
            m_Save.SaveScore(m_Score.CurrentScore);
        }

        private void OnResetClicked()
        {
            m_Score.Restore(0);
            SetStatus("점수를 0으로 리셋 (저장은 안 됨 — 저장 버튼 또는 자동 저장)");
        }

        private void SetStatus(string message)
        {
            if (m_StatusText != null)
                m_StatusText.text = message;
        }
    }
}
