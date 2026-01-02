using UnityEngine;
using System;

public class DailyRewardsDebugger : MonoBehaviour
{
    private const string LAST_LOGIN_KEY = "LastLoginDate";
    private const string STREAK_KEY = "DailyStreak";

    [Header("Debug Info")]
    public string storedDate;
    public int storedStreak;

    private void Update()
    {
        // Keep these updated so you can see them in Inspector
        storedDate = PlayerPrefs.GetString(LAST_LOGIN_KEY, "None");
        storedStreak = PlayerPrefs.GetInt(STREAK_KEY, 0);
    }

    [ContextMenu("Simulate: It is Tomorrow (Ready to Claim)")]
    public void SimulateTomorrow()
    {
        // Set last login to Yesterday
        // This makes the game think 24 hours have passed
        string yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        PlayerPrefs.SetString(LAST_LOGIN_KEY, yesterday);
        PlayerPrefs.Save();

        Debug.Log($"<color=cyan>[DEBUG]</color> Time Travel Successful! Game thinks you last played on: {yesterday}");

        // Refresh the UI immediately
        RefreshManager();
    }

    [ContextMenu("Simulate: Streak Broken (Missed a Day)")]
    public void SimulateStreakBreak()
    {
        // Set last login to 2 Days Ago
        // This makes the game think 48+ hours have passed
        string twoDaysAgo = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
        PlayerPrefs.SetString(LAST_LOGIN_KEY, twoDaysAgo);
        PlayerPrefs.Save();

        Debug.Log($"<color=red>[DEBUG]</color> Streak Broken! Game thinks you last played on: {twoDaysAgo}");

        // Refresh the UI immediately
        RefreshManager();
    }

    [ContextMenu("Reset: Fresh Install (Day 1)")]
    public void ResetDailyRewards()
    {
        PlayerPrefs.DeleteKey(LAST_LOGIN_KEY);
        PlayerPrefs.DeleteKey(STREAK_KEY);
        PlayerPrefs.Save();

        Debug.Log($"<color=orange>[DEBUG]</color> Daily Rewards Reset to Day 1.");

        RefreshManager();
    }

    [ContextMenu("Cheat: Add 1000 Coins")]
    public void AddCoins()
    {
        CurrencyManager.Instance.AddCoins(1000);
    }

    private void RefreshManager()
    {
        if (DailyRewardsManager.Instance != null)
        {
            // Force the manager to re-check logic and update UI
            DailyRewardsManager.Instance.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);
            // Or if Initialize is private, we can just toggle the object or call UpdateUI via reflection
            // But since your UpdateUI is public, let's call that:

            // We need to re-run the streak logic first, which might be private in your script.
            // A simple trick is to disable and re-enable the component:
            DailyRewardsManager.Instance.enabled = false;
            DailyRewardsManager.Instance.enabled = true;

            Debug.Log("UI Refreshed.");
        }
    }
}