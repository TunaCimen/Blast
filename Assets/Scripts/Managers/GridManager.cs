using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utility;
using Grid = Utility.Grid;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
    
        [SerializeField]private GameObject mouseTrack;
        [SerializeField] private GameBlockPooler pooler;
        private Grid _grid;
        private bool _isGameStarted;
        private bool _isGridClickable;

        private void Start()
        {
            _isGameStarted = false;
            _isGridClickable = true;
            EventManager.GetInstance.onGameStart += GameStarted;
            EventManager.GetInstance.onGamePaused += Pause;
            EventManager.GetInstance.onResumeGame += Resume;
            EventManager.GetInstance.onResetGrid += ResetGrid;
        }
    
        private void Update()
        {
            if (!_isGameStarted) return;
            TrackMouse();
            if (IsMouseClicked() && _isGridClickable) MouseClicked();
        }
    
        private void TrackMouse()
        {
            var mousePosition2D = GetMousePosition2D();
            mouseTrack.transform.position = mousePosition2D;
        }

        private Vector2 GetMousePosition2D()
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseX = (int) Math.Floor(mousePosition.x);
            var mouseY = (int) Math.Floor(mousePosition.y);
            if (mouseX < 0 || mouseY < 0 || mouseX >= _grid.Width|| mouseY >= _grid.Height)
            {
                mouseX = 10000;
                mouseY = 10000;
            }

            Vector2 mousePosition2D = new Vector2(mouseX, mouseY);
            return mousePosition2D;
        }

        private bool IsMouseClicked()
        {
            return Input.GetMouseButtonDown(0);
        }
        
        private async void MouseClicked()
        {
          
            var mousePosition2D = GetMousePosition2D();
            var x = (int) mousePosition2D.x;
            var y = (int) mousePosition2D.y;
            print("clicked " + x + " " + y);
            //out of bounds
            if (x < 0 || x >= _grid.Width) return;
            if (y < 0 || y >= _grid.Height) return;
            //get the color of the tile
            //blast them blocks and update the board.
            _isGridClickable = false;
            var (c,removedNodes) = await _grid.RemoveIsland(new Vector2(x,y));
            print("Island count is: " + c);
            ScoreManager.GetInstance.SetScore(c);
            _grid.UpdateBoard(removedNodes,()=>_isGridClickable=true);
        }
    
        private void GameStarted(int w, int h, int maxCol)
        {
            if (_grid == null)
            {
                _grid = new Grid(w,h,maxCol,pooler).SetUpGrid();
            }
            else
            {
                _grid.ChangeGrid(w,h,maxCol).SetUpGrid();
            }
            _isGameStarted = true;
        }

        private void Pause()
        {
            _isGameStarted = false;
            mouseTrack.transform.position = new Vector3(1000, 1000);
        }
        private void Resume()
        {
            _isGameStarted = true;
        }

        private void ResetGrid()
        {
            _grid.Reset();
        }
    }
}