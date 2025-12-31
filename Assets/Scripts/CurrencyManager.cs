using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public GameUIManager uiManager;

    [Header("UI")]
    public TextMeshProUGUI coinText;

    private int coins = 0;
    private const string COINS_KEY = "Coins";

    // Coin collection mission thresholds
    private static readonly int[] coinMissionAmounts = { 50, 100, 200, 400, 500, 750, 1000, 2000, 3500, 5000, 7500, 10000, 20000 };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt(COINS_KEY, 0);
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt(COINS_KEY, coins);
        uiManager.UpdateCoins(coins);

        // Track coin missions
        UpdateCoinMissions(amount);
    }

    private void UpdateCoinMissions(int amount)
    {
        foreach (int threshold in coinMissionAmounts)
        {
            MissionsManager.Instance?.IncrementMission($"MISSION_COINS_{threshold}", amount);
        }
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            PlayerPrefs.SetInt(COINS_KEY, coins);
            uiManager.UpdateCoins(coins);
            return true;
        }
        return false;
    }

    public int GetCoinCount()
    {
        return coins;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }
}
