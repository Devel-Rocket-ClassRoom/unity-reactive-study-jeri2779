using System;

namespace DIStudy.CoinClicker.Student
{
    public sealed class MyScoreService : IScoreService
    {
        public int CurrentScore { get; private set; }

        public event Action<int> ScoreChanged;

        public void Add(int amount)
        {
            if (amount == 0)
                return;

            CurrentScore += amount;
            ScoreChanged?.Invoke(CurrentScore);
        }

        public void Restore(int value)
        {
            CurrentScore = value;
            ScoreChanged?.Invoke(CurrentScore);
        }
    }
}
