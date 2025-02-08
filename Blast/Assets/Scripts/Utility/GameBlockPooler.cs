using UnityEngine;

namespace Utility
{
    public class GameBlockPooler: MonoBehaviour
    {
        [SerializeField] private GameBlock baseBlock;
        [SerializeField] private BlockSO[] blocksSO;
        private Pooler<GameBlock> _pooler;
        
        private void Awake()
        {
            _pooler = new Pooler<GameBlock>(blocksSO.Length);
        }

        public GameBlock CullBlock(int typeIndex, Vector3 position)
        {
            var gameBlock = _pooler.CullObject(typeIndex, position, 
                pos =>
            {
                var g = Instantiate(baseBlock, pos, Quaternion.identity).GetComponent<GameBlock>();
                g.Init(blocksSO[typeIndex]);
          
                return g;
            });
            gameBlock.ChangeChainCount(0);
            gameBlock.transform.position = position;
            return gameBlock;
        }

        public void ReturnBlock(GameBlock g, int typeIndex )
        {
            _pooler.ReturnObject(g,typeIndex,a =>a.transform.position = new Vector2(1000,1000));
        }
    }
}