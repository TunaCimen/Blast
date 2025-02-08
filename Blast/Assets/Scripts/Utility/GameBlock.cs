using UnityEngine;

namespace Utility
{
    public class GameBlock : MonoBehaviour
    {
    
        [SerializeField] private SpriteRenderer spriteRenderer;
        private BlockSO _blockSo;
        private int ChainCount { get; set; }

        public void Init(BlockSO so)
        {
            _blockSo = so;
        }
        private void Start()
        {
            ChainCount = 0;
        }

        public void ChangeChainCount(int value)
        {
            ChainCount = value;
            if (value < 5) value = 0;
            if (value is >= 5 and < 8) value = 1;
            if (value is >= 8 and < 10) value = 2;
            if (value >= 10) value = 3;
            spriteRenderer.sprite = _blockSo.GetSprites()[value];
        }
    }
}