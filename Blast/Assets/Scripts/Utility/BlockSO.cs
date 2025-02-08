using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "BlockSO", menuName = "ScriptableObjects/BlockSO", order = 1)]
    public class BlockSO : ScriptableObject
    {

        [SerializeField]private Sprite[] sprites;

        public Sprite[] GetSprites()
        {
            return sprites;
        }
    }
}