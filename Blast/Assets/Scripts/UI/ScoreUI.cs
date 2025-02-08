using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI scoreField;

        private void Start()
        {
            EventManager.GetInstance.onScoreChange += SetScore;
        }

        private void SetScore(int score)
        {
            scoreField.SetText(score.ToString());
        }
    }
}