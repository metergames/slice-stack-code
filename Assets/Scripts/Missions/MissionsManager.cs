using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager Instance;

    [Header("Mission Setup")]
    public List<MissionData> allMissions; // Drag your ScriptableObjects here
    public Transform missionListParent;   // The "Content" transform of your ScrollView
    public GameObject missionUIPrefab;    // A prefab row (we’ll build that next)

    private Dictionary<string, int> missionProgress = new();
    private HashSet<string> completedMissions = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadProgress();
        RefreshUI();
    }

    public void UpdateMission(string missionID, int increment)
    {
        if (!missionProgress.ContainsKey(missionID))
            missionProgress[missionID] = 0;

        // Update
        missionProgress[missionID] += increment;

        var mission = allMissions.Find(m => m.ID == missionID);
        if (mission != null && missionProgress[missionID] >= mission.RequiredValue && !completedMissions.Contains(missionID))
        {
            CompleteMission(mission);
        }

        RefreshUI();
        SaveProgress();
    }

    private void CompleteMission(MissionData mission)
    {
        completedMissions.Add(mission.ID);
        CurrencyManager.Instance.AddCoins(mission.RewardAmount);
        Debug.Log($"Mission completed: {mission.Description}, rewarded {mission.RewardAmount} coins");
    }

    public void RefreshUI()
    {
        // Clear old rows
        foreach (Transform child in missionListParent)
            Destroy(child.gameObject);

        // Create new rows
        foreach (var mission in allMissions)
        {
            int progress = missionProgress.ContainsKey(mission.ID) ? missionProgress[mission.ID] : 0;
            bool isCompleted = completedMissions.Contains(mission.ID);

            var go = Instantiate(missionUIPrefab, missionListParent);
            go.GetComponent<MissionUI>().Setup(mission, progress, isCompleted);
        }
    }

    private void SaveProgress()
    {
        foreach (var mission in allMissions)
        {
            PlayerPrefs.SetInt($"{mission.ID}_Progress", missionProgress.ContainsKey(mission.ID) ? missionProgress[mission.ID] : 0);
            PlayerPrefs.SetInt($"{mission.ID}_Completed", completedMissions.Contains(mission.ID) ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        missionProgress.Clear();
        completedMissions.Clear();

        foreach (var mission in allMissions)
        {
            int progress = PlayerPrefs.GetInt($"{mission.ID}_Progress", 0);
            int completed = PlayerPrefs.GetInt($"{mission.ID}_Completed", 0);

            missionProgress[mission.ID] = progress;
            if (completed == 1)
                completedMissions.Add(mission.ID);
        }
    }
}
