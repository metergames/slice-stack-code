using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI descriptionText;
    public GameObject progressBarBG;
    public RectTransform progressBarFill;
    public Button claimButton;
    public TextMeshProUGUI claimButtonText;

    private MissionData mission;
    private bool isReadyToClaim;

    private const float PROGRESS_BAR_MIN_RIGHT = 642f; // 0% progress
    private const float PROGRESS_BAR_MAX_RIGHT = 5f;   // 100% progress
    private const float DESCRIPTION_HEIGHT_WITH_PROGRESS = 65f;
    private const float DESCRIPTION_HEIGHT_WITHOUT_PROGRESS = 115f;

    // Button colors
    private static readonly Color BUTTON_UNCLAIMABLE_COLOR = new Color32(243, 244, 246, 255);
    private static readonly Color BUTTON_CLAIMABLE_COLOR = new Color32(34, 197, 94, 255);
    private static readonly Color TEXT_UNCLAIMABLE_COLOR = new Color32(55, 65, 81, 255);
    private static readonly Color TEXT_CLAIMABLE_COLOR = Color.white;

    public void Setup(MissionData missionData, int progress, bool isCompleted, bool isClaimed)
    {
        mission = missionData;

        // Don't render claimed missions at all
        if (isClaimed)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // Clamp progress to required value for display
        int displayProgress = Mathf.Min(progress, mission.RequiredValue);

        // Set description text with progress if applicable
        if (mission.HasProgressBar)
            descriptionText.text = $"{mission.Description} ({displayProgress}/{mission.RequiredValue})";
        else
            descriptionText.text = mission.Description;

        // Adjust description text height based on progress bar visibility
        AdjustDescriptionHeight(mission.HasProgressBar);

        // Check if ready to claim
        isReadyToClaim = isCompleted && !isClaimed;

        // Setup claim button
        if (claimButton != null)
        {
            claimButton.interactable = isReadyToClaim;
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(OnClaimClicked);

            // Set button color
            claimButton.image.color = isReadyToClaim ? BUTTON_CLAIMABLE_COLOR : BUTTON_UNCLAIMABLE_COLOR;

            if (claimButtonText != null)
            {
                // Set text color
                claimButtonText.color = isReadyToClaim ? TEXT_CLAIMABLE_COLOR : TEXT_UNCLAIMABLE_COLOR;

                // Set text content
                if (isReadyToClaim)
                    claimButtonText.text = $"<size=115%>Claim</size>\n<size=145%><voffset=-3px><sprite=0></voffset>{mission.RewardAmount}</size><color=#00000000>-</color>";
                else claimButtonText.text = $"<size=145%><color=#629BF7><voffset=-3px><sprite=0 tint=1></voffset></color></size>{mission.RewardAmount}<color=#00000000>-</color>";
            }
        }

        // Progress bar
        progressBarBG.SetActive(mission.HasProgressBar);
        if (mission.HasProgressBar)
            UpdateProgressBar(progress);

        // Visual styling - keep original color, only change for ready to claim
        if (isReadyToClaim)
        {
            if (progressBarFill != null)
            {
                var image = progressBarFill.GetComponent<Image>();
                if (image != null)
                    image.color = Color.green;
            }
        }
    }

    private void AdjustDescriptionHeight(bool hasProgressBar)
    {
        if (descriptionText == null) return;

        RectTransform rt = descriptionText.rectTransform;
        Vector2 sizeDelta = rt.sizeDelta;

        // Adjust height based on whether progress bar is shown
        sizeDelta.y = hasProgressBar ? DESCRIPTION_HEIGHT_WITH_PROGRESS : DESCRIPTION_HEIGHT_WITHOUT_PROGRESS;
        rt.sizeDelta = sizeDelta;
    }

    private void UpdateProgressBar(int progress)
    {
        if (progressBarFill == null) return;

        float percent = Mathf.Clamp01((float)progress / mission.RequiredValue);

        // Interpolate from 680 (0%) to 5 (100%)
        float rightValue = Mathf.Lerp(PROGRESS_BAR_MIN_RIGHT, PROGRESS_BAR_MAX_RIGHT, percent);

        Vector2 offsetMax = progressBarFill.offsetMax;
        offsetMax.x = -rightValue; // Negative because offsetMax.x is the right offset
        progressBarFill.offsetMax = offsetMax;
    }

    private void OnClaimClicked()
    {
        if (mission == null || !isReadyToClaim) return;

        MissionsManager.Instance?.ClaimMission(mission.ID);
        AudioManager.Instance?.PlayUISound();
    }
}
