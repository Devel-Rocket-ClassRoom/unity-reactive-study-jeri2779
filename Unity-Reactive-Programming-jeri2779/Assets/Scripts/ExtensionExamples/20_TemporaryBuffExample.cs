using System;
using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class TemporaryBuffExample : MonoBehaviour
    {
        [SerializeField] private float buffSeconds = 3f;

        private readonly ReactiveProperty<bool> hasBuff = new ReactiveProperty<bool>(false);

        private void Start()
        {
            hasBuff
                .DistinctUntilChanged()
                .Subscribe(active => Debug.Log($"[Buff] active: {active}"))
                .AddTo(this);
        }

        [ContextMenu("Apply Buff")]
        public void ApplyBuff()
        {
            hasBuff.Value = true;

            Observable
                .Timer(TimeSpan.FromSeconds(buffSeconds))
                .Subscribe(_ => hasBuff.Value = false)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            hasBuff.Dispose();
        }
    }
}
