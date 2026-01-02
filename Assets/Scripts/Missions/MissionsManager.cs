using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager Instance;

    public List<MissionData> allMissions;
    public Transform missionListParent;
    public GameObject missionUIPrefab;
    public GameObject separatorPrefab;

    [Header("Panel")]
    public RectTransform missionsPanel;
    public CanvasGroup missionsCanvasGroup;

    [Header("Level Progress")]
    public TextMeshProUGUI currentLevelLabel;
    public TextMeshProUGUI nextLevelLabel;
    public TextMeshProUGUI progressFractionLabel;
    public RectTransform levelProgressBarFill;

    [Header("Animation")]
    public float animationDuration = 0.4f;

    [Header("Buttons")]
    public Button settingsButton;
    public Button shopButton;
    public Button powerupsButton;

    [Header("Notification")]
    public GameObject notificationBadge; // LINK YOUR RED CIRCLE HERE

    private Dictionary<string, int> missionProgress = new();
    private HashSet<string> completedMissions = new();
    private HashSet<string> claimedMissions = new();
    private Dictionary<string, int> sessionProgress = new();

    private bool isOpen = false;
    private bool isDirty = false;
    private int currentLevel = 1;
    private int maxLevel = 1;

    private const string LEVEL_KEY = "MissionLevel";
    private const float PROGRESS_BAR_MIN_RIGHT = 507f;
    private const float PROGRESS_BAR_MAX_RIGHT = 5f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (allMissions.Count > 0)
            maxLevel = allMissions.Max(m => m.UnlockLevel);

        LoadProgress();

        // Initial Check
        CheckNotificationBadge();
    }

    private void OnEnable()
    {
        StackManager.OnGameReset += OnGameReset;
    }

    private void OnDisable()
    {
        StackManager.OnGameReset -= OnGameReset;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveToDisk();
        else CheckNotificationBadge(); // Re-check when coming back to app
    }

    private void OnApplicationQuit()
    {
        SaveToDisk();
    }

    private void OnGameReset()
    {
        sessionProgress.Clear();
        MarkDirty();
    }

    private void MarkDirty()
    {
        isDirty = true;
    }

    // --- NOTIFICATION LOGIC ---
    public void CheckNotificationBadge()
    {
        if (notificationBadge == null) return;

        bool anyMissionClaimable = HasAnyClaimableMission();

        // Check Daily Reward too
        bool dailyClaimable = false;
        if (DailyRewardsManager.Instance != null)
        {
            dailyClaimable = DailyRewardsManager.Instance.CanClaimToday();
        }

        notificationBadge.SetActive(anyMissionClaimable || dailyClaimable);
    }

    private bool HasAnyClaimableMission()
    {
        // Check only missions in current level
        foreach (var mission in allMissions)
        {
            if (mission.UnlockLevel == currentLevel)
            {
                bool isCompleted = completedMissions.Contains(mission.ID);
                bool isClaimed = claimedMissions.Contains(mission.ID);

                if (isCompleted && !isClaimed)
                    return true;
            }
        }
        return false;
    }
    // --------------------------

    public void IncrementMission(string missionID, int amount = 1)
    {
        if (claimedMissions.Contains(missionID)) return;

        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null || mission.UnlockLevel != currentLevel) return;

        if (!missionProgress.ContainsKey(missionID))
            missionProgress[missionID] = 0;

        missionProgress[missionID] += amount;
        CheckCompletion(missionID);
        SaveToMemory();
    }

    public void SetMissionProgress(string missionID, int value)
    {
        if (claimedMissions.Contains(missionID)) return;

        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null || mission.UnlockLevel != currentLevel) return;

        if (!missionProgress.ContainsKey(missionID) || value > missionProgress[missionID])
        {
            missionProgress[missionID] = value;
            CheckCompletion(missionID);
            SaveToMemory();
        }
    }

    public void SetSessionProgress(string missionID, int value)
    {
        if (claimedMissions.Contains(missionID)) return;

        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null || mission.UnlockLevel != currentLevel) return;

        sessionProgress[missionID] = value;

        if (mission != null && value >= mission.RequiredValue)
        {
            missionProgress[missionID] = value;
            CheckCompletion(missionID);
            SaveToMemory();
        }
    }

    public void ResetSessionProgress(string missionID)
    {
        if (sessionProgress.ContainsKey(missionID))
            sessionProgress[missionID] = 0;
    }

    public int GetProgress(string missionID)
    {
        int persistent = missionProgress.ContainsKey(missionID) ? missionProgress[missionID] : 0;
        int session = sessionProgress.ContainsKey(missionID) ? sessionProgress[missionID] : 0;
        return Mathf.Max(persistent, session);
    }

    public bool IsMissionCompleted(string missionID) => completedMissions.Contains(missionID);
    public bool IsMissionClaimed(string missionID) => claimedMissions.Contains(missionID);
    public int GetCurrentLevel() => currentLevel;

    public void ClaimMission(string missionID)
    {
        if (!completedMissions.Contains(missionID) || claimedMissions.Contains(missionID))
            return;

        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null) return;

        claimedMissions.Add(missionID);
        CurrencyManager.Instance?.AddCoins(mission.RewardAmount);

        Debug.Log($"Mission claimed: {mission.Description}, rewarded {mission.RewardAmount} coins");

        CheckLevelCompletion();
        SaveToMemory();

        // Update badge (turn off if this was the last one)
        CheckNotificationBadge();

        if (isOpen)
            RefreshUI();
    }

    private void CheckCompletion(string missionID)
    {
        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission != null &&
            missionProgress.ContainsKey(missionID) &&
            missionProgress[missionID] >= mission.RequiredValue &&
            !completedMissions.Contains(missionID))
        {
            completedMissions.Add(missionID);
            Debug.Log($"Mission completed: {mission.Description}");

            // Notification!
            CheckNotificationBadge();
        }
        MarkDirty();
    }

    private void CheckLevelCompletion()
    {
        var currentLevelMissions = allMissions.Where(m => m.UnlockLevel == currentLevel).ToList();
        bool allClaimed = currentLevelMissions.All(m => claimedMissions.Contains(m.ID));

        if (allClaimed && currentLevelMissions.Count > 0 && currentLevel < maxLevel)
        {
            currentLevel++;
            PlayerPrefs.SetInt(LEVEL_KEY, currentLevel);
            Debug.Log($"Level up! Now on mission level {currentLevel}");
            MarkDirty();
        }
    }

    public void RefreshUI()
    {
        isDirty = false;
        UpdateLevelProgressUI();

        foreach (Transform child in missionListParent)
            Destroy(child.gameObject);

        var currentLevelMissions = allMissions
            .Where(m => m.UnlockLevel == currentLevel && !claimedMissions.Contains(m.ID))
            .OrderByDescending(m => completedMissions.Contains(m.ID))
            .ThenBy(m => m.ID)
            .ToList();

        for (int i = 0; i < currentLevelMissions.Count; i++)
        {
            var mission = currentLevelMissions[i];
            int progress = GetProgress(mission.ID);
            bool isCompleted = completedMissions.Contains(mission.ID);
            bool isClaimed = claimedMissions.Contains(mission.ID);

            var go = Instantiate(missionUIPrefab, missionListParent);
            var ui = go.GetComponent<MissionUI>();
            ui.Setup(mission, progress, isCompleted, isClaimed);

            if (separatorPrefab != null && i < currentLevelMissions.Count - 1)
            {
                Instantiate(separatorPrefab, missionListParent);
            }
        }
    }

    private void UpdateLevelProgressUI()
    {
        var currentLevelMissions = allMissions.Where(m => m.UnlockLevel == currentLevel).ToList();
        int totalMissions = currentLevelMissions.Count;
        int claimedCount = currentLevelMissions.Count(m => claimedMissions.Contains(m.ID));

        if (currentLevelLabel != null)
            currentLevelLabel.text = $"Level {currentLevel}";

        if (nextLevelLabel != null)
        {
            if (currentLevel >= maxLevel) nextLevelLabel.text = "MAX";
            else nextLevelLabel.text = $"Level {currentLevel + 1}";
        }

        if (progressFractionLabel != null)
            progressFractionLabel.text = $"{claimedCount}/{totalMissions}";

        if (levelProgressBarFill != null)
        {
            float percent = totalMissions > 0 ? (float)claimedCount / totalMissions : 0f;
            float rightValue = Mathf.Lerp(PROGRESS_BAR_MIN_RIGHT, PROGRESS_BAR_MAX_RIGHT, percent);
            Vector2 offsetMax = levelProgressBarFill.offsetMax;
            offsetMax.x = -rightValue;
            levelProgressBarFill.offsetMax = offsetMax;
        }
    }

    private void SaveToMemory()
    {
        foreach (var mission in allMissions)
        {
            PlayerPrefs.SetInt($"{mission.ID}_Progress", missionProgress.ContainsKey(mission.ID) ? missionProgress[mission.ID] : 0);
            PlayerPrefs.SetInt($"{mission.ID}_Completed", completedMissions.Contains(mission.ID) ? 1 : 0);
            PlayerPrefs.SetInt($"{mission.ID}_Claimed", claimedMissions.Contains(mission.ID) ? 1 : 0);
        }
        PlayerPrefs.SetInt(LEVEL_KEY, currentLevel);
    }

    private void SaveToDisk()
    {
        SaveToMemory();
        PlayerPrefs.Save();
        Debug.Log("Missions saved to disk.");
    }

    private void LoadProgress()
    {
        missionProgress.Clear();
        completedMissions.Clear();
        claimedMissions.Clear();

        currentLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1);

        foreach (var mission in allMissions)
        {
            int progress = PlayerPrefs.GetInt($"{mission.ID}_Progress", 0);
            int completed = PlayerPrefs.GetInt($"{mission.ID}_Completed", 0);
            int claimed = PlayerPrefs.GetInt($"{mission.ID}_Claimed", 0);

            missionProgress[mission.ID] = progress;
            if (completed == 1) completedMissions.Add(mission.ID);
            if (claimed == 1) claimedMissions.Add(mission.ID);
        }

        CheckNotificationBadge(); // Check after loading
        MarkDirty();
    }

    public void ToggleMissions()
    {
        AudioManager.Instance.PlayUISound();

        settingsButton.interactable = !settingsButton.interactable;
        shopButton.interactable = !shopButton.interactable;
        powerupsButton.interactable = !powerupsButton.interactable;

        if (isOpen) CloseMissions();
        else OpenMissions();
    }

    private void OpenMissions()
    {
        isOpen = true;
        if (isDirty) RefreshUI();

        missionsPanel.localScale = Vector3.zero;
        missionsCanvasGroup.alpha = 0f;
        missionsCanvasGroup.interactable = false;
        missionsCanvasGroup.blocksRaycasts = false;

        missionsPanel.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
        missionsCanvasGroup.DOFade(1f, animationDuration).OnComplete(() =>
        {
            missionsCanvasGroup.interactable = true;
            missionsCanvasGroup.blocksRaycasts = true;
        });
    }

    private void CloseMissions()
    {
        isOpen = false;
        missionsCanvasGroup.interactable = false;
        missionsCanvasGroup.blocksRaycasts = false;
        missionsPanel.DOScale(0f, animationDuration).SetEase(Ease.InBack);
        missionsCanvasGroup.DOFade(0f, animationDuration);
    }

    public bool IsMissionsOpen() => isOpen;
}