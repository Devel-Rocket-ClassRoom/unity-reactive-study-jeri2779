using R3;
using UnityEngine;
using UnityEngine.Events;

namespace ExtensionExamples
{
    public class UnityEventObservableExample : MonoBehaviour
    {
        [SerializeField] private UnityEvent opened;
        [SerializeField] private UnityEvent<int> valueChanged;

        private int value;

        private void Start()
        {
            opened
                .AsObservable()
                .Subscribe(_ => Debug.Log("[UnityEvent] opened"))
                .AddTo(this);

            valueChanged
                .AsObservable()
                .Subscribe(nextValue => Debug.Log($"[UnityEvent] value: {nextValue}"))
                .AddTo(this);
        }

        [ContextMenu("Open")]
        public void Open()
        {
            opened.Invoke();
        }

        [ContextMenu("Increase Value")]
        public void IncreaseValue()
        {
            value++;
            valueChanged.Invoke(value);
        }
    }
}
