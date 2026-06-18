using UnityEngine;

namespace DIStudy.WhackAMole
{
    [RequireComponent(typeof(AudioSource))]
    public class MyAudioManager : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource m_AudioSource;

        [SerializeField] private Vector2 m_Volume = new Vector2();

        [SerializeField] private Vector2 m_Pitch = new Vector2();

        private void Awake()
        {
            if (m_AudioSource == null)
            {
                m_AudioSource = GetComponent<AudioSource>();
            }
        }

        public void PlaySoundEffect(AudioClip clip)
        {
            if (m_AudioSource == null || clip == null) return;
            
            m_AudioSource.pitch = Random.Range(m_Pitch.x, m_Pitch.y);
            m_AudioSource.PlayOneShot(clip, Random.Range(m_Volume.x, m_Volume.y));
        }
    }
}
