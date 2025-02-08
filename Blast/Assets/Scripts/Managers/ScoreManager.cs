
using System;

namespace Managers
{
    public sealed class ScoreManager
    {
        private int _score;
        private static readonly Lazy<ScoreManager> Instance =  new(() => new ScoreManager());

        private ScoreManager()
        {
            EventManager.GetInstance.onResetGrid += () => SetScore(-_score);
        }
    
        public static ScoreManager GetInstance => Instance.Value;
    
        public int GetScore()
        {
            return _score;
        }

        public void SetScore(int value)
        {
            if (value is >= 5 and < 8) value*=2;
            if (value is >= 8 and < 10) value*=3;
            if (value >= 10) value*=5;
            _score += value;
            EventManager.GetInstance.ScoreChanged(_score);
        }
    }
}