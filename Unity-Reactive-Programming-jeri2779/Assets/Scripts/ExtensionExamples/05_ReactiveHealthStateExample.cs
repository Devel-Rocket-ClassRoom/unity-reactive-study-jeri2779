using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class ReactiveHealthStateExample : MonoBehaviour
    {
        [SerializeField]
        private SerializableReactiveProperty<int> hp = new SerializableReactiveProperty<int>(100);

        private void Start()
        {
            hp
                .Subscribe(value => Debug.Log($"[Health] HP: {value}"))
                .AddTo(this);

            hp
                .Select(value => value <= 0)
                .DistinctUntilChanged()
                .Where(isDead => isDead)
                .Subscribe(_ => Debug.Log("[Health] dead"))
                .AddTo(this);

            hp
                .Pairwise()
                .Where(pair => pair.Current < pair.Previous)
                .Subscribe(pair => Debug.Log($"[Health] damage: {pair.Previous - pair.Current}"))
                .AddTo(this);
        }

        [ContextMenu("Damage 25")]
        public void Damage25()
        {
            hp.Value = Mathf.Max(0, hp.Value - 25);
        }

        [ContextMenu("Heal 15")]
        public void Heal15()
        {
            hp.Value += 15;
        }

        private void OnDestroy()
        {
            hp.Dispose();
        }
    }
}
