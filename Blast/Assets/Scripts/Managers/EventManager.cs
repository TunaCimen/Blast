using System;

namespace Managers
{
    public sealed class EventManager
    {

        private static readonly Lazy<EventManager> Instance =  new(() => new EventManager());

        private EventManager(){}
        public static EventManager GetInstance => Instance.Value;
    
        public event Action<int, int,int> onGameStart;

        public void GameStarted(int width, int height, int maxColors)
        {
            onGameStart?.Invoke(width,height,maxColors);
        }
        
        public event Action<int> onScoreChange;

        public void ScoreChanged(int newScore)
        {
            onScoreChange?.Invoke(newScore);
        }

        public event Action onGamePaused;

        public void GamePaused()
        {
            onGamePaused?.Invoke();
        }

        public event Action onResetGrid;

        public void ResetGrid()
        {
            onResetGrid?.Invoke();
        }
        
        public event Action onResumeGame;

        public void ResumeGame()
        {
            onResumeGame?.Invoke();
        }

        public event Action onResetFirstScreenBlocks;

        public void ResetFirstScreenBlocks()
        {
            onResetFirstScreenBlocks?.Invoke();
        }

        public event Action onAnimateFirstScreen;

        public void AnimateFirstScreen()
        {
            onAnimateFirstScreen?.Invoke();
        }
    }
}