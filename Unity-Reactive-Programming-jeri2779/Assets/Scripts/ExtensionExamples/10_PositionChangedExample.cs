using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class PositionChangedExample : MonoBehaviour
    {
        [SerializeField] private float minDistance = 0.25f;

        private void Start()
        {
            Observable
                .EveryUpdate()
                .Select(_ => transform.position)
                .Pairwise()
                .Where(pair => Vector3.Distance(pair.Previous, pair.Current) >= minDistance)
                .Subscribe(pair => Debug.Log($"[Move] {pair.Previous} -> {pair.Current}"))
                .AddTo(this);
        }
    }
}
