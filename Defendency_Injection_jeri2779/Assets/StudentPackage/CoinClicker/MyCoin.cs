using System;
using UnityEngine;
using VContainer;

namespace DIStudy.CoinClicker.Student
{
    public class MyCoin : MonoBehaviour
    {
        [SerializeField]
        private AudioClip m_CollectClip;

        private IScoreService m_Score;
        private IAudioService m_Audio;
        private MyGameConfig m_Config;
        private bool m_Collected;

        public event Action<MyCoin> Collected;

        [Inject]
        public void Construct(IScoreService score, IAudioService audio, MyGameConfig config)
        {
            m_Score = score;
            m_Audio = audio;
            m_Config = config;
        }

        public void Collect()
        {
            if (m_Collected)
                return;

            if (m_Score == null)
            {
                Debug.LogWarning("[Coin] 주입되지 않았습니다 — IObjectResolver.Instantiate로 생성했는지 확인하세요.");
                return;
            }

            m_Collected = true;
            m_Score.Add(m_Config.CoinValue);
            m_Audio.PlaySoundEffect(m_CollectClip);
            Collected?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
