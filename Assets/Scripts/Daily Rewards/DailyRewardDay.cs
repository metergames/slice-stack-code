using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyRewardDay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dayLabel;    // The text saying "Day 1"
    public TextMeshProUGUI amountLabel; // The text saying "50"
    public Image backgroundImage;       // The box background

    [Header("Configuration")]
    [Tooltip("Index of the day in the week (0-6)")]
    public int dayIndex;                // 0 for Day1, 1 for Day2, etc.

    public void Setup(int dayNum, int rewardAmount)
    {
        dayIndex = dayNum; // 0 to 6
        if (dayLabel != null) dayLabel.text = $"Day {dayNum + 1}";
        if (amountLabel != null) amountLabel.text = $"<size=110%><voffset=-3px><sprite=0 tint=1></voffset></size>{rewardAmount.ToString()}<color=#00000000>-</color>";
    }

    public void SetState(DayState state, Color color)
    {
        if (backgroundImage != null)
            backgroundImage.color = color;

        // Visual logic based on state
        switch (state)
        {
            case DayState.Claimed:
                // Optional: make text darker or transparent
                break;

            case DayState.Current:
                // Optional: Scale animation could go here
                break;

            case DayState.Locked:
                break;
        }
    }
}

public enum DayState
{
    Locked,
    Current,
    Claimed
}