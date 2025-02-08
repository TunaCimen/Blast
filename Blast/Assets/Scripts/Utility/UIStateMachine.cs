using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Utility
{
    public class UIStateMachine : MonoBehaviour
    {
        [SerializeField]private List<Transform> scenes;//when adding first scene ought to be the index 0!
        private int _currentScene;
        private Action[,] _changeMatrix;
        private int _wildCardIndex;
        private void Start()
        {
            _wildCardIndex = scenes.Count;
            _changeMatrix = new Action[scenes.Count+1,scenes.Count+1]; //+1 s are for destination or source unspecific 
            _changeMatrix[2, 3] = () => EventManager.GetInstance.GamePaused();
            _changeMatrix[3, 2] = () => EventManager.GetInstance.ResumeGame();
            _changeMatrix[3, 0] = () => EventManager.GetInstance.ResetGrid();
            _changeMatrix[0, _wildCardIndex] = () => EventManager.GetInstance.ResetFirstScreenBlocks();
            _changeMatrix[_wildCardIndex, 0] = () => EventManager.GetInstance.AnimateFirstScreen();
            foreach (var t in scenes)
            {
                t.gameObject.SetActive(false);
            }
            _currentScene = 0;
            ChangeState(0);
        }

        public void ChangeState(int scene)
        {
            scenes[_currentScene].gameObject.SetActive(false);
            scenes[scene].gameObject.SetActive(true);
            if(_changeMatrix[_currentScene,scene] != null)_changeMatrix[_currentScene,scene].Invoke();
            if (_changeMatrix[_currentScene, _wildCardIndex] != null) _changeMatrix[_currentScene, _wildCardIndex].Invoke();
            if (_changeMatrix[_wildCardIndex, scene] != null)_changeMatrix[_wildCardIndex,scene].Invoke();
            _currentScene = scene;
        }
    }
}
