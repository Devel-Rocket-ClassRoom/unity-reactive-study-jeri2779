using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class QuestProgressExample : MonoBehaviour
    {
        [SerializeField] private string targetEnemyId = "Slime";
        [SerializeField] private int requiredKills = 3;

        private readonly Subject<string> enemyKilled = new Subject<string>();
        private readonly ReactiveProperty<int> killCount = new ReactiveProperty<int>(0);

        private void Start()
        {
            enemyKilled
                .Where(enemyId => enemyId == targetEnemyId)
                .Subscribe(_ => killCount.Value++)
                .AddTo(this);

            killCount
                .Subscribe(count => Debug.Log($"[Quest] {targetEnemyId}: {count}/{requiredKills}"))
                .AddTo(this);

            killCount
                .Where(count => count >= requiredKills)
                .Take(1)
                .Subscribe(_ => Debug.Log("[Quest] complete"))
                .AddTo(this);
        }

        [ContextMenu("Kill Slime")]
        public void KillSlime()
        {
            enemyKilled.OnNext("Slime");
        }

        [ContextMenu("Kill Bat")]
        public void KillBat()
        {
            enemyKilled.OnNext("Bat");
        }

        private void OnDestroy()
        {
            enemyKilled.Dispose();
            killCount.Dispose();
        }
    }
}
