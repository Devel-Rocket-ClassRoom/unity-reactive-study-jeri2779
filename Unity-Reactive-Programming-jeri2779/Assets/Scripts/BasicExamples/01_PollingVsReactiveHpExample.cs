using R3;
using UnityEngine;

namespace BasicExamples
{
    public class PollingVsReactiveHpExample : MonoBehaviour
    {
        [SerializeField] private int hp = 100;

        private readonly Subject<int> hpChanged = new Subject<int>();
        private int lastPolledHp;

        public Observable<int> OnHpChanged => hpChanged;

        private void Start()
        {
            lastPolledHp = hp;

            OnHpChanged
                .Subscribe(currentHp => Debug.Log($"[Reactive] HP changed: {currentHp}"))
                .AddTo(this);
        }

        private void Update()
        {
            if (hp == lastPolledHp)
            {
                return;
            }

            Debug.Log($"[Polling] HP changed: {hp}");
            lastPolledHp = hp;
        }

        [ContextMenu("Take Damage 10")]
        public void TakeDamage10()
        {
            TakeDamage(10);
        }

        public void TakeDamage(int amount)
        {
            hp = Mathf.Max(0, hp - amount);
            hpChanged.OnNext(hp);
        }

        private void OnDestroy()
        {
            hpChanged.Dispose();
        }
    }
}
