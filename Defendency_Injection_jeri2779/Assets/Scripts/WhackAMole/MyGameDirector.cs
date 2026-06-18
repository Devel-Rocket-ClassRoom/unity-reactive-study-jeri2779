using System;
using UnityEngine;
using VContainer.Unity;

namespace DIStudy.WhackAMole
{
    public sealed class MyGameDirector : IStartable, ITickable, IDisposable
    {
        private readonly IScoreService m_Score;
        private readonly IHighScoreService m_HighScore;
        private readonly MyGameConfig m_Config;

        public bool IsRunning { get; private set; }
        public float TimeRemaining { get; private set; }

        public event Action<float> TimeRemainingChanged;
        public event Action RoundStarted;

        public event Action<int, int, bool> RoundEnded;

        public MyGameDirector(IScoreService score, IHighScoreService highScore, MyGameConfig config)
        {
            m_Score = score;
            m_HighScore = highScore;
            m_Config = config;
        }

        public void Start()
        {
            m_HighScore.Load();
            StartNewRound();
        }

        public void StartNewRound()
        {
            m_Score.Restore(0);
            TimeRemaining = m_Config.RoundDuration;
            IsRunning = true;
            RoundStarted?.Invoke();
            TimeRemainingChanged?.Invoke(TimeRemaining);
        }

        public void Tick()
        {
            if (!IsRunning) return;
                
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining > 0f)
            {
                TimeRemainingChanged?.Invoke(TimeRemaining);
                return;
            }

            TimeRemaining = 0f;
            TimeRemainingChanged?.Invoke(0f);
            EndRound();
        }

        private void EndRound()
        {
            IsRunning = false;
            int finalScore = m_Score.CurrentScore;
            bool isNewRecord = m_HighScore.TrySave(finalScore);
            RoundEnded?.Invoke(finalScore, m_HighScore.HighScore, isNewRecord);
        }

        public void Dispose()
        {
            if (IsRunning)
            {
                m_HighScore.TrySave(m_Score.CurrentScore);
            }
        }
    }
}
