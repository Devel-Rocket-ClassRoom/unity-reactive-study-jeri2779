using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class MergeDamageSourcesExample : MonoBehaviour
    {
        private readonly Subject<int> trapDamage = new Subject<int>();
        private readonly Subject<int> enemyDamage = new Subject<int>();
        private readonly Subject<int> poisonDamage = new Subject<int>();

        private int totalDamage;

        private void Start()
        {
            Observable
                .Merge(trapDamage, enemyDamage, poisonDamage)
                .Subscribe(damage =>
                {
                    totalDamage += damage;
                    Debug.Log($"[Damage] +{damage}, total: {totalDamage}");
                })
                .AddTo(this);
        }

        [ContextMenu("Trap Damage")]
        public void TrapDamage()
        {
            trapDamage.OnNext(15);
        }

        [ContextMenu("Enemy Damage")]
        public void EnemyDamage()
        {
            enemyDamage.OnNext(10);
        }

        [ContextMenu("Poison Damage")]
        public void PoisonDamage()
        {
            poisonDamage.OnNext(3);
        }

        private void OnDestroy()
        {
            trapDamage.Dispose();
            enemyDamage.Dispose();
            poisonDamage.Dispose();
        }
    }
}
