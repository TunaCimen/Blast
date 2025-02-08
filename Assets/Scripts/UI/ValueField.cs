using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ValueField : MonoBehaviour
    {
        [SerializeField]public int maxValue;
        [SerializeField]public int minValue;
        [SerializeField]private TextMeshProUGUI valueText;

        private void Start()
        {
            valueText.SetText(minValue.ToString());
        }

        public void IncrementValue()
        {
            var i = int.Parse(valueText.GetParsedText()) + 1;
            if (i > maxValue) return;
            valueText.SetText((int.Parse(valueText.GetParsedText()) + 1).ToString());
        }

        public void DecrementValue()
        {
            var i = int.Parse(valueText.GetParsedText()) - 1;
            if (i < minValue) return;
            valueText.SetText((int.Parse(valueText.GetParsedText()) - 1).ToString());
        }

        public int GetValue()
        {
            return int.Parse(valueText.GetParsedText());
        }
    }
}