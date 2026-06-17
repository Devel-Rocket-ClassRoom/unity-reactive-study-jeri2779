using System;
using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class ComboInputExample : MonoBehaviour
    {
        [SerializeField] private float comboWindowSeconds = 0.35f;

        private readonly Subject<string> commandStream = new Subject<string>();

        private void Start()
        {
            commandStream
                .Chunk(commandStream.Debounce(TimeSpan.FromSeconds(comboWindowSeconds)))
                .Where(commands => commands.Length >= 3)
                .Subscribe(commands =>
                {
                    string combo = string.Join("-", commands);
                    Debug.Log($"[Combo] input: {combo}");

                    if (combo == "A-B-A")
                    {
                        Debug.Log("[Combo] special attack");
                    }
                })
                .AddTo(this);
        }

        [ContextMenu("Press A")]
        public void PressA()
        {
            commandStream.OnNext("A");
        }

        [ContextMenu("Press B")]
        public void PressB()
        {
            commandStream.OnNext("B");
        }

        [ContextMenu("Demo A-B-A")]
        public void DemoABA()
        {
            PressA();
            PressB();
            PressA();
        }

        private void OnDestroy()
        {
            commandStream.Dispose();
        }
    }
}
