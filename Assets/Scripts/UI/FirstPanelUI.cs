using System.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class FirstPanelUI : MonoBehaviour
    {
        [SerializeField] private Transform[] letters;


        private void Awake()
        {
            EventManager.GetInstance.onAnimateFirstScreen += FallLetters;
            EventManager.GetInstance.onResetFirstScreenBlocks += Reset;

        }

        private void Reset()
        {
            foreach (var l in letters)
            {
                l.localPosition = new Vector3(l.localPosition.x,330);
            }
        }

        private async void FallLetters()
        {
            foreach (var l in letters)
            {
                await Task.Delay(50);
                l.transform.DOLocalMove(new Vector3(l.localPosition.x, 180), Random.Range(0.3f, 0.7f)).SetEase(Ease.OutBounce);
            }
        }
    }
}
