using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public GameUIManager uiManager;

    [Header("UI")]
    public RollingDigitText rollingDigitText;

    private int coins = 0;
    private const string COINS_KEY = "Coins";

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
