using System.Collections;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIStudy.CoinClicker.Student
{
    public class MyCoinSpawner : MonoBehaviour
    {
        [SerializeField]
        private MyCoin m_CoinPrefab;

        [SerializeField]
        private Vector3 m_AreaCenter = new Vector3(0f, 1.5f, 0f);

        [SerializeField]
        private Vector3 m_AreaSize = new Vector3(8f, 3f, 2f);

        private IObjectResolver m_Resolver;
        private MyGameConfig m_Config;

        [Inject]
        public void Construct(IObjectResolver resolver, MyGameConfig config)
        {
            m_Config = config;
            m_Resolver = resolver;
        }

        private void Start()
        {
            if (m_CoinPrefab == null)
            {
                Debug.LogWarning("[CoinSpawner] Coin 프리팹이 연결되지 않았습니다.");
                return;
            }

            if (m_Resolver == null)
            {
                Debug.LogWarning("[CoinSpawner] 주입되지 않았습니다 — LifetimeScope 등록을 확인하세요.");
                return;
            }

            for (int i = 0; i < m_Config.CoinCount; i++)
                Spawn();
        }

        private void Spawn()
        {
            Vector3 position =
                m_AreaCenter
                + new Vector3(
                    Random.Range(-m_AreaSize.x, m_AreaSize.x) * 0.5f,
                    Random.Range(-m_AreaSize.y, m_AreaSize.y) * 0.5f,
                    Random.Range(-m_AreaSize.z, m_AreaSize.z) * 0.5f
                );

            MyCoin coin = m_Resolver.Instantiate(m_CoinPrefab, position, m_CoinPrefab.transform.rotation);
            coin.Collected += OnCoinCollected;
        }

        private void OnCoinCollected(MyCoin coin)
        {
            coin.Collected -= OnCoinCollected;
            StartCoroutine(RespawnAfterDelay());
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(m_Config.RespawnDelay);
            Spawn();
        }
    }
}
