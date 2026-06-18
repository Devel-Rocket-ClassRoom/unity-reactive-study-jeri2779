using UnityEngine;

namespace DIStudy.CoinClicker.Student
{
    [RequireComponent(typeof(AudioSource))]
    public class MyAudioManager : MonoBehaviour, IAudioService
    {
        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private Vector2 m_Volume = new Vector2(0.5f, 0.9f);

        [SerializeField]
        private Vector2 m_Pitch = new Vector2(0.8f, 1.2f);

        private void Awake()
        {
            if (m_AudioSource == null)
                m_AudioSource = GetComponent<AudioSource>();
        }

        public void PlaySoundEffect(AudioClip clip)
        {
            if (m_AudioSource == null || clip == null)
                return;

            m_AudioSource.volume = Random.Range(m_Volume.x, m_Volume.y);
            m_AudioSource.pitch = Random.Range(m_Pitch.x, m_Pitch.y);
            m_AudioSource.clip = clip;
            m_AudioSource.Stop();
            m_AudioSource.Play();
        }
    }
}
