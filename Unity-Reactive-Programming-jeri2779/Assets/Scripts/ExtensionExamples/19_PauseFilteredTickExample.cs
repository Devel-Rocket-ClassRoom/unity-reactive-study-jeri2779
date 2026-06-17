using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class PauseFilteredTickExample : MonoBehaviour
    {
        private readonly ReactiveProperty<bool> isPaused = new ReactiveProperty<bool>(false);
        private int tick;

        private void Start()
        {
            Observable
                .IntervalFrame(30)
                .Where(_ => !isPaused.Value)
                .Subscribe(_ =>
                {
                    tick++;
                    Debug.Log($"[Tick] {tick}");
                })
                .AddTo(this);
        }

        [ContextMenu("Toggle Pause")]
        public void TogglePause()
        {
            isPaused.Value = !isPaused.Value;
            Debug.Log($"[Pause] {isPaused.Value}");
        }

        private void OnDestroy()
        {
            isPaused.Dispose();
        }
    }
}
