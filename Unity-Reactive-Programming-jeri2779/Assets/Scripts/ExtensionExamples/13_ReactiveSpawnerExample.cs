using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class ReactiveSpawnerExample : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int spawnEveryFrames = 60;
        [SerializeField] private int maxSpawnCount = 5;

        private int spawnedCount;

        private void Start()
        {
            Observable
                .IntervalFrame(spawnEveryFrames)
                .Take(maxSpawnCount)
                .Subscribe(_ => Spawn())
                .AddTo(this);
        }

        private void Spawn()
        {
            spawnedCount++;

            if (prefab != null)
            {
                Instantiate(prefab, transform.position, transform.rotation);
            }

            Debug.Log($"[Spawner] spawned {spawnedCount}/{maxSpawnCount}");
        }
    }
}
