using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace DIStudy.WhackAMole
{
    public class MyMole : MonoBehaviour
    {
        [SerializeField] private AudioClip m_HitClip;

        [SerializeField] private float m_RiseHeight = 0.1f;

        [SerializeField] private float m_MoveSpeed = 8f;

        [SerializeField] private float m_HitSinkDepth = 0.6f; // 잡혔을 때 내려가는 깊이(B 연출)

        [SerializeField] private float m_HitDuration = 0.1f; // 잡힘 연출 시간

        [SerializeField] private float m_HiddenDepth = 0.8f; // 쉬는(숨은) 상태에서 구멍보다 더 내려가는 깊이 — 지면 위로 머리가 안 보이게

        private IScoreService m_Score;
        private IAudioService m_Audio;
        private MyGameConfig m_Config;

        private bool m_Resolved;
        private bool m_Finished;
        private bool m_Despawned;

        private CancellationTokenSource m_Cts;

        public event Action<MyMole> Finished;

        public int HoleIndex { get; private set; }

        [Inject]
        public void Construct(IScoreService score, IAudioService audio, MyGameConfig config)
        {
            m_Score = score;
            m_Audio = audio;
            m_Config = config;
            m_Resolved = true;
        }

        public void Activate(int holeIndex, Vector3 downPosition)
        {
            HoleIndex = holeIndex;
            transform.position = downPosition + Vector3.down * m_HiddenDepth; // 숨은 상태에서 시작

            if (!m_Resolved)
            {
                Debug.LogWarning("Mole주입되지 않았습니다");
                return;
            }

            m_Cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            LifeCycle(downPosition, m_Cts.Token).Forget();
        }

        private async UniTaskVoid LifeCycle(Vector3 downPosition, CancellationToken ct)
        {
            Vector3 upPosition = downPosition + Vector3.up * m_RiseHeight;

            await MoveTo(upPosition, ct);

            await UniTask.Delay(
                TimeSpan.FromSeconds(m_Config.MoleUpDuration),
                cancellationToken: ct
            );

            m_Finished = true;
            await MoveTo(downPosition + Vector3.down * m_HiddenDepth, ct); // 지면 아래로 완전히 내려간 뒤
            Despawn();
        }

        private async UniTask MoveTo(Vector3 target, CancellationToken ct)
        {
            while ((transform.position - target).sqrMagnitude > 0.0001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, m_MoveSpeed * Time.deltaTime);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public void Hit()
        {
            if (m_Finished || !m_Resolved) return;

            m_Finished = true;
            m_Cts?.Cancel();  
            m_Score.Add(m_Config.ScorePerHit);
            m_Audio.PlaySoundEffect(m_HitClip);

            HitReaction(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask HitReaction(CancellationToken ct)
        {
            Vector3 start = transform.position;
            Vector3 end = start + Vector3.down * m_HitSinkDepth;
            Vector3 baseScale = transform.localScale;

            float t = 0f;
            while (t < m_HitDuration)
            {
                t += Time.deltaTime;
                float k = t / m_HitDuration;
                transform.position = Vector3.Lerp(start, end, k);
                transform.localScale = Vector3.Lerp(baseScale, baseScale * 0.5f, k);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            Despawn();
        }

        private void Despawn()
        {
            if (m_Despawned)
                return;

            m_Despawned = true;
            Finished?.Invoke(this);  
            Destroy(gameObject);
        }     

        private void OnDestroy()
        {
            m_Cts?.Dispose();
        }
    }
}
