using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class InitScreen : MonoBehaviour
    {

        [SerializeField]private ValueField width;
        [SerializeField]private ValueField height;
        [SerializeField]private ValueField colorCount;
        public void OnClickStart()
        {
            var a = width.GetValue();
            var b = height.GetValue();
            var c = colorCount.GetValue();
            EventManager.GetInstance.GameStarted(a,b,c);
        }

        public void OnRandomStart()
        {
            var w = Random.Range(width.minValue, width.maxValue+1);
            var h = Random.Range(height.minValue, height.maxValue+1);
            var c = Random.Range(colorCount.minValue, colorCount.maxValue+1);
            EventManager.GetInstance.GameStarted(w,h,c);
        }

 
    }
}