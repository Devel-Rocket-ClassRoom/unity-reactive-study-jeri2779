using System;

namespace DIStudy.WhackAMole
{
    /// <summary>현재 라운드 점수를 보관한다.</summary>
    public interface IScoreService
    {
        int CurrentScore { get; }

        event Action<int> ScoreChanged;

        void Add(int amount);

        void Restore(int value);
    }
}
