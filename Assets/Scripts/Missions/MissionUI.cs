using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI rewardText;
    public GameObject progressBarBG;
    public Image progressBarFill;

    public void Setup(MissionData mission, int progress, bool isCompleted)
    {
        if (mission.HasProgressBar)
            descriptionText.text = $"{mission.Description} ({progress}/{mission.RequiredValue})";
        else
            descriptionText.text = mission.Description;

        rewardText.text = isCompleted ? "Completed!" : $"+{mission.RewardAmount} Coins";

        // Progress bar
        progressBarBG.SetActive(mission.HasProgressBar);
        if (mission.HasProgressBar)
        {
            float pct = Mathf.Clamp01((float)progress / mission.RequiredValue);
            progressBarFill.fillAmount = pct;
        }

        if (isCompleted)
        {
            descriptionText.color = Color.gray;
            progressBarFill.color = Color.green;
        }
    }
}
