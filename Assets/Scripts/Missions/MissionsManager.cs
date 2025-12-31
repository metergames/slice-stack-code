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

    private Dictionary<string, int> missionProgress = new();
    private HashSet<string> completedMissions = new();
    private HashSet<string> claimedMissions = new();

    // Temporary session-only tracking (resets each run)
    private Dictionary<string, int> sessionProgress = new();

    private bool isOpen = false;
    private bool isDirty = false; // Track if we need to refresh UI
    private int currentLevel = 1;
    private int maxLevel = 1;

    private const string LEVEL_KEY = "MissionLevel";
    private const float PROGRESS_BAR_MIN_RIGHT = 507f; // 0% progress
    private const float PROGRESS_BAR_MAX_RIGHT = 5f;   // 100% progress

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Calculate max level from all missions
        if (allMissions.Count > 0)
            maxLevel = allMissions.Max(m => m.UnlockLevel);

        LoadProgress();
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
        if (pauseStatus)
            SaveToDisk();
    }

    private void OnApplicationQuit()
    {
        SaveToDisk();
    }

    private void OnGameReset()
    {
        // Reset session-only progress (streaks, single-run goals)
        sessionProgress.Clear();
        MarkDirty();
    }

    /// <summary>
    /// Marks the UI as needing a refresh. Actual refresh happens when menu opens.
    /// </summary>
    private void MarkDirty()
    {
        isDirty = true;
    }

    /// <summary>
    /// Increment progress by a value (for cumulative missions like "collect 100 coins total")
    /// </summary>
    public void IncrementMission(string missionID, int amount = 1)
    {
        if (claimedMissions.Contains(missionID)) return;

        // Only track missions for current level
        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null || mission.UnlockLevel != currentLevel) return;

        if (!missionProgress.ContainsKey(missionID))
            missionProgress[missionID] = 0;

        missionProgress[missionID] += amount;
        CheckCompletion(missionID);
        SaveToMemory();
    }

    /// <summary>
    /// Set progress to a specific value (for "reach score X" type missions)
    /// </summary>
    public void SetMissionProgress(string missionID, int value)
    {
        if (claimedMissions.Contains(missionID)) return;

        // Only track missions for current level
        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null || mission.UnlockLevel != currentLevel) return;

        // Only update if new value is higher (for high-score style missions)
        if (!missionProgress.ContainsKey(missionID) || value > missionProgress[missionID])
        {
            missionProgress[missionID] = value;
            CheckCompletion(missionID);
            SaveToMemory();
        }
    }

    /// <summary>
    /// Track session-only progress (resets each run). Good for streaks.
    /// </summary>
    public void SetSessionProgress(string missionID, int value)
    {
        if (claimedMissions.Contains(missionID)) return;

        // Only track missions for current level
        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null || mission.UnlockLevel != currentLevel) return;

        sessionProgress[missionID] = value;

        // If session progress meets requirement, mark as complete
        if (mission != null && value >= mission.RequiredValue)
        {
            missionProgress[missionID] = value; // Persist the completion
            CheckCompletion(missionID);
            SaveToMemory();
        }
    }

    /// <summary>
    /// Reset session progress for a specific mission (e.g., streak broken)
    /// </summary>
    public void ResetSessionProgress(string missionID)
    {
        if (sessionProgress.ContainsKey(missionID))
            sessionProgress[missionID] = 0;
    }

    /// <summary>
    /// Get current progress for a mission (combines persistent + session)
    /// </summary>
    public int GetProgress(string missionID)
    {
        int persistent = missionProgress.ContainsKey(missionID) ? missionProgress[missionID] : 0;
        int session = sessionProgress.ContainsKey(missionID) ? sessionProgress[missionID] : 0;
        return Mathf.Max(persistent, session);
    }

    public bool IsMissionCompleted(string missionID)
    {
        return completedMissions.Contains(missionID);
    }

    public bool IsMissionClaimed(string missionID)
    {
        return claimedMissions.Contains(missionID);
    }

    public int GetCurrentLevel() => currentLevel;

    /// <summary>
    /// Claim a completed mission and receive rewards
    /// </summary>
    public void ClaimMission(string missionID)
    {
        if (!completedMissions.Contains(missionID) || claimedMissions.Contains(missionID))
            return;

        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission == null) return;

        claimedMissions.Add(missionID);
        CurrencyManager.Instance?.AddCoins(mission.RewardAmount);

        Debug.Log($"Mission claimed: {mission.Description}, rewarded {mission.RewardAmount} coins");

        // Check if all missions for current level are claimed
        CheckLevelCompletion();

        SaveToMemory();
        
        // Refresh UI immediately since we're in the menu
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
        }
        MarkDirty();
    }

    private void CheckLevelCompletion()
    {
        var currentLevelMissions = allMissions.Where(m => m.UnlockLevel == currentLevel).ToList();

        // Check if all missions for current level are claimed
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

        // Update level progress UI
        UpdateLevelProgressUI();

        // Clear old rows
        foreach (Transform child in missionListParent)
            Destroy(child.gameObject);

        // Get missions for current level only
        var currentLevelMissions = allMissions
            .Where(m => m.UnlockLevel == currentLevel && !claimedMissions.Contains(m.ID))
            .OrderByDescending(m => completedMissions.Contains(m.ID)) // Ready to claim first
            .ThenBy(m => m.ID) // Consistent ordering
            .ToList();

        // Create new rows with separators between them
        for (int i = 0; i < currentLevelMissions.Count; i++)
        {
            var mission = currentLevelMissions[i];
            int progress = GetProgress(mission.ID);
            bool isCompleted = completedMissions.Contains(mission.ID);
            bool isClaimed = claimedMissions.Contains(mission.ID);

            var go = Instantiate(missionUIPrefab, missionListParent);
            var ui = go.GetComponent<MissionUI>();
            ui.Setup(mission, progress, isCompleted, isClaimed);

            // Add separator after each item except the last one
            if (separatorPrefab != null && i < currentLevelMissions.Count - 1)
            {
                Instantiate(separatorPrefab, missionListParent);
            }
        }
    }

    private void UpdateLevelProgressUI()
    {
        // Get all missions for current level
        var currentLevelMissions = allMissions.Where(m => m.UnlockLevel == currentLevel).ToList();
        int totalMissions = currentLevelMissions.Count;
        int claimedCount = currentLevelMissions.Count(m => claimedMissions.Contains(m.ID));

        // Update level labels
        if (currentLevelLabel != null)
            currentLevelLabel.text = $"Level {currentLevel}";

        if (nextLevelLabel != null)
        {
            // Show "MAX" if current level is the max level, otherwise show next level
            if (currentLevel >= maxLevel)
                nextLevelLabel.text = "MAX";
            else nextLevelLabel.text = $"Level {currentLevel + 1}";
        }

        // Update progress fraction label
        if (progressFractionLabel != null)
            progressFractionLabel.text = $"{claimedCount}/{totalMissions}";

        // Update progress bar fill
        if (levelProgressBarFill != null)
        {
            float percent = totalMissions > 0 ? (float)claimedCount / totalMissions : 0f;

            // Interpolate from 507 (0%) to 5 (100%)
            float rightValue = Mathf.Lerp(PROGRESS_BAR_MIN_RIGHT, PROGRESS_BAR_MAX_RIGHT, percent);

            Vector2 offsetMax = levelProgressBarFill.offsetMax;
            offsetMax.x = -rightValue; // Negative because offsetMax.x is the right offset
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
        // Note: No PlayerPrefs.Save() here - only writing to memory
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
            if (completed == 1)
                completedMissions.Add(mission.ID);
            if (claimed == 1)
                claimedMissions.Add(mission.ID);
        }

        MarkDirty();
    }

    public void ToggleMissions()
    {
        AudioManager.Instance.PlayUISound();

        settingsButton.interactable = !settingsButton.interactable;
        shopButton.interactable = !shopButton.interactable;
        powerupsButton.interactable = !powerupsButton.interactable;

        if (isOpen)
            CloseMissions();
        else
            OpenMissions();
    }

    private void OpenMissions()
    {
        isOpen = true;

        // Refresh UI when opening if dirty
        if (isDirty)
            RefreshUI();

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
