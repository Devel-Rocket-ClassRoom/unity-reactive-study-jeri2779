using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace DIStudy.WhackAMole
{
     public class MyGameHudController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ScoreText;

        [SerializeField] private TextMeshProUGUI m_HighScoreText;

        [SerializeField] private TextMeshProUGUI m_TimeText;

        [SerializeField] private TextMeshProUGUI m_StatusText;

        [SerializeField] private Button m_RestartButton;

        private IScoreService m_Score;
        private IHighScoreService m_HighScore;
        private MyGameDirector m_Director;

        [Inject]
        public void Construct(IScoreService score, IHighScoreService highScore, MyGameDirector director)
        {
            m_Score = score;
            m_HighScore = highScore;
            m_Director = director;
        }

        private void Start()
        {
            if (m_Director == null)
            {
                SetStatus("주입 실패");
                return;
            }

            m_Score.ScoreChanged += OnScoreChanged;
            m_HighScore.HighScoreChanged += OnHighScoreChanged;
            m_Director.TimeRemainingChanged += OnTimeChanged;
            m_Director.RoundStarted += OnRoundStarted;
            m_Director.RoundEnded += OnRoundEnded;

            m_RestartButton.onClick.AddListener(m_Director.StartNewRound);
             

       
            OnScoreChanged(m_Score.CurrentScore);
            OnHighScoreChanged(m_HighScore.HighScore);
            OnTimeChanged(m_Director.TimeRemaining);
        }

        private void OnDestroy()
        {
            m_Score.ScoreChanged -= OnScoreChanged;
            m_HighScore.HighScoreChanged -= OnHighScoreChanged;
            m_Director.TimeRemainingChanged -= OnTimeChanged;
            m_Director.RoundStarted -= OnRoundStarted;
            m_Director.RoundEnded -= OnRoundEnded;
            
        }

         
        private void OnScoreChanged(int score)
        {
            m_ScoreText.text = score.ToString();
        }

        private void OnHighScoreChanged(int high)
        {
            m_HighScoreText.text = high.ToString();
        }

        private void OnTimeChanged(float remaining)
        {
            m_TimeText.text = Mathf.CeilToInt(remaining).ToString();
        }

        private void OnRoundStarted()
        {
            SetStatus("");
        }

        private void OnRoundEnded(int finalScore, int highScore, bool isNewRecord)
        {
            SetStatus(isNewRecord ? $"{finalScore}점 — '다시 시작'으로 한 판 더"
                : $"종료 — {finalScore}점 (최고 {highScore}점). '다시 시작'으로 한 판 더");
        }

        private void SetStatus(string message)
        {
            // 상태표시 텍스트는 선택 요소(HUD에 없을 수 있음) — 없으면 그냥 건너뛴다.
            if (m_StatusText != null)
                m_StatusText.text = message;
        }
    }
}
