using System;

namespace DIStudy.CoinClicker.Student
{
    public interface IScoreService
    {
        int CurrentScore { get; }

        event Action<int> ScoreChanged;

        void Add(int amount);

        void Restore(int value);
    }
}
