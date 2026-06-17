using System;
using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class DebouncedSearchExample : MonoBehaviour
    {
        [SerializeField] private float delaySeconds = 0.4f;

        private readonly Subject<string> searchTextChanged = new Subject<string>();

        private void Start()
        {
            searchTextChanged
                .Debounce(TimeSpan.FromSeconds(delaySeconds))
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Subscribe(text => Debug.Log($"[Search] query: {text}"))
                .AddTo(this);
        }

        [ContextMenu("Type Sword")]
        public void TypeSword()
        {
            searchTextChanged.OnNext("s");
            searchTextChanged.OnNext("sw");
            searchTextChanged.OnNext("swo");
            searchTextChanged.OnNext("sword");
        }

        [ContextMenu("Type Potion")]
        public void TypePotion()
        {
            searchTextChanged.OnNext("p");
            searchTextChanged.OnNext("po");
            searchTextChanged.OnNext("potion");
        }

        private void OnDestroy()
        {
            searchTextChanged.Dispose();
        }
    }
}
