using System;
using UnityEngine;
using VContainer.Unity;

namespace DIStudy.CoinClicker.Student
{
    public sealed class MyGameDirector : IStartable, ITickable, IDisposable
    {
        private readonly IScoreService m_Score;
        private readonly ISaveService m_Save;
        private readonly MyGameConfig m_Config;

        private float m_Elapsed;

        public MyGameDirector(IScoreService score, ISaveService save, MyGameConfig config)
        {
            m_Score = score;
            m_Save = save;
            m_Config = config;
        }

        public void Start()
        {
            m_Score.Restore(m_Save.LoadScore());
            Debug.Log($"[GameDirector] 시작 — 저장된 점수 {m_Score.CurrentScore}점 복원");
        }

        public void Tick()
        {
            m_Elapsed += Time.deltaTime;
            if (m_Elapsed < m_Config.AutoSaveInterval)
                return;

            m_Elapsed = 0f;
            m_Save.SaveScore(m_Score.CurrentScore);
            Debug.Log($"[GameDirector] 자동 저장 — {m_Score.CurrentScore}점");
        }

        public void Dispose()
        {
            m_Save.SaveScore(m_Score.CurrentScore);
            Debug.Log($"[GameDirector] 스코프 폐기 — 마지막 점수 {m_Score.CurrentScore}점 저장");
            
        }

    }
}
