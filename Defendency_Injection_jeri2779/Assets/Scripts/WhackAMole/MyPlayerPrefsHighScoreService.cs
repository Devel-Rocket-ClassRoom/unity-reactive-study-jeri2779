using System;
using UnityEngine;

namespace DIStudy.WhackAMole
{
    public sealed class MyPlayerPrefsHighScoreService : IHighScoreService
    {
        private const string HighScoreKey = "DIStudy.WhackAMole.HighScore";

        public int HighScore { get; private set; }

        public event Action<int> HighScoreChanged;

        public int Load()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            HighScoreChanged?.Invoke(HighScore);
            return HighScore;
        }

        public bool TrySave(int score)
        {
            if (score <= HighScore) return false;

            HighScore = score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            HighScoreChanged?.Invoke(HighScore);
            return true;
        }
    }
}
