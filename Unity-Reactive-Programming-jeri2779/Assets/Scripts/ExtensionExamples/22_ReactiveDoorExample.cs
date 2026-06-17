using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class ReactiveDoorExample : MonoBehaviour
    {
        private readonly ReactiveProperty<bool> hasKey = new ReactiveProperty<bool>(false);
        private readonly Subject<Unit> openRequests = new Subject<Unit>();

        private void Start()
        {
            openRequests
                .Where(_ => hasKey.Value)
                .Subscribe(_ => Debug.Log("[Door] opened"))
                .AddTo(this);

            openRequests
                .Where(_ => !hasKey.Value)
                .Subscribe(_ => Debug.Log("[Door] locked"))
                .AddTo(this);
        }

        [ContextMenu("Pick Up Key")]
        public void PickUpKey()
        {
            hasKey.Value = true;
            Debug.Log("[Door] key acquired");
        }

        [ContextMenu("Try Open")]
        public void TryOpen()
        {
            openRequests.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            hasKey.Dispose();
            openRequests.Dispose();
        }
    }
}
