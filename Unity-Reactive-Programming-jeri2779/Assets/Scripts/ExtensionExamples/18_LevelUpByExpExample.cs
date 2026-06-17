using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class LevelUpByExpExample : MonoBehaviour
    {
        [SerializeField] private int expPerLevel = 100;

        private readonly ReactiveProperty<int> exp = new ReactiveProperty<int>(0);

        private void Start()
        {
            exp
                .Select(value => value / expPerLevel + 1)
                .DistinctUntilChanged()
                .Pairwise()
                .Where(pair => pair.Current > pair.Previous)
                .Subscribe(pair => Debug.Log($"[Level] {pair.Previous} -> {pair.Current}"))
                .AddTo(this);
        }

        [ContextMenu("Gain EXP 30")]
        public void GainExp30()
        {
            exp.Value += 30;
            Debug.Log($"[EXP] {exp.Value}");
        }

        [ContextMenu("Gain EXP 100")]
        public void GainExp100()
        {
            exp.Value += 100;
            Debug.Log($"[EXP] {exp.Value}");
        }

        private void OnDestroy()
        {
            exp.Dispose();
        }
    }
}
