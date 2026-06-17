using System;
using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class DebouncedAutoSaveExample : MonoBehaviour
    {
        [SerializeField] private float saveDelaySeconds = 1f;

        private readonly ReactiveProperty<int> coins = new ReactiveProperty<int>(0);
        private readonly Subject<Unit> saveRequested = new Subject<Unit>();

        private void Start()
        {
            coins
                .Skip(1)
                .Subscribe(_ => saveRequested.OnNext(Unit.Default))
                .AddTo(this);

            saveRequested
                .Debounce(TimeSpan.FromSeconds(saveDelaySeconds))
                .Subscribe(_ => Debug.Log($"[Save] coins saved: {coins.Value}"))
                .AddTo(this);
        }

        [ContextMenu("Add Coin")]
        public void AddCoin()
        {
            coins.Value++;
            Debug.Log($"[Coin] {coins.Value}");
        }

        private void OnDestroy()
        {
            coins.Dispose();
            saveRequested.Dispose();
        }
    }
}
