using UnityEngine;

public enum MissionType
{
    Performance,   // score, blocks, height
    Combo,         // streaks, perfects
    Time,          // survive x seconds
    Economy,       // coins/shop/powerups
    Progression,   // runs, levels
    Special        // fun/weird challenges
}

[CreateAssetMenu(fileName = "NewMission", menuName = "Missions/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string id;       // Unique mission key
    [SerializeField] private MissionType type;
    [SerializeField][TextArea] private string description;

    [Header("Requirements")]
    [SerializeField] private int requiredValue;
    [SerializeField] private bool hasProgressBar = true;
    [SerializeField] private int unlockLevel = 1;

    [Header("Rewards")]
    [SerializeField] private int rewardAmount;

    // Properties (read-only)
    public string ID => id;
    public MissionType Type => type;
    public string Description => description;
    public int RequiredValue => requiredValue;
    public bool HasProgressBar => hasProgressBar;
    public int UnlockLevel => unlockLevel;
    public int RewardAmount => rewardAmount;
}
