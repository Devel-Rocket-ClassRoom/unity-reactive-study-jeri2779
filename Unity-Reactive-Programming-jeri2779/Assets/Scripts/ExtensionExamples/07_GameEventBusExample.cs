using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class GameEventBusExample : MonoBehaviour
    {
        private readonly Subject<GameEvent> eventBus = new Subject<GameEvent>();

        private void Start()
        {
            eventBus
                .Where(message => message.Type == GameEventType.CoinCollected)
                .Subscribe(message => Debug.Log($"[EventBus] coin +{message.Amount}"))
                .AddTo(this);

            eventBus
                .Where(message => message.Type == GameEventType.PlayerDamaged)
                .Subscribe(message => Debug.Log($"[EventBus] damage {message.Amount}"))
                .AddTo(this);
        }

        [ContextMenu("Collect Coin")]
        public void CollectCoin()
        {
            eventBus.OnNext(new GameEvent(GameEventType.CoinCollected, 1));
        }

        [ContextMenu("Damage Player")]
        public void DamagePlayer()
        {
            eventBus.OnNext(new GameEvent(GameEventType.PlayerDamaged, 10));
        }

        private void OnDestroy()
        {
            eventBus.Dispose();
        }

        private readonly struct GameEvent
        {
            public GameEvent(GameEventType type, int amount)
            {
                Type = type;
                Amount = amount;
            }

            public GameEventType Type { get; }
            public int Amount { get; }
        }

        private enum GameEventType
        {
            CoinCollected,
            PlayerDamaged
        }
    }
}
