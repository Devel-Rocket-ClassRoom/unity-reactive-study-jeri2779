using System;
using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class DoubleClickReactiveExample : MonoBehaviour
    {
        [SerializeField] private float clickWindowSeconds = 0.25f;

        private readonly Subject<int> clickStream = new Subject<int>();
        private int clickSequence;

        private void Start()
        {
            clickStream
                .Chunk(clickStream.Debounce(TimeSpan.FromSeconds(clickWindowSeconds)))
                .Where(clicks => clicks.Length >= 2)
                .Subscribe(clicks => Debug.Log($"[DoubleClick] detected: {clicks.Length} clicks"))
                .AddTo(this);
        }

        [ContextMenu("Click")]
        public void Click()
        {
            clickStream.OnNext(++clickSequence);
            Debug.Log($"[Click] {clickSequence}");
        }

        private void OnDestroy()
        {
            clickStream.Dispose();
        }
    }
}
