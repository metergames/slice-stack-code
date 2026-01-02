using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DailyRewardsManager : MonoBehaviour
{
    public static DailyRewardsManager Instance;

    [Header("Configuration")]
    public int[] rewardValues = { 50, 75, 100, 150, 200, 300, 500 };

    [Header("UI References")]
    public DailyRewardDay[] dayUIItems;
    public Button claimButton;
    public TextMeshProUGUI claimButtonText;

    [Header("Visual Settings")]
    public Color claimedColor = new Color32(100, 100, 100, 255);
    public Color currentColor = new Color32(112, 213, 83, 255);
    public Color lockedColor = new Color32(200, 200, 200, 255);
    public Color disabledButtonColor = new Color32(128, 128, 128, 255);
    public Color activeButtonColor = new Color32(0, 200, 83, 255);

    private const string LAST_LOGIN_KEY = "LastLoginDate";
    private const string STREAK_KEY = "DailyStreak";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Initialize();
        claimButton.onClick.AddListener(OnClaimClicked);

        // NOTIFY: Check badge when game starts
        MissionsManager.Instance?.CheckNotificationBadge();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        CheckStreakLogic();
        UpdateUI();
    }

    private void CheckStreakLogic()
    {
        string lastClaimDate = PlayerPrefs.GetString(LAST_LOGIN_KEY, "");
        if (string.IsNullOrEmpty(lastClaimDate)) return;

        DateTime lastDate = DateTime.Parse(lastClaimDate);
        DateTime today = DateTime.Now;
        double hoursDiff = (today.Date - lastDate.Date).TotalHours;

        if (hoursDiff == 0) return;

        if (hoursDiff >= 48)
        {
            Debug.Log("Streak lost! Resetting to Day 1.");
            PlayerPrefs.SetInt(STREAK_KEY, 0);
        }
    }

    public void UpdateUI()
    {
        int currentStreak = PlayerPrefs.GetInt(STREAK_KEY, 0);
        bool canClaim = CanClaimToday();
        
        // If we can't claim today, the streak already reflects today's claim,
        // so the "current" day is actually the one we already claimed
        int todayIndex = canClaim ? currentStreak % 7 : (currentStreak - 1) % 7;
        if (todayIndex < 0) todayIndex = 0;

        for (int i = 0; i < dayUIItems.Length; i++)
        {
            if (i >= rewardValues.Length) break;
            DailyRewardDay dayItem = dayUIItems[i];
            dayItem.Setup(i, rewardValues[i]);

            if (i < todayIndex) dayItem.SetState(DayState.Claimed, claimedColor);
            else if (i == todayIndex)
            {
                // Today's day - show as claimed if already claimed, otherwise current
                dayItem.SetState(canClaim ? DayState.Current : DayState.Claimed, canClaim ? currentColor : claimedColor);
            }
            else dayItem.SetState(DayState.Locked, lockedColor);
        }

        claimButton.interactable = canClaim;
        claimButton.image.color = canClaim ? activeButtonColor : disabledButtonColor;

        if (claimButtonText != null)
        {
            if (canClaim)
            {
                claimButtonText.text = "CLAIM";
                claimButton.transform.DOScale(1.1f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
            else
            {
                claimButtonText.text = "NEXT\nDAY";
                claimButton.transform.DOKill();
                claimButton.transform.localScale = Vector3.one;
            }
        }
    }

    public bool CanClaimToday()
    {
        string lastClaimDate = PlayerPrefs.GetString(LAST_LOGIN_KEY, "");
        string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
        return lastClaimDate != todayDate;
    }

    private void OnClaimClicked()
    {
        if (!CanClaimToday()) return;

        int currentStreak = PlayerPrefs.GetInt(STREAK_KEY, 0);
        int dayIndex = currentStreak % 7;
        int rewardAmount = rewardValues[dayIndex];

        CurrencyManager.Instance.AddCoins(rewardAmount);
        AudioManager.Instance.PlayUISound();
        StackManager.VibrateClick();

        string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
        PlayerPrefs.SetString(LAST_LOGIN_KEY, todayDate);
        PlayerPrefs.SetInt(STREAK_KEY, currentStreak + 1);

        UpdateUI();

        // NOTIFY: Update badge after claiming
        MissionsManager.Instance?.CheckNotificationBadge();
    }
}