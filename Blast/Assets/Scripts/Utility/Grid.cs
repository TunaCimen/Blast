using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEditor.UIElements;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utility
{
    public class Grid
    {
        public int Width { get; private set; }
        public int Height { get; set; }
        private int MaxColors { get; set; }
        private int[,] _colorMatrix;
        private int[,] _adjacencyBoard;
        private GameBlock[,] _gameBlocks;
        private readonly int[][] _adjacencyMatrix;
        private GameBlockPooler _pooler;

        public Grid(int width, int height, int maxColors, GameBlockPooler pooler)
        {
            _adjacencyMatrix = new int[4][];
            _adjacencyMatrix[0] = new[] {1, 0};
            _adjacencyMatrix[1] = new[] {-1, 0};
            _adjacencyMatrix[2] = new[] {0, -1};
            _adjacencyMatrix[3] = new[] {0, 1};
            Width = width;
            Height = height;
            MaxColors = maxColors;
            _pooler = pooler;
            _colorMatrix = new int[Width, Height];
            _gameBlocks = new GameBlock[Width, Height];
            _adjacencyBoard = new int[Width, Height];
        }


        public Grid ChangeGrid(int width,int height,int maxColors)
        {
            Width = width;
            Height = height;
            MaxColors = maxColors;
            _colorMatrix = new int[Width, Height];
            _gameBlocks = new GameBlock[Width, Height];
            _adjacencyBoard = new int[Width, Height];
            return this;
        }

        public Grid SetUpGrid()
        {
            RandomSetUp();
            UpdateAdjacencies();
            return this;
        }
        private void RandomSetUp()
        {
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    var pos = new Vector2(i, j);
                    var randomInt = Random.Range(0, MaxColors);
                    var n = _pooler.CullBlock(randomInt, pos);
                    _colorMatrix[i, j] = randomInt;
                    _gameBlocks[i, j] = n;
                }
            }
        }

        private void UpdateAdjacencies()
        {
            CheckAllBoard();
            UpdateBoardIslandCounts();
        }

        private void CheckAllBoard()
        {
            while (true)
            {
                var found = false;
                ResetAdjacenyBoard();
                for (var i = 0; i < Width; i++)
                {
                    for (var j = 0; j < Height; j++)
                    {
                        var f = HaveAdjacent(new Vector2(i, j));
                        if (f) _adjacencyBoard[i, j] = _colorMatrix[i, j];
                        if (!found) found = f;
                    }
                }

                //deadlock
                if (found) return;
                Shuffle();
            }
        }

        private bool HaveAdjacent(Vector2 pos)
        {
            var found = false;
            foreach (var shift in _adjacencyMatrix)
            {
                var shiftedPos = pos + new Vector2(shift[0], shift[1]);
                if (shiftedPos.x < 0 || shiftedPos.y < 0) continue;
                if (shiftedPos.x >= Width || shiftedPos.y >= Height) continue;
                var adjacentTileColor = _colorMatrix[(int) shiftedPos.x, (int) shiftedPos.y];
                var thisTileColor = _colorMatrix[(int) pos.x, (int) pos.y];
                if (adjacentTileColor != thisTileColor) continue;
                found = true;
            }

            return found;
        }

        private void ResetAdjacenyBoard()
        {
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    _adjacencyBoard[i, j] = -1;
                }
            }
        }

        private void Shuffle()
        {
            //Find if exists the swap that assures at least 1 move. O(K). K = color count.
            var counts = new int [MaxColors];
            var cors = new Vector2[MaxColors];
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    counts[_colorMatrix[i, j]]++;
                    if (counts[_colorMatrix[i, j]] > 1)
                    {
                        foreach (var shift in _adjacencyMatrix)
                        {
                            var shifted = cors[_colorMatrix[i, j]] + new Vector2(shift[0], shift[1]);
                            var shiftedX = (int) shifted.x;
                            var shiftedY = (int) shifted.y;
                            if (shiftedX < 0 || shiftedX >= Width) continue;
                            if (shiftedY < 0 || shiftedY >= Height) continue;
                            SwapBlocks(shiftedX, shiftedY, i, j);
                            return;
                        }
                    }

                    cors[_colorMatrix[i, j]] = new Vector2(i, j);
                }
            }
            
            //width*height diff colors on the board. We need to change the color of one block.
            foreach (var shift in _adjacencyMatrix)
            {
                var shifted = new Vector2(1,1) + new Vector2(shift[0], shift[1]);
                var shiftedX = (int) shifted.x;
                var shiftedY = (int) shifted.y;
                if (shiftedX < 0 || shiftedX >= Width) continue;
                if (shiftedY < 0 || shiftedY >= Height) continue;
                _pooler.ReturnBlock(_gameBlocks[shiftedX,shiftedY],_colorMatrix[shiftedX,shiftedY]);
                _gameBlocks[shiftedX,shiftedY] = _pooler.CullBlock(_colorMatrix[1,1], new Vector3(shiftedX, shiftedY));
                _colorMatrix[shiftedX, shiftedY] = _colorMatrix[1,1];
                return;


            }
        }

        public async Task<(int, HashSet<Vector2>)> RemoveIsland(Vector2 mousePos)
        {
            var (count, nodes) = GetIslandCount(mousePos);
            if (count == 1) return (0, null);
            Task lastAnim = null;
            foreach (var n in nodes)
            {
                var i = (int) n.x;
                var j = (int) n.y;
                lastAnim = _gameBlocks[i, j].transform.DOScale(Vector3.zero, 0.3f)
                    .OnComplete(() =>
                    {
                        _pooler.ReturnBlock(_gameBlocks[i, j], _colorMatrix[i, j]);
                        _gameBlocks[i, j].transform.localScale = Vector3.one;
                        _colorMatrix[i, j] = -1;
                    }).AsyncWaitForCompletion();
            }
            if(lastAnim != null)await lastAnim;
            
            return (count, nodes);
        }

        public void UpdateBoard(HashSet<Vector2> removed, Action followUp)
        {
            if (removed == null)
            {
                followUp();
                return;
            }

            var queue = new Queue<Vector2>();
            foreach (var node in removed)
            {
                queue.Enqueue(node);
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var i = (int) node.x;
                var j = (int) node.y;
                for (var k = 0; k < Height - j; k++)
                {
                    if (_colorMatrix[i, j + k] == -1) continue;
                    _colorMatrix[i, j] = _colorMatrix[i, j + k]; //Color set.
                    _colorMatrix[i, j + k] = -1; //Set color to -1 because its empty.
                    _gameBlocks[i, j + k].gameObject.transform.DOMove(new Vector2(i, j), 
                        0.3f).SetEase(Ease.OutBounce);
                    _gameBlocks[i, j] = _gameBlocks[i, j + k]; //GameBlock swap.
                    queue.Enqueue(new Vector2(i, j + k));
                    break;
                }
            }
            DropNewBlocks(()=>
            {
                UpdateAdjacencies();
                followUp();
            });
        }
        
        private void DropNewBlocks(Action onComplete)
        {
            Tween lastAnim = null;
            for (var i = 0; i < Width; i++)
            {
                var counter = 0;
                for (var j = Height - 1; j >= 0; j--)
                {
                    if (_colorMatrix[i, j] == -1)
                    {
                        counter++;
                    }
                }

                for (var k = counter; k > 0; k--)
                {
                    var a = Random.Range(0, MaxColors); //Random generator
                    var g = _pooler.CullBlock(a,
                        new Vector2(i, Height + 2*counter-k)); //Cull the block if we have a existing one.
                    lastAnim = g.gameObject.transform.DOMove(new Vector2(i, Height - k), 
                        .4f).SetEase(Ease.OutBounce);
                    _gameBlocks[i, Height - k] = g;
                    _colorMatrix[i, Height - k] = a;
                }
            }
            
            lastAnim.OnComplete(new TweenCallback(onComplete));
        }

        private void SwapBlocks(int block1X, int block1Y, int i, int j)
        {
            _gameBlocks[block1X, block1Y].gameObject.transform.DOMove(new Vector2(i, j), 0.5f);
            _gameBlocks[i, j].gameObject.transform.DOMove(new Vector2(block1X, block1Y), 0.5f);
            var copy = -1;
            copy = _colorMatrix[i, j];
            _colorMatrix[i, j] = _colorMatrix[block1X, block1Y];
            _colorMatrix[block1X, block1Y] = copy;
            (_gameBlocks[i, j], _gameBlocks[block1X, block1Y]) = (_gameBlocks[block1X, block1Y], _gameBlocks[i, j]);
        }

        private void UpdateBoardIslandCounts()
        {
            var visited = new bool[Width, Height];
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    if (visited[i, j])
                    {
                        continue;
                    }

                    var (count, islandSet) = GetIslandCount(new Vector2(i, j));
                    foreach (var v in islandSet)
                    {
                        visited[(int) v.x, (int) v.y] = true;
                        _gameBlocks[(int) v.x, (int) v.y].ChangeChainCount(count);
                    }
                }
            }
        }

        private (int, HashSet<Vector2>) GetIslandCount(Vector2 pos)
        {
            var color = _colorMatrix[(int) pos.x, (int) pos.y];
            var queue = new Queue<Vector2>();
            var visitedSet = new HashSet<Vector2>();
            var visited = new bool[Width, Height]; //binary encoding for improved effeciency;
            var count = 0;
            queue.Enqueue(pos);
            if (_adjacencyBoard[(int) pos.x, (int) pos.y] == -1)
            {
                visitedSet.Add(new Vector2((int) pos.x, (int) pos.y));
                return (1, visitedSet);
            }

            while (queue.Count > 0)
            {
                var currentPos = queue.Dequeue();
                var indexX = (int) currentPos.x;
                var indexY = (int) currentPos.y;
                if (visited[indexX, indexY])continue;
                if (_adjacencyBoard[indexX, indexY] != color) continue;
                visited[indexX, indexY] = true;
                visitedSet.Add(new Vector2(indexX, indexY));
                count++;
                foreach (var shift in _adjacencyMatrix)
                {
                    var shiftedX = indexX + shift[0];
                    var shiftedY = indexY + shift[1];
                    if (shiftedX < 0 || shiftedX >= Width) continue;
                    if (shiftedY < 0 || shiftedY >= Height) continue;
                    if (visited[shiftedX, shiftedY]) continue;
                    queue.Enqueue(new Vector2(shiftedX, shiftedY));
                }
            }
            return (count, visitedSet);
        }

        public void Reset()
        {
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    _pooler.ReturnBlock(_gameBlocks[i,j],_colorMatrix[i,j]);
                    _colorMatrix[i, j] = -1;
                }
            }
        }
    }
}