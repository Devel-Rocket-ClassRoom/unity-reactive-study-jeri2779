using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class LowHpWarningExample : MonoBehaviour
    {
        [SerializeField] private int maxHp = 100;
        [SerializeField] private float warningRatio = 0.3f;

        private readonly ReactiveProperty<int> hp = new ReactiveProperty<int>(100);

        private void Start()
        {
            hp.Value = maxHp;

            hp
                .Select(value => value <= maxHp * warningRatio)
                .DistinctUntilChanged()
                .Where(isLow => isLow)
                .Subscribe(_ => Debug.Log("[HP] low health warning"))
                .AddTo(this);
        }

        [ContextMenu("Damage 20")]
        public void Damage20()
        {
            hp.Value = Mathf.Max(0, hp.Value - 20);
            Debug.Log($"[HP] {hp.Value}/{maxHp}");
        }

        private void OnDestroy()
        {
            hp.Dispose();
        }
    }
}
