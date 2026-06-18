using System;

namespace DIStudy.WhackAMole
{
    /// <summary>하이스코어를 영구 저장/복원한다.</summary>
    public interface IHighScoreService
    {
        int HighScore { get; }

        event Action<int> HighScoreChanged;

        /// <summary>저장된 하이스코어를 불러와 HighScore에 반영하고 그 값을 반환한다.</summary>
        int Load();

        /// <summary>score가 기존 기록보다 크면 저장하고 true를 반환한다.</summary>
        bool TrySave(int score);
    }
}
