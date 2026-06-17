using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class ReactiveGameStateExample : MonoBehaviour
    {
        private readonly ReactiveProperty<GameState> state = new ReactiveProperty<GameState>(GameState.Ready);

        private void Start()
        {
            state
                .DistinctUntilChanged()
                .Subscribe(next => Debug.Log($"[State] {next}"))
                .AddTo(this);
        }

        [ContextMenu("Start Game")]
        public void StartGame()
        {
            state.Value = GameState.Playing;
        }

        [ContextMenu("Pause Game")]
        public void PauseGame()
        {
            state.Value = GameState.Paused;
        }

        [ContextMenu("Game Over")]
        public void GameOver()
        {
            state.Value = GameState.GameOver;
        }

        private void OnDestroy()
        {
            state.Dispose();
        }

        private enum GameState
        {
            Ready,
            Playing,
            Paused,
            GameOver
        }
    }
}
