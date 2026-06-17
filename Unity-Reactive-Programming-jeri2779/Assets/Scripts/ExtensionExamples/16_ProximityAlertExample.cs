using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class ProximityAlertExample : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float alertDistance = 3f;

        private void Start()
        {
            Observable
                .EveryUpdate()
                .Where(_ => target != null)
                .Select(_ => Vector3.Distance(transform.position, target.position) <= alertDistance)
                .DistinctUntilChanged()
                .Subscribe(isNear => Debug.Log(isNear ? "[Proximity] entered" : "[Proximity] exited"))
                .AddTo(this);
        }
    }
}
